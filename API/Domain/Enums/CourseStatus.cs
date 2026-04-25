using System.Runtime.Serialization;

namespace Domain.Enums
{
    public enum CourseStatus
    {
        [EnumMember(Value = "In Progress")]
        InProgress,

        [EnumMember(Value = "Completed")]
        Done
    }
}