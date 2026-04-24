using Application.Common.Models;
using Application.DTOs.Category;
using Application.Features.Categories.Commands.Create;
using Application.Features.Categories.Commands.Delete;
using Application.Features.Categories.Commands.Update;
using Application.Features.Categories.Queries.GetAll;
using Application.Features.Categories.Queries.GetById;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Endpoints
{
    public class CategoriesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/categories")
                .WithTags("Categories");

            group.MapGet("/", GetAll)
                .WithName(nameof(GetAll))
                .Produces<PaginatedResult<CategoryResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetById)
                .WithName(nameof(GetById))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                .WithName(nameof(Create))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            group.MapPut("/{id:guid}", Update)
                .WithName(nameof(Update))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            
            group.MapDelete("/{id:guid}", Delete)
                .WithName(nameof(Delete))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        public static async Task<Results<Ok<PaginatedResult<CategoryResponseDto>>, BadRequest>> GetAll(
            [AsParameters] QueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetCategoriesQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<CategoryResponseDto>, NotFound>> GetById(
            Guid id, IMediator mediator)
        {
            var category = await mediator.Send(new GetCategoryByIdQuery(id));

            if (category is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(category);
        }

        public static async Task<Results<CreatedAtRoute<CategoryResponseDto>, BadRequest>> Create(
            CategoryCreateDto request, IMediator mediator)
        {
            var id = await mediator.Send(new CreateCategoryCommand(request));
            var category = await mediator.Send(new GetCategoryByIdQuery(id));
            
            return TypedResults.CreatedAtRoute(category!, nameof(GetById), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> Update(
            Guid id, CategoryUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateCategoryCommand(id, request));

            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> Delete(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteCategoryCommand(id));

            return TypedResults.NoContent();
        }
    }
}