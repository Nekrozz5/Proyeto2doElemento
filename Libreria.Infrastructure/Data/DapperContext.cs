using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using Dapper;
using Libreria.Core.Enums;
using Libreria.Core.Interfaces;

namespace Libreria.Infrastructure.Data
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _connFactory;
        private static readonly AsyncLocal<(IDbConnection? Conn, IDbTransaction? Tx)> _ambient = new();

        public DatabaseProvider Provider => _connFactory.Provider;

        public DapperContext(IDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx)
        {
            _ambient.Value = (conn, tx);
        }

        public void ClearAmbientConnection()
        {
            _ambient.Value = (null, null);
        }

        private (IDbConnection conn, IDbTransaction? tx, bool owns) GetConnAndTx()
        {
            var ambient = _ambient.Value;
            if (ambient.Conn != null)
                return (ambient.Conn, ambient.Tx, false);

            var conn = _connFactory.CreateConnection();
            return (conn, null, true);
        }

        private async Task OpenIfNeededAsync(IDbConnection conn)
        {
            if (conn is DbConnection dbConn && dbConn.State == ConnectionState.Closed)
                await dbConn.OpenAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryAsync<T>(new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.ExecuteAsync(new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();
            try
            {
                await OpenIfNeededAsync(conn);
                var res = await conn.ExecuteScalarAsync(new CommandDefinition(sql, param, tx, commandType: commandType));
                if (res == null || res == DBNull.Value) return default!;
                return (T)Convert.ChangeType(res, typeof(T));
            }
            finally
            {
                if (owns)
                {
                    if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                        await dbConn.CloseAsync();
                    conn.Dispose();
                }
            }
        }
    }
}
