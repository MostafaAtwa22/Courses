using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.Delete;
using Application.Features.Profiles.Commands.Update;
using Application.Features.Profiles.Commands.DeleteImage;
using Application.Features.Profiles.Commands.UpdateImage;
using Application.Features.Profiles.Commands.SetPassword;
using Application.Features.Profiles.Commands.ChangePassword;

namespace API.Endpoints
{
    public class ProfilesEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/profiles")
                .WithTags("Profiles")
                .RequireAuthorization();

            group.MapPut("/update-profile", UpdateProfile)
                .WithName(nameof(UpdateProfile))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/delete-profile", DeleteProfile)
                .WithName(nameof(DeleteProfile))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPatch("/update-image", UpdateProfileImage)
                .WithName(nameof(UpdateProfileImage))
                .DisableAntiforgery()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/delete-image", DeleteProfileImage)
                .WithName(nameof(DeleteProfileImage))
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);
            
            group.MapPost("/change-password", ChangePassword)
                .WithName(nameof(ChangePassword))
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);
            
            group.MapPost("/set-password", SetPassword)
                .WithName(nameof(SetPassword))
                .RequireRateLimiting(RateLimiterPolicies.PasswordManagement)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult, NotFound>> UpdateProfile(
            [FromBody] UpdateProfileDto dto,
            IMediator mediator)
        {
            await mediator.Send(new UpdateProfileCommand(dto));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult, NotFound>> DeleteProfile(
            [FromBody] DeleteProfileDto dto,
            IMediator mediator)
        {
            await mediator.Send(new DeleteProfileCommand(dto));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult, NotFound>> UpdateProfileImage(
            [FromForm] UpdateProfileImageDto dto,
            IMediator mediator)
        {
            await mediator.Send(new UpdateProfileImageCommand(dto));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult, NotFound>> DeleteProfileImage(
            IMediator mediator)
        {
            await mediator.Send(new DeleteProfileImageCommand());
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult, NotFound>> ChangePassword(
            [FromBody] ChangePasswordDto dto,
            IMediator mediator)
        {
            await mediator.Send(new ChangePasswordCommand(dto));
            return TypedResults.NoContent();
        }

        public static async Task<Results<NoContent, BadRequest, UnauthorizedHttpResult, NotFound>> SetPassword(
            [FromBody] SetPasswordDto dto,
            IMediator mediator)
        {
            await mediator.Send(new SetPasswordCommand(dto));
            return TypedResults.NoContent();
        }
    }
}
