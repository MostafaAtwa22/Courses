export interface UserResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  phoneNumber: string;
  profilePicture?: string;
  gender: string;
  roles: string[];
  isLocked?: boolean;
  lockoutEnd?: string;
}

export interface CheckBoxRoleManage {
  roleId: string;
  roleName: string;
  isSelected: boolean;
}

export interface UserRolesManage {
  roles: CheckBoxRoleManage[];
}

export interface RolesResponse {
  id: string;
  name: string;
}

export interface LockUserDto {
  lockoutEnd: string;
}
