namespace Application.Common.Interfaces.Cache
{
    public static class CacheKeys
    {
        // Courses
        public static string Courses() => "courses";
        public static string Course(Guid id) => $"course:{id}";
        public static string CoursesByStudent(string userId, int pageNumber, int pageSize) =>
            $"courses:student:{userId}:{pageNumber}:{pageSize}";
        public static string CoursesByInstructor(Guid instructorId, int pageNumber, int pageSize) =>
            $"courses:instructor:{instructorId}:{pageNumber}:{pageSize}";
        public static string CoursesByInstructorPublic(Guid instructorId, int pageNumber, int pageSize) =>
            $"courses:instructor:public:{instructorId}:{pageNumber}:{pageSize}";
        public static string CourseSuggestions(string term) => $"courses:suggestions:{term}";
        public static string TopPerformingCourses(int limit) => $"courses:top:{limit}";

        // Categories
        public static string Categories() => "categories";
        public static string Category(Guid id) => $"category:{id}";
        public static string Categories(string searchTerm, int pageNumber, int pageSize) =>
            $"categories:{searchTerm}:{pageNumber}:{pageSize}";

        // Instructors
        public static string Instructors() => "instructors";
        public static string Instructor(Guid id) => $"instructor:{id}";
        public static string InstructorPublic(Guid id) => $"instructor:public:{id}";
        public static string InstructorByCourse(Guid courseId) => $"instructor:course:{courseId}";
        public static string InstructorByUser(string userId) => $"instructor:user:{userId}";
        public static string Instructors(string searchTerm, int pageNumber, int pageSize) =>
            $"instructors:{searchTerm}:{pageNumber}:{pageSize}";

        // Contents
        public static string Contents() => "contents";
        public static string Content(Guid id) => $"content:{id}";
        public static string ContentsBySection(Guid sectionId) => $"contents:section:{sectionId}";
        public static string ContentsByCourse(Guid courseId, int pageNumber, int pageSize) =>
            $"contents:course:{courseId}:{pageNumber}:{pageSize}";

        // Sections
        public static string Sections() => "sections";
        public static string Section(Guid id) => $"section:{id}";
        public static string SectionsByCourse(Guid courseId, int pageNumber, int pageSize) =>
            $"sections:course:{courseId}:{pageNumber}:{pageSize}";
        public static string Sections(string searchTerm, int pageNumber, int pageSize) =>
            $"sections:{searchTerm}:{pageNumber}:{pageSize}";

        // Reviews
        public static string Reviews() => "reviews";
        public static string Review(Guid id) => $"review:{id}";
        public static string ReviewsByCourse(Guid courseId, int pageNumber, int pageSize) =>
            $"reviews:course:{courseId}:{pageNumber}:{pageSize}";
        public static string ReviewByUserCourse(string userId, Guid courseId) =>
            $"review:user:{userId}:course:{courseId}";

        // Students
        public static string Students() => "students";
        public static string Student(Guid id) => $"student:{id}";
        public static string StudentByUser(string userId) => $"student:user:{userId}";
        public static string Students(string searchTerm, int pageNumber, int pageSize) =>
            $"students:{searchTerm}:{pageNumber}:{pageSize}";

        // Admins
        public static string Admins() => "admins";
        public static string Admin(Guid id) => $"admin:{id}";
        public static string AdminByUser(string userId) => $"admin:user:{userId}";
        public static string Admins(string searchTerm, string? role, int pageNumber, int pageSize) =>
            $"admins:{searchTerm}:{role ?? string.Empty}:{pageNumber}:{pageSize}";

        // Users (Account)
        public static string Users() => "users";
        public static string User(Guid id) => $"user:{id}";
        public static string Users(string searchTerm, string? gender, string? role, int pageNumber, int pageSize) =>
            $"users:{searchTerm}:{gender}:{role}:{pageNumber}:{pageSize}";

        // Roles (Authorization)
        public static string Roles() => "roles";
        public static string AllRoles() => "roles:all";
        public static string RoleByUser(string userId) => $"roles:user:{userId}";

        // Progress
        public static string Progress() => "progress";
        public static string ProgressByUserCourse(string userId, Guid courseId) =>
            $"progress:user:{userId}:course:{courseId}";
        public static string ProgressByUser(string userId) => $"progress:user:{userId}";

        // Admin Dashboard
        public static string AdminDashboard() => "admin-dashboard";
        public static string EnrollmentStatistics() => $"admin-dashboard:enrollments";
        public static string RoleStatistics() => $"admin-dashboard:roles";

        // Instructor Dashboard
        public static string InstructorDashboard() => "instructor-dashboard";
        public static string InstructorEnrollmentStatistics(Guid instructorId) => $"instructor-dashboard:enrollments:{instructorId}";

        // Discounts
        public static string Discounts() => "discounts";
        public static string DiscountsByCourse(Guid courseId) => $"discounts:course:{courseId}";

        // Permissions
        public static string Permissions() => "permissions";
        public static string PermissionsByRole(string roleId) => $"permissions:role:{roleId}";
        public static string PermissionsByUser(string userId) => $"permissions:user:{userId}";

        // Lockout
        public static string UserLockout(string userId) => $"lockout:{userId}";
    }
}