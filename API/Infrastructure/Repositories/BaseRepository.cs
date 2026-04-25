namespace Infrastructure.Repositories
{
    public abstract class BaseRepository(IDbConnectionFactory factory)
    {
        protected async Task<IDbConnection> CreateConnectionAsync(CancellationToken ct) =>
            await factory.CreateConnectionAsync(ct);


        protected async Task<PaginatedResult<T>> ExecutePaginatedQueryAsync<T>(
            QueryParams queryParams,
            string countSql,
            string selectSql,
            Dictionary<string, string> allowedSortColumns,
            string defaultSortColumn,
            string? searchCondition,
            CancellationToken ct)
        {
            using var connection = await CreateConnectionAsync(ct);

            var parameters = new DynamicParameters();
            var conditions = new List<string>();

            var pageNumber = queryParams.PageNumber ?? 1;
            var pageSize = Math.Min(queryParams.PageSize ?? 10, 50);

            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm) && searchCondition is not null)
            {
                conditions.Add(searchCondition);
                parameters.Add("SearchTerm", $"%{queryParams.SearchTerm}%");
            }

            var whereClause = conditions.Count != 0
                ? $"WHERE {string.Join(" AND ", conditions)}"
                : string.Empty;

            var orderByColumn = !string.IsNullOrWhiteSpace(queryParams.SortBy)
                    && allowedSortColumns.TryGetValue(queryParams.SortBy, out var dbColumn)
                ? dbColumn
                : defaultSortColumn;

            var sortDirection = (queryParams.SortDescending ?? false) ? "DESC" : "ASC";

            var sql = $@"
                {countSql} {whereClause};
                {selectSql} {whereClause}
                ORDER BY {orderByColumn} {sortDirection}
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = (await multi.ReadAsync<T>()).AsList();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
