using Application.DTOs.Admin;
using Application.Features.Admin.Commands.Delete;
using Application.Features.Admin.Commands.Create;
using Application.Features.Admin.Queries.GetAll;
using Application.Features.Admin.Queries.GetById;

namespace API.Endpoints;

public class AdminEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admins")
            .WithTags("Admins")
            .RequireAuthorization(policy =>
                policy.RequireRole(
                    Role.Admin.ToString(),
                    Role.SuperAdmin.ToString()));

        group.MapGet("/", GetAllAdmins)
            .WithName(nameof(GetAllAdmins))
            .Produces<PaginatedResult<AdminResponseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetAdminById)
            .WithName(nameof(GetAdminById))
            .Produces<AdminResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapPost("/", CreateAdmin)
            .WithName(nameof(CreateAdmin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteAdmin)
            .WithName(nameof(DeleteAdmin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<Results<Ok<PaginatedResult<AdminResponseDto>>, BadRequest>> GetAllAdmins(
        [AsParameters] AdminQueryParams queryParams,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetAllAdminsQuery(queryParams));
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<AdminResponseDto>, NotFound>> GetAdminById(
        Guid id, IMediator mediator)
    {
        var result = await mediator.Send(new GetAdminByIdQuery(id));
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, BadRequest>> CreateAdmin(
        AdminCreateDto dto,
        IMediator mediator)
    {
        await mediator.Send(new CreateAdminCommand(dto));
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteAdmin(
        Guid id, IMediator mediator)
    {
        await mediator.Send(new DeleteAdminCommand(id));
        return TypedResults.NoContent();
    }
}