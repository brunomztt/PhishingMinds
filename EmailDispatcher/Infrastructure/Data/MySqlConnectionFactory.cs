using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EmailDispatcher.Infrastructure.Data
{
    public class MySqlConnectionFactory
    {
        private readonly string _conn;

        public MySqlConnectionFactory(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_conn);
        }
    }
}
