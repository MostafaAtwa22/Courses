namespace Application.Features.Authorization.Commands.UpdateUserRoles;

public sealed record UpdateUserRolesCommand(string UserId, UserRolesManageDto Dto) : IRequest;