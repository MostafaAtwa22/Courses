namespace Application.Common.Interfaces.Identity
{
    public interface IPermissionService
    {
        Task<HashSet<string>> GetPermissionsAsync(string userId);
    }
}