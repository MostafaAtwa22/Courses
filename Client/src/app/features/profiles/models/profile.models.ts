import { Gender } from '../../../shared/models/identity.models';

export interface UpdateProfileDto {
  firstName: string;
  lastName: string;
  userName: string;
  phoneNumber?: string;
  gender: Gender;
}

export interface DeleteProfileDto {
  password: string;
}

export interface SetPasswordDto {
  newPassword: string;
  confirmNewPassword: string;
}

export interface ChangePasswordDto extends SetPasswordDto {
  oldPassword: string;
}
