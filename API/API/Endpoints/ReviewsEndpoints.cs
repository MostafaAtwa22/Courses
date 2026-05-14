using Application.Features.Reviews.Queries.GetByCourse;
using Application.Features.Reviews.Queries.GetById;
using Application.Features.Reviews.Commands.Create;
using Application.Features.Reviews.Commands.Update;
using Application.Features.Reviews.Commands.Delete;
using Application.DTOs.Review;

namespace API.Endpoints
{
    public class ReviewsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/reviews")
                .WithTags("Reviews");

            group.MapGet("/course/{courseId:guid}", GetReviewsByCourse)
                .WithName(nameof(GetReviewsByCourse))
                .Produces<PaginatedResult<ReviewResponseDto>>(StatusCodes.Status200OK);

            group.MapGet("/{id:guid}", GetReviewById)
                .WithName(nameof(GetReviewById))
                .Produces<ReviewResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/", CreateReview)
                .WithName(nameof(CreateReview))
                .RequireRateLimiting(RateLimiterPolicies.Review)
                .Produces<Guid>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status409Conflict)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Student.ToString()));

            group.MapPut("/{id:guid}", UpdateReview)
                .WithName(nameof(UpdateReview))
                .RequireRateLimiting(RateLimiterPolicies.Review)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Student.ToString()));

            group.MapDelete("/{id:guid}", DeleteReview)
                .WithName(nameof(DeleteReview))
                .RequireRateLimiting(RateLimiterPolicies.Review)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Student.ToString()));
        }

        public static async Task<Ok<PaginatedResult<ReviewResponseDto>>> GetReviewsByCourse(Guid courseId, [AsParameters] QueryParams queryParams, IMediator mediator)
        {
            var result = await mediator.Send(new GetReviewsByCourseQuery(courseId, queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<Ok<ReviewResponseDto>, NotFound>> GetReviewById(Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetReviewByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<CreatedAtRoute<Guid>> CreateReview(ReviewCreateDto dto, IMediator mediator)
        {
            var id = await mediator.Send(new CreateReviewCommand(dto));
            return TypedResults.CreatedAtRoute(id, nameof(GetReviewById), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> UpdateReview(Guid id, ReviewUpdateDto dto, IMediator mediator)
        {
            await mediator.Send(new UpdateReviewCommand(id, dto));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteReview(Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteReviewCommand(id));
            return TypedResults.NoContent();
        }
    }
}
