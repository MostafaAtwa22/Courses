using System.Runtime.Serialization;

namespace Domain.Enums
{
    public enum Gender
    {
        [EnumMember(Value = "Male")]
        Male,

        [EnumMember(Value = "Female")]
        Female
    }
}