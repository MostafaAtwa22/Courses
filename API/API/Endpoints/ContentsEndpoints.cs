using Application.DTOs.Content;
using Application.Features.Contents.Commands.Create;
using Application.Features.Contents.Commands.Delete;
using Application.Features.Contents.Commands.Update;
using Application.Features.Contents.Queries.GetById;
using Application.Features.Contents.Queries.GetByCourse;
using Application.Features.Contents.Queries.GetBySection;

namespace API.Endpoints
{
    public class ContentsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/contents")
                .WithTags("Contents");

            group.MapGet("/section/{sectionId:guid}", GetBySection)
                .WithName(nameof(GetBySection))
                .Produces<PaginatedResult<ContentResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/course/{courseId:guid}", GetByCourse)
                .WithName(nameof(GetByCourse))
                .Produces<PaginatedResult<ContentResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetContentById)
                .WithName(nameof(GetContentById))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateContent)
                .WithName(nameof(CreateContent))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .DisableAntiforgery();

            group.MapPut("/{id:guid}", UpdateContent)
                .WithName(nameof(UpdateContent))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .DisableAntiforgery();

            group.MapDelete("/{id:guid}", DeleteContent)
                .WithName(nameof(DeleteContent))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        public static async Task<Results<Ok<PaginatedResult<ContentResponseDto>>, BadRequest>> GetBySection(
            Guid sectionId,
            [AsParameters] QueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetContentBySectionQuery(sectionId, queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<PaginatedResult<ContentResponseDto>>, BadRequest>> GetByCourse(
            Guid courseId,
            [AsParameters] QueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetContentByCourseQuery(courseId, queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<ContentResponseDto>, NotFound>> GetContentById(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetContentByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<CreatedAtRoute<ContentResponseDto>, BadRequest>> CreateContent(
            [FromForm] ContentCreateDto request, IMediator mediator)
        {
            var id = await mediator.Send(new CreateContentCommand(request));
            var content = await mediator.Send(new GetContentByIdQuery(id));

            return TypedResults.CreatedAtRoute(content!, nameof(GetContentById), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> UpdateContent(
            Guid id, [FromForm] ContentUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateContentCommand(id, request));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteContent(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteContentCommand(id));
            return TypedResults.NoContent();
        }
    }
}
