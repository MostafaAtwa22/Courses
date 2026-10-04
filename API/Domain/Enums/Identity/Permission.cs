namespace Domain.Enums.Identity
{
    public enum Permission
    {
        ReadRole       = 1,
        UpdateRole     = 2,

        ReadUser       = 3,
        LockingUser    = 4,

        ReadAdmin      = 5,
        CreateAdmin    = 6,
        DeleteAdmin    = 7,

        ReadStudent    = 8,

        ReadInstructor = 9,

        ReadCategory   = 10,
        CreateCategory = 11,
        UpdateCategory = 12,
        DeleteCategory = 13,
    }
}