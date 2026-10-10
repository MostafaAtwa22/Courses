namespace Application.Common.Interfaces.Identity
{
    public interface IRequireAuthorization
    {
        string[] RequiredRoles { get; }
        string[] RequiredPermissions { get; }
        bool RequireOwnership { get; }
        Guid ResourceId { get; }
    }
}
