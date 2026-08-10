export interface StudentResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  profilePicture: string | null;
  gender: string;
  totalEnrollments: number;
}

export interface StudentQueryParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  gender?: string;
  courseId?: string;
}
