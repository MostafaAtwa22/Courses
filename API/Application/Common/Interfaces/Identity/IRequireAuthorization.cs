namespace Application.Common.Interfaces.Identity
{
    public interface IRequireAuthorization
    {
        string[] RequiredRoles { get; }
        bool RequireOwnership { get; }
        Guid ResourceId { get; }
    }
}
