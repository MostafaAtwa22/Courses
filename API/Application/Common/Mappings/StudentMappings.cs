using Domain.Entities.Identity;

namespace Application.Common.Mappings
{
    public static class StudentMappings
    {
        public static Student ToStudent(this string userId)
        {
            return new Student
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}