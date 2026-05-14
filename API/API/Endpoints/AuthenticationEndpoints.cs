using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.Login;
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

            group.MapPost("/login", Login)
                .WithName(nameof(Login))
                .Produces<AuthResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status400BadRequest);
        }

        public static async Task<Results<Created, BadRequest>> Register(
            RegisterDto request, IMediator mediator)
        {
            await mediator.Send(new CreateRegisterCommand(request));
            return TypedResults.Created();
        }

        public static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> Login(
            LoginDto request, IMediator mediator)
        {
            var result = await mediator.Send(new CreateLoginCommand(request));
            return TypedResults.Ok(result);
        }
    }
}