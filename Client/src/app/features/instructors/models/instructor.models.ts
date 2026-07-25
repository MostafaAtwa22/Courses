export interface InstructorCreateRequest {
  bio: string;
  title: string;
  linkedInProfileUrl: string;
  gitHubProfileUrl: string;
  cvUrl: File;
}

export interface InstructorResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  profilePicture: string | null;
  gender: string;
  bio: string;
  title: string;
  linkedInProfileUrl: string;
  gitHubProfileUrl: string;
  status: string;
}

export interface InstructorPrivateResponse extends InstructorResponse {
  phoneNumber: string;
  cvUrl: string;
}

export interface InstructorPublicResponse extends InstructorResponse {
}
