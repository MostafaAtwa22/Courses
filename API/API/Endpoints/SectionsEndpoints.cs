using Application.DTOs.Section;
using Application.Features.Sections.Commands.Create;
using Application.Features.Sections.Commands.Delete;
using Application.Features.Sections.Commands.Update;
using Application.Features.Sections.Queries.GetAll;
using Application.Features.Sections.Queries.GetById;
using Application.Features.Sections.Queries.GetByCourseId;

namespace API.Endpoints
{
    public class SectionsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/sections")
                .WithTags("Sections");

            group.MapGet("/", GetSections)
                .WithName(nameof(GetSections))
                .Produces<PaginatedResult<SectionResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/course/{courseId:guid}", GetSectionsByCourseId)
                .WithName(nameof(GetSectionsByCourseId))
                .Produces<PaginatedResult<SectionResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetSectionById)
                .WithName(nameof(GetSectionById))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateSection)
                .WithName(nameof(CreateSection))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString()));
            
            group.MapPut("/{id:guid}", UpdateSection)
                .WithName(nameof(UpdateSection))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString()));
            
            group.MapDelete("/{id:guid}", DeleteSection)
                .WithName(nameof(DeleteSection))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString()));
        }

        public static async Task<Results<Ok<PaginatedResult<SectionResponseDto>>, BadRequest>> GetSections(
            [AsParameters] QueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetSectionsQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<PaginatedResult<SectionResponseDto>>, BadRequest>> GetSectionsByCourseId(
            Guid courseId,
            [AsParameters] QueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetSectionsByCourseIdQuery(courseId, queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<SectionResponseDto>, NotFound>> GetSectionById(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetSectionByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<CreatedAtRoute<SectionResponseDto>, BadRequest>> CreateSection(
            SectionCreateDto request, IMediator mediator)
        {
            var id = await mediator.Send(new CreateSectionCommand(request));
            var section = await mediator.Send(new GetSectionByIdQuery(id));
            
            return TypedResults.CreatedAtRoute(section!, nameof(GetSectionById), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> UpdateSection(
            Guid id, SectionUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateSectionCommand(id, request));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteSection(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteSectionCommand(id));
            return TypedResults.NoContent();
        }
    }
}
