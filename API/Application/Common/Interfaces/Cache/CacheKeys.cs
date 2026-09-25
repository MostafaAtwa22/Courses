namespace Application.Common.Interfaces.Cache
{
    internal static class CacheKeys
    {
        // Courses
        internal static string Courses() => "courses";
        internal static string Course(Guid id) => $"course:{id}";
        internal static string CoursesByStudent(string userId, int pageNumber, int pageSize) => 
            $"courses:student:{userId}:{pageNumber}:{pageSize}";
        internal static string CoursesByInstructor(Guid instructorId, int pageNumber, int pageSize) => 
            $"courses:instructor:{instructorId}:{pageNumber}:{pageSize}";
        internal static string CoursesByInstructorPublic(Guid instructorId, int pageNumber, int pageSize) => 
            $"courses:instructor:public:{instructorId}:{pageNumber}:{pageSize}";
        internal static string CourseSuggestions(string term) => $"courses:suggestions:{term}";
        internal static string TopPerformingCourses(int limit) => $"courses:top:{limit}";

        // Categories
        internal static string Categories() => "categories";
        internal static string Category(Guid id) => $"category:{id}";
        internal static string Categories(string searchTerm, int pageNumber, int pageSize) => 
            $"categories:{searchTerm}:{pageNumber}:{pageSize}";

        // Instructors
        internal static string Instructors() => "instructors";
        internal static string Instructor(Guid id) => $"instructor:{id}";
        internal static string InstructorPublic(Guid id) => $"instructor:public:{id}";
        internal static string InstructorByCourse(Guid courseId) => $"instructor:course:{courseId}";
        internal static string InstructorByUser(string userId) => $"instructor:user:{userId}";
        internal static string Instructors(string searchTerm, int pageNumber, int pageSize) => 
            $"instructors:{searchTerm}:{pageNumber}:{pageSize}";

        // Contents
        internal static string Contents() => "contents";
        internal static string Content(Guid id) => $"content:{id}";
        internal static string ContentsBySection(Guid sectionId) => $"contents:section:{sectionId}";
        internal static string ContentsByCourse(Guid courseId, int pageNumber, int pageSize) => 
            $"contents:course:{courseId}:{pageNumber}:{pageSize}";

        // Sections
        internal static string Sections() => "sections";
        internal static string Section(Guid id) => $"section:{id}";
        internal static string SectionsByCourse(Guid courseId, int pageNumber, int pageSize) => 
            $"sections:course:{courseId}:{pageNumber}:{pageSize}";
        internal static string Sections(string searchTerm, int pageNumber, int pageSize) => 
            $"sections:{searchTerm}:{pageNumber}:{pageSize}";

        // Reviews
        internal static string Reviews() => "reviews";
        internal static string Review(Guid id) => $"review:{id}";
        internal static string ReviewsByCourse(Guid courseId, int pageNumber, int pageSize) => 
            $"reviews:course:{courseId}:{pageNumber}:{pageSize}";
        internal static string ReviewByUserCourse(string userId, Guid courseId) => 
            $"review:user:{userId}:course:{courseId}";

        // Students
        internal static string Students() => "students";
        internal static string Student(Guid id) => $"student:{id}";
        internal static string StudentByUser(string userId) => $"student:user:{userId}";
        internal static string Students(string searchTerm, int pageNumber, int pageSize) => 
            $"students:{searchTerm}:{pageNumber}:{pageSize}";

        // Users (Account)
        internal static string Users() => "users";
        internal static string User(Guid id) => $"user:{id}";
        internal static string Users(string searchTerm, string? gender, string? role, int pageNumber, int pageSize) => 
            $"users:{searchTerm}:{gender}:{role}:{pageNumber}:{pageSize}";

        // Roles (Authorization)
        internal static string Roles() => "roles";
        internal static string RoleByUser(string userId) => $"roles:user:{userId}";

        // Progress
        internal static string Progress() => "progress";
        internal static string ProgressByUserCourse(string userId, Guid courseId) => 
            $"progress:user:{userId}:course:{courseId}";
        internal static string ProgressByUser(string userId) => $"progress:user:{userId}";

        // Admin Dashboard
        internal static string AdminDashboard() => "admin-dashboard";
        internal static string EnrollmentStatistics() => $"admin-dashboard:enrollments";
        internal static string RoleStatistics() => $"admin-dashboard:roles";

        // Instructor Dashboard
        internal static string InstructorDashboard() => "instructor-dashboard";
        internal static string InstructorEnrollmentStatistics(Guid instructorId) => $"instructor-dashboard:enrollments:{instructorId}";

        // Discounts
        internal static string Discounts() => "discounts";
        internal static string DiscountsByCourse(Guid courseId) => $"discounts:course:{courseId}";
    }
}