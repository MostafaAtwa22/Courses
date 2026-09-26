using Application.DTOs.Admin;

namespace Application.Features.Admin.Commands.Create;

public sealed record CreateAdminCommand(AdminCreateDto Dto) : IRequest;