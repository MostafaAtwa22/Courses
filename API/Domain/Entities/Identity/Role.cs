using Domain.Shared;

namespace Domain.Entities.Identity
{
    public sealed class Role(int id, string name) :
        Enumeration<Role>(id, name)
    {
        public static readonly Role SuperAdmin = new(1, nameof(SuperAdmin));
        public static readonly Role Admin      = new(2, nameof(Admin));

        public ICollection<Permission> Permissions { get; private set; } = [];
        public ICollection<ApplicationUser> Users { get; private set; } = [];
    }
}