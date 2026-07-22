# Requirements Document

## Introduction

This feature restricts access to the private instructor response endpoint (`GET /instructors/private/{id}`) so that sensitive instructor data — specifically `PhoneNumber` and `CvUrl` — is only visible to authorized parties. Currently, the endpoint allows any authenticated user with the `Instructor` or `Admin` role to view another instructor's private data. After this change, only the instructor who owns the record, users with the `Admin` role, and users with the `SuperAdmin` role may access a private instructor response.

## Glossary

- **Private_Instructor_Response**: The response returned by `GET /instructors/private/{id}`, containing sensitive fields such as `PhoneNumber` and `CvUrl`, in addition to the common instructor fields.
- **Requesting_User**: The authenticated user making the API request.
- **Owner**: The instructor whose record is being requested (i.e., their `UserId` matches the requested resource `id`).
- **Admin**: A user with the `Admin` role.
- **SuperAdmin**: A user with the `SuperAdmin` role.
- **Instructor**: A user with the `Instructor` role who is not the Owner of the requested record.
- **Student**: A user with the `Student` role.
- **Authorization_Service**: The application component responsible for evaluating access permissions before returning data.
- **Private_Instructor_Endpoint**: The API endpoint `GET /instructors/private/{id}`.

## Requirements

### Requirement 1: Role-Based Access Control for Private Instructor Responses

**User Story:** As an Admin or SuperAdmin, I want to view the full private profile of any instructor, so that I can manage instructor data and review sensitive information.

#### Acceptance Criteria

1. WHEN the Requesting_User holds the `Admin` role and sends a request to the Private_Instructor_Endpoint, THE Authorization_Service SHALL permit access and return the Private_Instructor_Response with HTTP 200.
2. WHEN the Requesting_User holds the `SuperAdmin` role and sends a request to the Private_Instructor_Endpoint, THE Authorization_Service SHALL permit access and return the Private_Instructor_Response with HTTP 200.
3. WHEN the Requesting_User is the Owner of the requested record and sends a request to the Private_Instructor_Endpoint, THE Authorization_Service SHALL permit access and return the Private_Instructor_Response with HTTP 200.
4. WHEN the Requesting_User holds the `Instructor` role and IF the Requesting_User is not the Owner of the requested record, THE Authorization_Service SHALL deny access and return an HTTP 403 Forbidden response.
5. WHEN the Requesting_User holds the `Student` role and sends a request to the Private_Instructor_Endpoint, THE Authorization_Service SHALL deny access and return an HTTP 403 Forbidden response.
6. WHEN the Requesting_User is unauthenticated and sends a request to the Private_Instructor_Endpoint, THE Authorization_Service SHALL return an HTTP 401 Unauthorized response.
7. WHEN the Requesting_User holds both the `Instructor` role and the `Admin` role and sends a request to the Private_Instructor_Endpoint, THE Authorization_Service SHALL permit access and return the Private_Instructor_Response with HTTP 200, treating the `Admin` role as granting full access regardless of ownership.

### Requirement 2: Ownership Verification

**User Story:** As an instructor, I want to be able to view my own private profile, so that I can see what sensitive information the system holds about me.

#### Acceptance Criteria

1. WHEN the Requesting_User holds the `Instructor` role and the requested record's `UserId` matches the `UserId` claim in the Requesting_User's authentication token, THE Authorization_Service SHALL permit access and return the Private_Instructor_Response with HTTP 200.
2. WHEN the Requesting_User holds the `Instructor` role and the requested record's `UserId` does not match the `UserId` claim in the Requesting_User's authentication token, THE Authorization_Service SHALL return an HTTP 403 Forbidden response, and the response body SHALL NOT contain any fields from `InstructorPrivateResponseDto`.
3. IF the instructor record identified by the route parameter `id` does not exist in the data store, THEN THE Private_Instructor_Endpoint SHALL return an HTTP 404 Not Found response regardless of the Requesting_User's role.

### Requirement 3: No Leakage Through Public Endpoint

**User Story:** As a system administrator, I want to ensure that sensitive instructor fields are never exposed through the public endpoint, so that private data cannot be accessed by bypassing the access control.

#### Acceptance Criteria

1. THE `PhoneNumber` and `CvUrl` fields SHALL be defined exclusively in `InstructorPrivateResponseDto` and SHALL NOT be present in `InstructorCommonResponseDto` or any other shared DTO.
2. WHEN any user sends a request to the public instructor endpoint `GET /instructors/public/{id}`, THE response body SHALL NOT contain a `PhoneNumber` or `CvUrl` field, regardless of the Requesting_User's role.
3. WHEN the Requesting_User is permitted access to the Private_Instructor_Endpoint, THE Authorization_Service SHALL return an HTTP 200 response whose body contains all fields inherited from `InstructorCommonResponseDto` as well as `PhoneNumber` and `CvUrl`.

### Requirement 4: Unchanged Behavior for Permitted Roles

**User Story:** As an Admin, I want existing access workflows to remain unaffected, so that my day-to-day tasks are not disrupted by this change.

#### Acceptance Criteria

1. WHEN the Requesting_User holds the `Admin` role and sends a request to the Private_Instructor_Endpoint, THE Private_Instructor_Endpoint SHALL return HTTP 200 and the response body SHALL contain all fields defined in `InstructorCommonResponseDto` plus `PhoneNumber` and `CvUrl`, all non-null where data exists.
2. WHEN the Requesting_User is the Owner and sends a request to the Private_Instructor_Endpoint, THE Private_Instructor_Endpoint SHALL return HTTP 200 and the response body SHALL contain all fields defined in `InstructorCommonResponseDto` plus `PhoneNumber` and `CvUrl`, all non-null where data exists.
