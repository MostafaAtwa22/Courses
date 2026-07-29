import { Gender } from '../../../shared/models/identity.models';

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto extends LoginDto {
  firstName: string;
  lastName: string;
  userName: string;
  gender: Gender;
  confirmPassword: string;
  role: string;
}

export interface BaseIdentityResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  phoneNumber?: string;
  profilePicture?: string;
  is2FAEnable: boolean;
  hasPassword: boolean;
  gender: Gender;
  roles: string[];
  createdAt?: string;
}

export interface AuthResponseDto extends BaseIdentityResponse {
  token?: string;
  requiresTwoFactor: boolean;
  provider?: string;
}
