using Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<(IReadOnlyList<T> Items, int TotalCount)> PaginateAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, count);
    }

    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, count) = await query.PaginateAsync(pageNumber, pageSize, cancellationToken);
        return new PaginatedResult<T>(items, count, pageNumber, pageSize);
    }
}
