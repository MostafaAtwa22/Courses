using System.Data;

namespace Application.Common.Interfaces
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync(CancellationToken ct = default);
    }
}