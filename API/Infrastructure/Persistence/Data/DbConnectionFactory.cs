using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Application.Common.Interfaces;

namespace Infrastructure.Persistence.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken ct = default)
        {
            var connection = new NpgsqlConnection(
                _config.GetConnectionString("DefaultConnection")
            );
            await connection.OpenAsync(ct);
            return connection;
        }
    }
}