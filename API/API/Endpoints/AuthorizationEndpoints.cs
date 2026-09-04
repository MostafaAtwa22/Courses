using Application.DTOs.Authorization;
using Application.Features.Authorization.Commands.UpdateUserRoles;
using Application.Features.Authorization.Queries.GetAll;
using Application.Features.Authorization.Queries.GetRoleByUserId;

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
            
        group.MapGet("/roles", GetRoles)
            .WithName(nameof(GetRoles))
            .WithTags(nameof(GetRoles))
            .Produces<IReadOnlyCollection<RolesResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/users/{userId}/roles", GetRoleByUserId)
            .WithName(nameof(GetRoleByUserId))
            .WithTags(nameof(GetRoleByUserId))
            .Produces<UserRolesResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPatch("/users/{userId}/roles", UpdateUserRoles)
            .WithName(nameof(UpdateUserRoles))
            .WithTags(nameof(UpdateUserRoles))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
        
    public static async Task<Results<Ok<IReadOnlyCollection<RolesResponseDto>>, BadRequest>> GetRoles(IMediator mediator)
    {
        var result = await mediator.Send(new GetRolesQuery());
        return TypedResults.Ok(result);
    }

    public static async Task<Ok<UserRolesResponseDto>> GetRoleByUserId(string userId, IMediator mediator)
    {
        var result = await mediator.Send(new GetRoleByUserIdQuery(userId));
        return TypedResults.Ok(result);
    }

    public static async Task<Results<NoContent, BadRequest, NotFound>> UpdateUserRoles(string userId, UserRolesManageDto dto, IMediator mediator)
    {
        await mediator.Send(new UpdateUserRolesCommand(userId, dto));
        return TypedResults.NoContent();
    }
}