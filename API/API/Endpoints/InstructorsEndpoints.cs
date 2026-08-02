using Application.DTOs.Instructor;
using Application.Features.Instructors.Commands.ChangeStatus;
using Application.Features.Instructors.Commands.Create;
using Application.Features.Instructors.Commands.Update;
using Application.Features.Instructors.Queries.GetAll;
using Application.Features.Instructors.Queries.GetPublicById;
using Application.Features.Instructors.Queries.GetPublicByCourseId;
using Application.Features.Instructors.Queries.GetPrivateById;
using Application.Features.Instructors.Commands.Delete;


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

            group.MapGet("/public/by-course/{courseId:guid}", GetPublicInstructorByCourseId)
                .WithName(nameof(GetPublicInstructorByCourseId))
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
                    policy.RequireRole(
                        Role.Instructor.ToString()));

            group.MapPut("/{id:guid}", UpdateInstructor)
                .WithName(nameof(UpdateInstructor))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .DisableAntiforgery()
                .RequireAuthorization(policy =>
                    policy.RequireRole(Role.Instructor.ToString()));

            group.MapGet("/admin/all", GetAllInstructors)
                .WithName(nameof(GetAllInstructors))
                .Produces<PaginatedResult<InstructorPrivateResponseDto>>(StatusCodes.Status200OK)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Admin.ToString(),
                        Role.SuperAdmin.ToString()));

            group.MapPut("/admin/{id:guid}/status", ChangeInstructorStatus)
                .WithName(nameof(ChangeInstructorStatus))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Admin.ToString(),
                        Role.SuperAdmin.ToString()));

            group.MapDelete("/{id:guid}", DeleteInstructor)
                .WithName(nameof(DeleteInstructor))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .RequireAuthorization(policy =>
                    policy.RequireRole(
                        Role.Admin.ToString(),
                        Role.SuperAdmin.ToString()));
        }

        public static async Task<Results<Ok<InstructorPublicResponseDto>, NotFound>> GetPublicInstructor(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetPublicInstructorByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<Ok<InstructorPublicResponseDto>, NotFound>> GetPublicInstructorByCourseId(
            Guid courseId, IMediator mediator)
        {
            var result = await mediator.Send(new GetPublicInstructorByCourseIdQuery(courseId));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<Ok<InstructorPrivateResponseDto>, NotFound>> GetPrivateInstructor(
            Guid id, IMediator mediator)
        {
            var result = await mediator.Send(new GetPrivateInstructorByIdQuery(id));
            return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
        }

        public static async Task<Results<CreatedAtRoute<InstructorPrivateResponseDto>, BadRequest>> CreateInstructor(
            [FromForm] InstructorCreateDto request, IMediator mediator)
        {
            var id = await mediator.Send(new CreateInstructorCommand(request));
            var instructor = await mediator.Send(new GetPrivateInstructorByIdQuery(id));
            return TypedResults.CreatedAtRoute(instructor!, nameof(GetPrivateInstructor), new { id });
        }

        public static async Task<Results<NoContent, NotFound>> UpdateInstructor(
            Guid id, [FromForm] InstructorUpdateDto request, IMediator mediator)
        {
            await mediator.Send(new UpdateInstructorCommand(id, request));
            return TypedResults.NoContent();
        }

        public static async Task<Ok<PaginatedResult<InstructorPrivateResponseDto>>> GetAllInstructors(
            [AsParameters] InstructorQueryParams queryParams,
            IMediator mediator)
        {
            var result = await mediator.Send(new GetAllInstructorsQuery(queryParams));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<NoContent, NotFound>> ChangeInstructorStatus(
            Guid id,
            [FromBody] ChangeInstructorStatusDto request,
            IMediator mediator)
        {
            await mediator.Send(new ChangeInstructorStatusCommand(id, request.Status));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, NotFound>> DeleteInstructor(
            Guid id, IMediator mediator)
        {
            await mediator.Send(new DeleteInstructorCommand(id));
            return TypedResults.NoContent();
        }
    }
}
