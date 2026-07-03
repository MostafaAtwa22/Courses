using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.ExternalLogin.Facebook;
using Application.Features.Authentication.Commands.ExternalLogin.Google;
using Application.Features.Authentication.Commands.Login;
using Application.Features.Authentication.Commands.Register;
using Application.Features.Authentication.Commands.RefreshToken;
using Application.Features.Authentication.Commands.RevokeToken;

namespace API.Endpoints;

public class AuthenticationEndpoints : ICarterModule
{
    private const string RefreshTokenCookieName = "refreshToken";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/authentication")
            .WithTags("Authentication");
        
        group.MapPost("/register", Register)
            .WithName(nameof(Register))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimiterPolicies.Auth)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/login", Login)
            .WithName(nameof(Login))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimiterPolicies.Auth)
            .Produces<AuthResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);
        
        group.MapPost("/google-login", GoogleLogin)
            .WithName(nameof(GoogleLogin))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimiterPolicies.Auth)
            .Produces<AuthResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/facebook-login", FacebookLogin)
            .WithName(nameof(FacebookLogin))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimiterPolicies.Auth)
            .Produces<AuthResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/github-login", GithubLogin)
            .WithName(nameof(GithubLogin))
            .AllowAnonymous()
            .RequireRateLimiting(RateLimiterPolicies.Auth)
            .Produces<AuthResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/refresh-token", RefreshToken)
            .WithName(nameof(RefreshToken))
            .AllowAnonymous()
            .Produces<AuthResponseDto>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/revoke-token", RevokeToken)
            .WithName(nameof(RevokeToken))
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<Results<Created, BadRequest>> Register(
        RegisterDto request, IMediator mediator)
    {
        await mediator.Send(new CreateRegisterCommand(request));
        return TypedResults.Created();
    }

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> Login(
        LoginDto request, IMediator mediator, HttpContext context)
    {
        var result = await mediator.Send(new CreateLoginCommand(request));
        return ProcessTokenResponse(context, result);
    }

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> GoogleLogin(
        GoogleLoginDto request, IMediator mediator, HttpContext context)
    {
        var result = await mediator.Send(new CreateGoogleLoginCommand(request));
        return ProcessTokenResponse(context, result);
    }

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> FacebookLogin(
        FacebookLoginDto request, IMediator mediator, HttpContext context)
    {
        var result = await mediator.Send(new CreateFacebookLoginCommand(request));
        return ProcessTokenResponse(context, result);
    }

    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult, BadRequest>> GithubLogin(
        GithubLoginDto request, IMediator mediator, HttpContext context)
    {
        var result = await mediator.Send(new Application.Features.Authentication.Commands.ExternalLogin.Github.CreateGithubLoginCommand(request));
        return ProcessTokenResponse(context, result);
    }
    
    private static async Task<Results<Ok<AuthResponseDto>, UnauthorizedHttpResult>> RefreshToken(
        HttpContext context, IMediator mediator)
    {
        if (!context.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return TypedResults.Unauthorized();

        var result = await mediator.Send(new CreateRefreshTokenCommand(refreshToken));
        return ProcessTokenResponse(context, result);
    }

    private static async Task<Results<Ok, UnauthorizedHttpResult>> RevokeToken(
        HttpContext context, IMediator mediator)
    {
        if (!context.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            return TypedResults.Unauthorized();

        await mediator.Send(new CreateRevokeTokenCommand(refreshToken));
        
        context.Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        });

        return TypedResults.Ok();
    }

    private static Ok<AuthResponseDto> ProcessTokenResponse(HttpContext context, AuthResponseDto result)
    {
        SetRefreshTokenCookie(context, result.RefreshToken!, result.RefreshTokenExpiration);
        result.RefreshToken = null; 
        return TypedResults.Ok(result);
    }

    private static void SetRefreshTokenCookie(HttpContext context, string token, DateTime expires)
    {
        context.Response.Cookies.Append(RefreshTokenCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Expires = expires,
            Secure = true,
            SameSite = SameSiteMode.None
        });
    }
}