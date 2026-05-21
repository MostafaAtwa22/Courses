import { BaseIdentityResponse } from '../../auth/models/auth.models';
import { Gender } from '../../../shared/models/identity.models';
import { QueryParams } from '../../../shared/models/query-params.model';

export interface UserResponseDto extends BaseIdentityResponse {}

export interface LockUserDto {
  lockoutUntil?: string;
  reason: string;
}

export interface ForgetPasswordDto {
  email: string;
}

export interface ResetPasswordDto extends ForgetPasswordDto {
  token: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface UserQueryParams extends QueryParams {
  gender?: Gender;
  role?: string;
}
