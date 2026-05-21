export interface Disable2FADto {
  password: string;
  code: string;
}

export interface VerifyTwoFactorDto {
  email: string;
  code: string;
}

export interface ConfirmEmailDto extends VerifyTwoFactorDto {}
