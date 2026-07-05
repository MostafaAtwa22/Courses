using Application.DTOs.Course;
using Application.Features.Discount.Commands.Create;
using Application.Features.Discount.Commands.Update;
using Application.Features.Discount.Commands.Delete;
using Application.Features.Discount.Queries.GetDiscounts;
using Carter;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints
{
    public class DiscountsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/discounts")
                .WithTags("Discounts");

            group.MapPost("/{courseId:guid}", AddDiscount)
                .WithName(nameof(AddDiscount))
                .RequireRateLimiting(RateLimiterPolicies.InstructorWrite)
                .Produces<Guid>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString()));

            group.MapGet("/{courseId:guid}", GetCourseDiscounts)
                .WithName(nameof(GetCourseDiscounts))
                .Produces<IEnumerable<CourseDiscountDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPut("/{id:guid}", UpdateDiscount)
                .WithName(nameof(UpdateDiscount))
                .RequireRateLimiting(RateLimiterPolicies.InstructorWrite)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString()));

            group.MapDelete("/{id:guid}", DeleteDiscount)
                .WithName(nameof(DeleteDiscount))
                .RequireRateLimiting(RateLimiterPolicies.InstructorWrite)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString()));
        }

        public static async Task<Results<CreatedAtRoute<Guid>, BadRequest, NotFound>> AddDiscount(
            Guid courseId, CreateCourseDiscountDto request, IMediator mediator)
        {
            var discountId = await mediator.Send(new CreateDiscountCommand(courseId, request));

            return TypedResults.CreatedAtRoute(discountId, nameof(GetCourseDiscounts), new { courseId });
        }

        public static async Task<Results<Ok<IEnumerable<CourseDiscountDto>>, NotFound>> GetCourseDiscounts(
            Guid courseId, IMediator mediator)
        {
            var result = await mediator.Send(new GetCourseDiscountsQuery(courseId));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<NoContent, NotFound>> UpdateDiscount(
            Guid id, [FromBody] UpdateCourseDiscountDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateDiscountCommand(id, request));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteDiscount(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteDiscountCommand(id));
            return TypedResults.NoContent();
        }
    }
}
