using MySqlConnector;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace PhishingMinds.Server.Data
{
    public class DbConnectionFactory
    {
        private readonly string? _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            try
            {
                _connectionString = configuration?.GetConnectionString("DefaultConnection");
            }
            catch
            {
                _connectionString = null;
            }
        }

        public virtual DbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}