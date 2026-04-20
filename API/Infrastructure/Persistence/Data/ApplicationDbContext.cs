using System.Reflection;

namespace Infrastructure.Persistence.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : 
        IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
        
        public required DbSet<Course> Courses { get; set; }
        public required DbSet<Category> Categories { get; set; }
    }
}