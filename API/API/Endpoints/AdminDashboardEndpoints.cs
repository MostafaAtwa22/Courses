using Application.DTOs.AdminDashboard;
using Application.DTOs.Course;
using Application.Features.AdminDashboard.Queries.GetEnrollmentStatistics;
using Application.Features.AdminDashboard.Queries.GetRoleStatistics;
using Application.Features.Courses.Queries.GetTopPerformingCourses;
using Infrastructure.Constants;
using Infrastructure.Enums;

namespace API.Endpoints
{
    public class AdminDashboardEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin/dashboard")
                .WithTags("AdminDashboard");

            group.MapGet("/role-statistics", GetRoleStatistics)
                .WithName(nameof(GetRoleStatistics))
                .RequireAuthorization(PermissionConstants.Build(Module.Admin, CRUD.Read))
                .Produces<RoleStatisticsDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden);

            group.MapGet("/courses/top-performing", GetTopPerformingCourses)
                .WithName(nameof(GetTopPerformingCourses))
                .RequireAuthorization(PermissionConstants.Build(Module.Admin, CRUD.Read))
                .Produces<IEnumerable<AdminCourseAnalyticsDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden);

            group.MapGet("/enrollment-statistics", GetEnrollmentStatistics)
                .WithName(nameof(GetEnrollmentStatistics))
                .RequireAuthorization(PermissionConstants.Build(Module.Admin, CRUD.Read))
                .Produces<IEnumerable<EnrollmentStatisticsDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden);
        }

        public static async Task<IResult> GetRoleStatistics(IMediator mediator)
        {
            var result = await mediator.Send(new GetRoleStatisticsQuery());
            return TypedResults.Ok(result);
        }

        public static async Task<IResult> GetTopPerformingCourses(IMediator mediator, int limit = 5)
        {
            var result = await mediator.Send(new GetTopPerformingCoursesQuery(limit));
            return TypedResults.Ok(result);
        }

        public static async Task<IResult> GetEnrollmentStatistics(IMediator mediator)
        {
            var result = await mediator.Send(new GetEnrollmentStatisticsQuery());
            return TypedResults.Ok(result);
        }
    }
}
