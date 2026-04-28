using Application.Features.Courses.Queries.GetAll;
using Application.Features.Courses.Queries.GetById;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Create;
using Application.Features.Courses.Commands.Update;
using Application.Features.Courses.Commands.Delete;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints
{
    public class CoursesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/courses")
                .WithTags("Courses");

            group.MapGet("/", GetCourses)
                .WithName(nameof(GetCourses))
                .Produces<PaginatedResult<CourseResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetCourseById)
                .WithName(nameof(GetCourseById))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateCourse)
                .WithName(nameof(CreateCourse))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .DisableAntiforgery();
            
            group.MapPut("/{id:guid}", UpdateCourse)
                .WithName(nameof(UpdateCourse))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .DisableAntiforgery();
            
            group.MapDelete("/{id:guid}", DeleteCourse)
                .WithName(nameof(DeleteCourse))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        public static async Task<Results<Ok<PaginatedResult<CourseResponseDto>>, BadRequest>> GetCourses([AsParameters] QueryParams queryParams, 
            IMediator mediator)
        {
            var result = await mediator.Send(new GetCoursesQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<CourseResponseDto>, NotFound>> GetCourseById(Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetCourseByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<CreatedAtRoute<CourseResponseDto>, BadRequest>> CreateCourse(
            [FromForm] CourseCreateDto request, IMediator mediator) 
        {
            var id = await mediator.Send(new CreateCourseCommand(request));
            var course = await mediator.Send(new GetCourseByIdQuery(id));

            return TypedResults.CreatedAtRoute(course!, nameof(GetCourseById), new {id});
        }

        public static async Task<Results<NoContent, NotFound>> UpdateCourse(
            Guid id, [FromForm] CourseUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateCourseCommand(id, request));

            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteCourse(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteCourseCommand(id));

            return TypedResults.NoContent();
        }
    }
}