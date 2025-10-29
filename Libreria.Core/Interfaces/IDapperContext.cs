using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Libreria.Core.Enums;

namespace Libreria.Core.Interfaces
{
    public interface IDapperContext
    {
        DatabaseProvider Provider { get; }

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);

        void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx);
        void ClearAmbientConnection();
    }
}
