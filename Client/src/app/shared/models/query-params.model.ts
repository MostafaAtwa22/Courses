export interface QueryParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface CourseQueryParams extends QueryParams {
  category?: string;
  minRating?: number;
  maxRating?: number;
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

export function createCourseQueryParams(
  params?: Partial<CourseQueryParams>
): CourseQueryParams {
  return {
    ...createQueryParams(params),
    category: '',
    minRating: undefined,
    maxRating: undefined,
    ...params
  };
}