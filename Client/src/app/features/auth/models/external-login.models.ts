export interface GoogleLoginDto {
    idToken: string;
}

export interface FacebookLoginDto {
    accessToken: string;
}

export interface GithubLoginDto {
    code: string;
    redirectUri: string;
}