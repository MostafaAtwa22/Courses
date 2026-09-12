import { BaseResponseModel } from "../../../shared/models/base-response.model";

export interface StudentResponse extends BaseResponseModel {
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
  sortBy?: string;
  sortDescending?: boolean;
}
