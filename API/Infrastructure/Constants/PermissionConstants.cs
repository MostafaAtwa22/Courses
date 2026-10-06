using Infrastructure.Enums;

namespace Infrastructure.Constants
{
    public static class PermissionConstants
    {
        public const string PermissionClaimValuePrefix = "Permission";

        public static List<string> GeneratePermissionList(Module module)
            =>
            [
                $"{PermissionClaimValuePrefix}:{module}:{CRUD.Create}",
                $"{PermissionClaimValuePrefix}:{module}:{CRUD.Update}",
                $"{PermissionClaimValuePrefix}:{module}:{CRUD.Read}",
                $"{PermissionClaimValuePrefix}:{module}:{CRUD.Delete}",
            ];
        
        public static List<string> GenerateAllPermissions()
            => [..Enum.GetValues<Module>()
                .SelectMany(GeneratePermissionList)];
    }
}