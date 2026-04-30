export class PaginatedResultModel<T> {
  items: T[] = [];
  totalCount: number = 0;
  pageNumber: number = 0;
  pageSize: number = 0;
  totalPages: number = 0;
  hasNextPage: boolean = false;
  hasPreviousPage: boolean = false;

  static fromApi<T>(raw: unknown): PaginatedResultModel<T> {
    const data = (raw ?? {}) as Record<string, unknown>;
    const result = new PaginatedResultModel<T>();

    result.items = (data['items'] ?? data['Items'] ?? []) as T[];
    result.totalCount = Number(data['totalCount'] ?? data['TotalCount'] ?? 0);
    result.pageNumber = Number(data['pageNumber'] ?? data['PageNumber'] ?? 0);
    result.pageSize = Number(data['pageSize'] ?? data['PageSize'] ?? 0);
    result.totalPages = Number(data['totalPages'] ?? data['TotalPages'] ?? 0);
    result.hasNextPage = Boolean(data['hasNextPage'] ?? data['HasNextPage'] ?? false);
    result.hasPreviousPage = Boolean(data['hasPreviousPage'] ?? data['HasPreviousPage'] ?? false);

    return result;
  }
}