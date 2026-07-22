# Implementation Plan: Instructor Private Response Visibility

## Overview

Restrict `GET /instructors/private/{id}` so that only Admins, SuperAdmins, and the owning instructor can receive a 200 response. All other authenticated callers receive 403; unauthenticated callers receive 401. Implementation is minimal and consistent with existing patterns: ownership is checked in the endpoint delegate using `ICurrentUserService`, and `ForbiddenException` is thrown for non-owner instructors. FsCheck property-based tests are added to the `API.Tests` project alongside standard unit tests in `Application.Tests`.

## Tasks

- [ ] 1. Add `UserId` to `InstructorPrivateResponseDto` and update repository mapping
  - [ ] 1.1 Add `UserId` property to `InstructorPrivateResponseDto`
    - Open `Application/DTOs/Instructor/InstructorPrivateResponseDto.cs`
    - Add `public string UserId { get; set; } = string.Empty;` — this field is used only for ownership comparison in the endpoint delegate, never returned to callers
    - _Requirements: 2.1, 2.2_

  - [ ] 1.2 Update `PrivateSelectColumns` in `InstructorRepository` to include `user_id`
    - Open `Infrastructure/Repositories/InstructorRepository.cs`
    - Append `i.user_id AS UserId` to the `PrivateSelectColumns` computed property so Dapper maps it into `InstructorPrivateResponseDto.UserId`
    - _Requirements: 2.1, 2.2_

- [ ] 2. Update `RequireAuthorization` and add ownership check in `GetPrivateInstructor`
  - [ ] 2.1 Add `SuperAdmin` role to the `RequireAuthorization` policy on the private endpoint
    - Open `API/Endpoints/InstructorsEndpoints.cs`
    - Change the inline policy to require `Role.Instructor | Role.Admin | Role.SuperAdmin`
    - Add `.Produces(StatusCodes.Status401Unauthorized)` and `.Produces(StatusCodes.Status403Forbidden)` to the endpoint builder chain for accurate OpenAPI metadata
    - _Requirements: 1.1, 1.2, 1.6_

  - [ ] 2.2 Expand `GetPrivateInstructor` delegate to accept `ICurrentUserService` and `ClaimsPrincipal`, and enforce ownership
    - Update the delegate signature to include `ICurrentUserService currentUserService` and `ClaimsPrincipal user` parameters
    - After fetching the record and checking for 404, add the ownership/role guard:
      - If caller has `Admin` or `SuperAdmin` role → short-circuit to `TypedResults.Ok(result)`
      - Otherwise (Instructor role): if `currentUserService.UserId != result.UserId` → `throw new ForbiddenException("You are not authorized to view this instructor's private profile.")`
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.7_

- [ ] 3. Checkpoint — verify the application builds and existing tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 4. Write unit tests for the endpoint ownership logic
  - [ ] 4.1 Create `Tests/API.Tests/Endpoints/InstructorsEndpointsTests.cs` with example-based unit/integration tests
    - Add `FsCheck` and `FsCheck.Xunit` NuGet packages to `API.Tests.csproj` before writing tests
    - Test cases to cover:
      - Admin caller → HTTP 200
      - SuperAdmin caller → HTTP 200
      - Instructor caller with matching `UserId` → HTTP 200
      - Instructor caller with non-matching `UserId` → HTTP 403
      - Student caller → HTTP 403 (blocked by `RequireRole` policy)
      - Unauthenticated caller → HTTP 401
      - Admin caller, record not found → HTTP 404
      - Instructor who also holds Admin role → HTTP 200 (Admin takes precedence)
    - Use `_factory.CurrentUserServiceMock.Setup(u => u.UserId).Returns(...)` to inject the requesting user's identity, and JWT bearer tokens (or the test factory's auth bypass) for role assignment — follow the pattern from `ReviewsEndpointsTests.cs`
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 2.1, 2.2, 2.3_

  - [ ] 4.2 Write property-based test for Property 1: Admin/SuperAdmin always allowed
    - `// Feature: instructor-private-response-visibility, Property 1: Admin and SuperAdmin always receive the private DTO`
    - Use `FsCheck.Xunit` `[Property]` attribute with minimum 100 iterations
    - Generate random `InstructorPrivateResponseDto` instances with random `UserId`s; for each, assert HTTP 200 when caller has `Admin` or `SuperAdmin` role
    - **Property 1: Admin and SuperAdmin always receive the private DTO**
    - **Validates: Requirements 1.1, 1.2, 1.7**

  - [ ] 4.3 Write property-based test for Property 2: Owner always allowed
    - `// Feature: instructor-private-response-visibility, Property 2: Owner receives the private DTO`
    - Generate random instructor records; set requesting user `UserId` equal to `record.UserId` for each iteration; assert HTTP 200
    - **Property 2: Owner receives the private DTO**
    - **Validates: Requirements 1.3, 2.1, 4.2**

  - [ ] 4.4 Write property-based test for Property 3: Non-owner instructor always receives 403
    - `// Feature: instructor-private-response-visibility, Property 3: Non-owner instructor always receives 403`
    - Generate random instructor records; guarantee requesting user `UserId` ≠ `record.UserId` (e.g. use `Gen.suchThat`); assert HTTP 403 and that response body contains no `PhoneNumber` or `CvUrl`
    - **Property 3: Non-owner instructor always receives 403**
    - **Validates: Requirements 1.4, 2.2**

  - [ ] 4.5 Write property-based test for Property 6: Non-existent record returns 404 for any caller
    - `// Feature: instructor-private-response-visibility, Property 6: Non-existent record returns 404 for any caller`
    - Generate random roles (including Admin/SuperAdmin) and random non-existent GUIDs; assert HTTP 404 for every combination
    - **Property 6: Non-existent record returns 404 for any caller**
    - **Validates: Requirements 2.3**

  - [ ] 4.6 Write property-based test for Property 7: Public endpoint never leaks private fields
    - `// Feature: instructor-private-response-visibility, Property 7: Public endpoint never leaks private fields`
    - Generate random `InstructorPublicResponseDto`-equivalent responses; deserialize and assert no `phoneNumber` or `cvUrl` key in the JSON for any caller
    - **Property 7: Public endpoint never leaks private fields**
    - **Validates: Requirements 3.1, 3.2**

- [ ] 5. Final checkpoint — ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for a faster MVP
- Each task references specific requirements for traceability
- FsCheck (and FsCheck.Xunit) must be added to `API.Tests.csproj` before the property tests can compile — do this as part of task 4.1
- The `GetPrivateInstructor` delegate change (task 2.2) is the only behavior change; all other tasks are plumbing (DTO field, SQL column, test coverage)
- Property tests P4 (Student) and P5 (Unauthenticated) are covered as example-based tests in task 4.1 — iterating over them with FsCheck would not surface additional bugs
- `InstructorPrivateResponseDto.UserId` is an implementation detail for ownership comparison; it is intentionally not present on `InstructorCommonResponseDto` or `InstructorPublicResponseDto`

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1", "1.2"] },
    { "id": 1, "tasks": ["2.1", "2.2"] },
    { "id": 2, "tasks": ["4.1"] },
    { "id": 3, "tasks": ["4.2", "4.3", "4.4", "4.5", "4.6"] }
  ]
}
```
