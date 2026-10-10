using Domain.Enums.Identity;

namespace Domain.Constants
{
    public static class PermissionConstants
    {
        public const string PermissionClaimValuePrefix = "Permission";

        private static readonly Lazy<List<string>> _allPermissions = new(() =>
            [.. Enum.GetValues<Module>().SelectMany(GeneratePermissionList)]);

        public static string Build(Module module, CRUD action)
        {
            if (!Enum.IsDefined(typeof(Module), module))
                throw new ArgumentException($"Invalid module: {module}", nameof(module));

            if (!Enum.IsDefined(typeof(CRUD), action))
                throw new ArgumentException($"Invalid CRUD action: {action}", nameof(action));

            return $"{PermissionClaimValuePrefix}:{module}:{action}";
        }

        public static List<string> GeneratePermissionList(Module module)
        {
            if (!Enum.IsDefined(typeof(Module), module))
                throw new ArgumentException($"Invalid module: {module}", nameof(module));

            return
            [
                Build(module, CRUD.Create),
                Build(module, CRUD.Update),
                Build(module, CRUD.Read),
                Build(module, CRUD.Delete),
            ];
        }

        public static List<string> GenerateAllPermissions() => _allPermissions.Value;

        public static List<string> FilterByActions(List<string> permissions, params CRUD[] allowedActions)
        {
            if (permissions == null || permissions.Count == 0)
                return [];

            if (allowedActions == null || allowedActions.Length == 0)
                return permissions;

            var allowedSuffixes = allowedActions.Select(action => $":{action}").ToHashSet();
            return [.. permissions.Where(p => allowedSuffixes.Any(suffix => p.EndsWith(suffix)))];
        }
    }
}