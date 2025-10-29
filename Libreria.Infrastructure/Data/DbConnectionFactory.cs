using Libreria.Core.Enums;
using Libreria.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace Libreria.Infrastructure.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;
        private readonly string _sqlConn;
        private readonly string _mySqlConn;
        public DatabaseProvider Provider { get; }

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
            _sqlConn = _config.GetConnectionString("ConnectionSqlServer") ?? string.Empty;
            _mySqlConn = _config.GetConnectionString("ConnectionMySql") ?? string.Empty;

            var providerStr = _config.GetSection("DatabaseProvider").Value ?? "MySql";

            Provider = providerStr.Equals("MySql", StringComparison.OrdinalIgnoreCase)
                ? DatabaseProvider.MySql
                : DatabaseProvider.SqlServer;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_mySqlConn);

        }
    }
}
