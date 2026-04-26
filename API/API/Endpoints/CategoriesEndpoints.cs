using Application.Common.Models;
using Application.DTOs.Category;
using Application.Features.Categories.Commands.Create;
using Application.Features.Categories.Commands.Delete;
using Application.Features.Categories.Commands.Update;
using Application.Features.Categories.Queries.GetAll;
using Application.Features.Categories.Queries.GetById;

namespace API.Endpoints
{
    public class CategoriesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/categories")
                .WithTags("Categories");

            group.MapGet("/", GetCategories)
                .WithName(nameof(GetCategories))
                .Produces<PaginatedResult<CategoryResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetCategoryById)
                .WithName(nameof(GetCategoryById))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateCategory)
                .WithName(nameof(CreateCategory))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            group.MapPut("/{id:guid}", UpdateCategory)
                .WithName(nameof(UpdateCategory))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            
            group.MapDelete("/{id:guid}", DeleteCategory)
                .WithName(nameof(DeleteCategory))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        public static async Task<Results<Ok<PaginatedResult<CategoryResponseDto>>, BadRequest>> GetCategories(
            [AsParameters] QueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetCategoriesQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<CategoryResponseDto>, NotFound>> GetCategoryById(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetCategoryByIdQuery(id));

            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<CreatedAtRoute<CategoryResponseDto>, BadRequest>> CreateCategory(
            CategoryCreateDto request, IMediator mediator)
        {
            var id = await mediator.Send(new CreateCategoryCommand(request));
            var category = await mediator.Send(new GetCategoryByIdQuery(id));
            
            return TypedResults.CreatedAtRoute(category!, nameof(GetCategoryById), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> UpdateCategory(
            Guid id, CategoryUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateCategoryCommand(id, request));

            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteCategory(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteCategoryCommand(id));

            return TypedResults.NoContent();
        }
    }
}