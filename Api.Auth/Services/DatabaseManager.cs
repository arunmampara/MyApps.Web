using Microsoft.Data.SqlClient;
using System.Data;

namespace Api.Auth.Services
{
    /// <summary>
    /// A reusable database manager for executing SQL Server stored procedures.
    /// Compatible with .NET 6+ / .NET 10.
    /// </summary>
    public sealed class DatabaseManager : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;

        public DatabaseManager(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Opens the SQL connection if not already open.
        /// </summary>
        private async Task EnsureConnectionOpenAsync()
        {
            _connection ??= new SqlConnection(_connectionString);

            if (_connection.State != ConnectionState.Open)
                await _connection.OpenAsync();
        }

        /// <summary>
        /// Executes a stored procedure that returns a DataTable.
        /// </summary>
        public async Task<DataTable> ExecuteStoredProcedureAsync(
            string procedureName,
            Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name cannot be null or empty.", nameof(procedureName));

            await EnsureConnectionOpenAsync();

            using var cmd = new SqlCommand(procedureName, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(cmd, parameters);

            var dt = new DataTable();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);

            return dt;
        }

        /// <summary>
        /// Executes a stored procedure that does not return a result set (INSERT, UPDATE, DELETE).
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(
            string procedureName,
            Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name cannot be null or empty.", nameof(procedureName));

            await EnsureConnectionOpenAsync();

            using var cmd = new SqlCommand(procedureName, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(cmd, parameters);

            return await cmd.ExecuteNonQueryAsync();
        }



        /// <summary>
        /// Executes a stored procedure that returns a single scalar value.
        /// </summary>
        public async Task<object> ExecuteScalarAsync(
            string procedureName,
            Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name cannot be null or empty.", nameof(procedureName));

            await EnsureConnectionOpenAsync();

            using var cmd = new SqlCommand(procedureName, _connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(cmd, parameters);

            return await cmd.ExecuteScalarAsync();
        }

        /// <summary>
        /// Adds parameters to the SqlCommand.
        /// </summary>
        private static void AddParameters(SqlCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
