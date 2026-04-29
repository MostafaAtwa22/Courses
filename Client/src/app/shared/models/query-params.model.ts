export interface QueryParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export function createQueryParams(
  params?: Partial<QueryParams>
): QueryParams {
  return {
    pageNumber: 1,
    pageSize: 10,
    sortDescending: false,
    searchTerm: '',
    sortBy: '',
    ...params
  };
}