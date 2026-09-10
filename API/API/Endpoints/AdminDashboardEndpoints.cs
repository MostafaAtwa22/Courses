using Application.DTOs.AdminDashboard;
using Application.Features.AdminDashboard.Queries.GetRoleStatistics;
using Domain.Enums.Identity;

namespace API.Endpoints
{
    public class AdminDashboardEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin/dashboard")
                .WithTags("AdminDashboard")
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Admin.ToString(),
                        Role.SuperAdmin.ToString()));

            group.MapGet("/role-statistics", GetRoleStatistics)
                .WithName(nameof(GetRoleStatistics))
                .Produces<RoleStatisticsDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden);
        }

        public static async Task<IResult> GetRoleStatistics(IMediator mediator)
        {
            var result = await mediator.Send(new GetRoleStatisticsQuery());
            return TypedResults.Ok(result);
        }
    }
}
