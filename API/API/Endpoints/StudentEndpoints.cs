using Application.DTOs.Student;
using Application.Features.Student.Commands.DeleteStudent;
using Application.Features.Student.Queries.GetAll;
using Application.Features.Student.Queries.GetById;
using Application.Features.Student.Queries.GetByUserId;

namespace API.Endpoints;

public class StudentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/students")
            .WithTags("Students")
            .RequireAuthorization(policy =>
                policy.RequireRole(
                    Role.Admin.ToString(),
                    Role.SuperAdmin.ToString()));

        group.MapGet("/", GetAllStudents)
            .WithName(nameof(GetAllStudents))
            .Produces<PaginatedResult<StudentResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetStudentById)
            .WithName(nameof(GetStudentById))
            .Produces<StudentResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/by-user/{userId}", GetStudentByUserId)
            .WithName(nameof(GetStudentByUserId))
            .Produces<StudentResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteStudent)
            .WithName(nameof(DeleteStudent))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<Results<Ok<PaginatedResult<StudentResponseDto>>, BadRequest>> GetAllStudents(
        [AsParameters] StudentQueryParams queryParams,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetStudentsQuery(queryParams));
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<StudentResponseDto>, NotFound>> GetStudentById(
        Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetStudentByIdQuery(id));
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<StudentResponseDto>, NotFound>> GetStudentByUserId(
        string userId, IMediator mediator)
    {
        var result = await mediator.Send(new GetStudentByUserIdQuery(userId));
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteStudent(
        Guid id, IMediator mediator)
    {
        await mediator.Send(new DeleteStudentCommand(id));
        return TypedResults.NoContent();
    }
}
