using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace PhishingMinds.Tests
{
    public class MockDbConnection : DbConnection
    {
        private readonly Func<string, object, object> _queryHandler;

        public MockDbConnection(Func<string, object, object> queryHandler)
        {
            _queryHandler = queryHandler;
        }

        protected override DbCommand CreateDbCommand()
        {
            return new MockDbCommand(this, _queryHandler);
        }

        public override string ConnectionString { get; set; } = "MockConnectionString";
        public override string Database => "MockDB";
        public override string DataSource => "MockDataSource";
        public override string ServerVersion => "MockServerVersion";
        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName) { }
        public override void Close() { }
        public override void Open() { }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new MockDbTransaction(this);
        }
    }

    public class MockDbTransaction : DbTransaction
    {
        public MockDbTransaction(DbConnection connection)
        {
            DbConnection = connection;
        }

        public override void Commit() { }
        public override void Rollback() { }
        protected override DbConnection DbConnection { get; }
        public override IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
    }

    public class MockDbCommand : DbCommand
    {
        private readonly MockDbConnection _connection;
        private readonly Func<string, object, object> _queryHandler;
        private readonly DbParameterCollection _parameters = new MockDbParameterCollection();

        public MockDbCommand(MockDbConnection connection, Func<string, object, object> queryHandler)
        {
            _connection = connection;
            _queryHandler = queryHandler;
        }

        public override string CommandText { get; set; } = "";
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => _parameters;
        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel() { }
        
        public override int ExecuteNonQuery()
        {
            var pObj = GetParamsObject();
            var res = _queryHandler(CommandText, pObj);
            return res is int i ? i : 1;
        }

        public override object ExecuteScalar()
        {
            var pObj = GetParamsObject();
            return _queryHandler(CommandText, pObj);
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var pObj = GetParamsObject();
            var res = _queryHandler(CommandText, pObj);
            return new MockDbDataReader(res);
        }

        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new MockDbParameter();

        private object GetParamsObject()
        {
            var dict = new Dictionary<string, object>();
            foreach (MockDbParameter p in _parameters)
            {
                dict[p.ParameterName] = p.Value;
            }
            return dict;
        }
    }

    public class MockDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; } = "";
        public override int Size { get; set; }
        public override string SourceColumn { get; set; } = "";
        public override bool SourceColumnNullMapping { get; set; }
        public override object Value { get; set; } = DBNull.Value;
        public override void ResetDbType() { }
    }

    public class MockDbParameterCollection : DbParameterCollection
    {
        private readonly List<object> _list = new List<object>();

        public override int Add(object value)
        {
            _list.Add(value);
            return _list.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var v in values) _list.Add(v);
        }

        public override void Clear() => _list.Clear();
        public override bool Contains(object value) => _list.Contains(value);
        public override int IndexOf(object value) => _list.IndexOf(value);
        public override void Insert(int index, object value) => _list.Insert(index, value);
        public override void Remove(object value) => _list.Remove(value);
        public override void RemoveAt(int index) => _list.RemoveAt(index);

        protected override DbParameter GetParameter(int index) => (DbParameter)_list[index];
        
        protected override DbParameter GetParameter(string parameterName)
        {
            foreach (DbParameter p in _list)
            {
                if (p.ParameterName == parameterName || p.ParameterName == "@" + parameterName)
                    return p;
            }
            return null;
        }

        protected override void SetParameter(int index, DbParameter value) => _list[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var p = GetParameter(parameterName);
            if (p != null)
            {
                var idx = _list.IndexOf(p);
                _list[idx] = value;
            }
            else
            {
                _list.Add(value);
            }
        }

        public override int Count => _list.Count;
        public override object SyncRoot => null;
        public override bool IsSynchronized => false;
        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
        public override void CopyTo(Array array, int index) => _list.ToArray().CopyTo(array, index);
        public override bool Contains(string value) => GetParameter(value) != null;

        public override int IndexOf(string value)
        {
            var p = GetParameter(value);
            return p != null ? _list.IndexOf(p) : -1;
        }

        public override void RemoveAt(string parameterName)
        {
            var p = GetParameter(parameterName);
            if (p != null) _list.Remove(p);
        }
    }

    public class MockDbDataReader : DbDataReader
    {
        private readonly IEnumerable _data;
        private IEnumerator _enumerator;
        private object _current;
        private PropertyInfo[] _properties;

        public MockDbDataReader(object data)
        {
            if (data is IEnumerable enumerable && !(data is string))
            {
                _data = enumerable;
            }
            else
            {
                _data = new List<object> { data };
            }
            _enumerator = _data.GetEnumerator();

            object first = null;
            foreach (var item in _data)
            {
                first = item;
                break;
            }
            if (first != null)
            {
                _properties = first.GetType().GetProperties();
            }
            else
            {
                _properties = new PropertyInfo[0];
                foreach (var i in _data.GetType().GetInterfaces())
                {
                    if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var itemType = i.GetGenericArguments()[0];
                        _properties = itemType.GetProperties();
                        break;
                    }
                }
            }
        }

        public override bool Read()
        {
            var hasNext = _enumerator.MoveNext();
            if (hasNext)
            {
                _current = _enumerator.Current;
                if (_current != null && (_properties == null || _properties.Length == 0))
                {
                    _properties = _current.GetType().GetProperties();
                }
            }
            return hasNext;
        }

        public override int FieldCount => _properties?.Length ?? 0;
        public override string GetName(int ordinal) => _properties[ordinal].Name;

        public override int GetOrdinal(string name)
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                if (string.Equals(_properties[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        public override object GetValue(int ordinal)
        {
            if (_properties == null || ordinal < 0 || ordinal >= _properties.Length)
                return DBNull.Value;
            return _properties[ordinal].GetValue(_current) ?? DBNull.Value;
        }

        public override bool HasRows => true;
        public override bool IsClosed => false;
        public override int RecordsAffected => -1;
        public override int Depth => 0;

        public override bool GetBoolean(int ordinal) => Convert.ToBoolean(GetValue(ordinal));
        public override byte GetByte(int ordinal) => Convert.ToByte(GetValue(ordinal));
        
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            var bytes = (byte[])GetValue(ordinal);
            int available = bytes.Length - (int)dataOffset;
            int toCopy = Math.Min(available, length);
            Array.Copy(bytes, dataOffset, buffer, bufferOffset, toCopy);
            return toCopy;
        }

        public override char GetChar(int ordinal) => Convert.ToChar(GetValue(ordinal));

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            var chars = GetString(ordinal).ToCharArray();
            int available = chars.Length - (int)dataOffset;
            int toCopy = Math.Min(available, length);
            Array.Copy(chars, dataOffset, buffer, bufferOffset, toCopy);
            return toCopy;
        }

        public override string GetDataTypeName(int ordinal) => _properties[ordinal].PropertyType.Name;
        public override DateTime GetDateTime(int ordinal) => Convert.ToDateTime(GetValue(ordinal));
        public override decimal GetDecimal(int ordinal) => Convert.ToDecimal(GetValue(ordinal));
        public override double GetDouble(int ordinal) => Convert.ToDouble(GetValue(ordinal));
        public override Type GetFieldType(int ordinal) => _properties[ordinal].PropertyType;
        public override float GetFloat(int ordinal) => Convert.ToSingle(GetValue(ordinal));
        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);
        public override short GetInt16(int ordinal) => Convert.ToInt16(GetValue(ordinal));
        public override int GetInt32(int ordinal) => Convert.ToInt32(GetValue(ordinal));
        public override long GetInt64(int ordinal) => Convert.ToInt64(GetValue(ordinal));
        public override string GetString(int ordinal) => Convert.ToString(GetValue(ordinal));
        public override bool IsDBNull(int ordinal) => GetValue(ordinal) == DBNull.Value || GetValue(ordinal) == null;
        public override void Close() { }
        public override IEnumerator GetEnumerator() => _enumerator;
        public override bool NextResult() => false;

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name]
        {
            get
            {
                var ord = GetOrdinal(name);
                if (ord == -1) throw new IndexOutOfRangeException($"Column '{name}' not found");
                return GetValue(ord);
            }
        }

        public override int GetValues(object[] values)
        {
            int numElements = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < numElements; i++)
            {
                values[i] = GetValue(i);
            }
            return numElements;
        }
    }
}
