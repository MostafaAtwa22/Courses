using Application.DTOs.Authorization;
using Application.Features.Authorization.Queries.GetAll;

namespace API.Endpoints;

public class AuthorizationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/authorization")
            .WithTags("Authorization")
            .RequireAuthorization(policy =>
                policy.RequireRole(
                    Role.Admin.ToString(),
                    Role.SuperAdmin.ToString()));
            
        group.Map("/roles", GetRoles)
            .WithName(nameof(GetRoles))
            .WithTags(nameof(GetRoles))
            .Produces<IReadOnlyCollection<RolesResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.Map("/users/{userId}/roles", GetRoleByUserId)
            .WithName(nameof(GetRoleByUserId))
            .WithTags(nameof(GetRoleByUserId))
            .Produces<UserRolesManageDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }
        
    public static async Task<Results<Ok<IReadOnlyCollection<RolesResponseDto>>, BadRequest>> GetRoles(IMediator mediator)
    {
        var result = await mediator.Send(new GetRolesQuery());
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<UserRolesManageDto>, NotFound>> GetRoleByUserId(string userId, IMediator mediator)
    {
        var result = await mediator.Send(new GetRoleByUserIdQuery(userId));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}