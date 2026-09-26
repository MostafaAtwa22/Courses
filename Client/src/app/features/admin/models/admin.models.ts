import { BaseResponseModel } from "../../../shared/models/base-response.model";

export interface AdminResponse extends BaseResponseModel {
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  profilePicture: string | null;
  gender: string;
  role: string;
}

export interface AdminQueryParams {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  gender?: string;
  role?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface AdminCreateDto {
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  password: string;
  confirmPassword: string;
  gender: string;
  role: string;
}