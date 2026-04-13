using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace PhishingMinds.Server.Data
{
    public class DbConnectionFactory
    {
        private readonly string?_connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}