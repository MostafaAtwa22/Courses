using Application.DTOs.Account;
using Application.Features.Account.Commands.Lock;
using Application.Features.Account.Commands.UnLock;
using Application.Features.Account.Queries.GetAll;
using Application.Features.Account.Queries.GetById;
using Application.Features.Account.Commands.ForgetPassword;
using Application.Features.Account.Commands.ResetPassword;

namespace API.Endpoints
{
    public class AccountEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/account")
                .WithTags("Account")
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Admin.ToString(),
                        Role.SuperAdmin.ToString()));

            group.MapGet("/users", GetUsers)
                .WithName(nameof(GetUsers))
                .Produces<PaginatedResult<UserResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/users/{id:guid}", GetUserById)
                .WithName(nameof(GetUserById))
                .Produces<UserResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/users/{id:guid}/lock", LockUser)
                .WithName(nameof(LockUser))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);
            
            group.MapPost("/users/{id:guid}/unlock", UnlockUser)
                .WithName(nameof(UnlockUser))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPost("/forget-password", ForgetPassword)
                .WithName(nameof(ForgetPassword))
                .AllowAnonymous()
                .RequireRateLimiting(RateLimiterPolicies.ForgotPassword)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPost("/reset-password", ResetPassword)
                .WithName(nameof(ResetPassword))
                .AllowAnonymous()
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest);
        }

        public static async Task<Results<NoContent, BadRequest>> ForgetPassword(
            ForgetPasswordDto request, IMediator mediator)
        {
            await mediator.Send(new ForgetPasswordCommand(request));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest>> ResetPassword(
            ResetPasswordDto request, IMediator mediator)
        {
            await mediator.Send(new ResetPasswordCommand(request));
            return TypedResults.NoContent();
        }

        public static async Task<Results<Ok<PaginatedResult<UserResponseDto>>, BadRequest>> GetUsers(
            [AsParameters] UserQueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetUsersQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<UserResponseDto>, NotFound>> GetUserById(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetUserByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<NoContent, NotFound, BadRequest>> LockUser(
            Guid id, LockUserDto request, IMediator mediator)
        {
            await mediator.Send(new LockUserCommand(id, request));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound, BadRequest>> UnlockUser(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new UnLockUserCommand(id));
            return TypedResults.NoContent();
        }
    }
}
