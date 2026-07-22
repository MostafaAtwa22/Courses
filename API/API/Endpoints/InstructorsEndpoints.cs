using Application.DTOs.Instructor;
using Application.Features.Instructors.Commands.Create;
using Application.Features.Instructors.Commands.Update;
using Application.Features.Instructors.Queries.GetPublicById;
using Application.Features.Instructors.Queries.GetPrivateById;

namespace API.Endpoints
{
    public class InstructorsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/instructors")
                .WithTags("Instructors");

            group.MapGet("/public/{id:guid}", GetPublicInstructor)
                .WithName(nameof(GetPublicInstructor))
                .Produces<InstructorPublicResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/private/{id:guid}", GetPrivateInstructor)
                .WithName(nameof(GetPrivateInstructor))
                .Produces<InstructorPrivateResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Instructor.ToString(),
                        Role.Admin.ToString(),
                        Role.SuperAdmin.ToString()));

            group.MapPost("/", CreateInstructor)
                .WithName(nameof(CreateInstructor))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .DisableAntiforgery()
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Admin.ToString()));

            group.MapPut("/{id:guid}", UpdateInstructor)
                .WithName(nameof(UpdateInstructor))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .DisableAntiforgery()
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Instructor.ToString()));
        }

        public static async Task<Results<Ok<InstructorPublicResponseDto>, NotFound>> GetPublicInstructor(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetPublicInstructorByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<Ok<InstructorPrivateResponseDto>, NotFound>> GetPrivateInstructor(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetPrivateInstructorByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<CreatedAtRoute<InstructorPublicResponseDto>, BadRequest>> CreateInstructor(
            [FromForm] InstructorCreateDto request, IMediator mediator)
        {
            var id = await mediator.Send(new CreateInstructorCommand(request));
            var instructor = await mediator.Send(new GetPublicInstructorByIdQuery(id));
            return TypedResults.CreatedAtRoute(instructor!, nameof(GetPublicInstructor), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> UpdateInstructor(
            Guid id, [FromForm] InstructorUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateInstructorCommand(id, request));
            return TypedResults.NoContent();
        }
    }
}
