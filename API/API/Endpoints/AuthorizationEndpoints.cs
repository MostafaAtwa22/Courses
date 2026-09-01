using Application.DTOs.Account;
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
            .WithTags(nameof(GetRoles))
            .Produces<IReadOnlyCollection<RoleResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
        
    public static async Task<Results<Ok<IReadOnlyCollection<RoleResponseDto>>, BadRequest>> GetRoles(IMediator mediator)
    {
        var result = await mediator.Send(new GetRolesQuery());
        return TypedResults.Ok(result);
    }
}