using Application.Features.Security.Commands.Disable2FA;
using Application.Features.Security.Commands.Enable2FA;
using Application.Features.Security.Commands.Generate2FA;
using Application.Features.Security.Commands.VerifyTwoFactor;
using Application.Features.Security.Commands.ConfirmEmail;
using Application.DTOs.Security;
using Application.DTOs.Authentication;

namespace API.Endpoints
{
    public class SecurityEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/security")
                .WithTags("Security")
                .RequireAuthorization();

            group.MapPost("/2fa/generate", Generate2FAToken)
                .WithName(nameof(Generate2FAToken))
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/2fa/enable", Enable2FA)
                .WithName(nameof(Enable2FA))
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/2fa/disable", Disable2FA)
                .WithName(nameof(Disable2FA))
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/2fa/verify", VerifyTwoFactor)
                .WithName(nameof(VerifyTwoFactor))
                .AllowAnonymous()
                .Produces<AuthResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapPost("/confirm-email", ConfirmEmail)
                .WithName(nameof(ConfirmEmail))
                .AllowAnonymous()
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces<AuthResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status400BadRequest);
        }

        public static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> ConfirmEmail(
            ConfirmEmailDto request, IMediator mediator)
        {
            var result = await mediator.Send(new ConfirmEmailCommand(request));
            return TypedResults.Ok(result);
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult>> Generate2FAToken(
            IMediator mediator)
        {
            await mediator.Send(new Generate2FATokenCommand());
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult>> Enable2FA(
            [FromBody] string code,
            IMediator mediator)
        {
            await mediator.Send(new Enable2FACommand(code));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult>> Disable2FA(
            [FromBody] Disable2FADto dto,
            IMediator mediator)
        {
            await mediator.Send(new Disable2FACommand(dto));
            return TypedResults.NoContent();
        }

        public static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> VerifyTwoFactor(
            VerifyTwoFactorDto request, IMediator mediator)
        {
            var result = await mediator.Send(new VerifyTwoFactorCommand(request));
            return TypedResults.Ok(result);
        }
    }
}
