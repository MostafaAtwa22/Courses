namespace Domain.Enums.Identity
{
    public enum Role
    {
        [EnumMember(Value = "SuperAdmin")]
        SuperAdmin,
        [EnumMember(Value = "Admin")]
        Admin,
        [EnumMember(Value = "Instructor")]
        Instructor,
        [EnumMember(Value = "Student")]
        Student
    }
}