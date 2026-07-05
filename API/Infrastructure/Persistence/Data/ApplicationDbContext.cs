using System.Reflection;

namespace Infrastructure.Persistence.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : 
        IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        
        public required DbSet<Course> Courses { get; set; }
        public required DbSet<Category> Categories { get; set; }
        public required DbSet<Student> Students { get; set; }
        public required DbSet<Instructor> Instructors { get; set; }
        public required DbSet<Review> Reviews { get; set; }
        public required DbSet<Enrollment> Enrollments { get; set; }
        public required DbSet<Section> Sections { get; set; }
        public required DbSet<Content> Contents { get; set; }
        public required DbSet<CourseDiscount> CourseDiscounts { get; set; }
        public required DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}