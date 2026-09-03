namespace Application.Features.Authorization.Queries.GetAll
{
    public sealed record GetRolesQuery() : IRequest<IReadOnlyCollection<RolesResponseDto>>;
}

