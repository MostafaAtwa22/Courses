using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.Register;

namespace API.Endpoints
{
    public class AuthenticationEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/authentication")
                .WithTags("Authentication");
            
            group.MapPost("/register", Register)
                .WithName(nameof(Register))
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
        }

        public static async Task<Results<Created, BadRequest>> Register(
            RegisterDto request, IMediator mediator)
        {
            await mediator.Send(new CreateRegisterCommand(request));
            return TypedResults.Created();
        }
    }
}