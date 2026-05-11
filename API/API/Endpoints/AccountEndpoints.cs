using Application.DTOs.Account;
using Application.Features.Account.Queries.GetAll;
using Application.Features.Account.Queries.GetById;

namespace API.Endpoints
{
    public class AccountEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/account")
                .WithTags("Account");

            group.MapGet("/users", GetUsers)
                .WithName(nameof(GetUsers))
                .Produces<PaginatedResult<UserResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/users/{id:guid}", GetUserById)
                .WithName(nameof(GetUserById))
                .Produces<UserResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
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
    }
}
