using Domain.Enums;

namespace Application.Common.Extensions;

public static class GenderMappingExtensions
{
    public static Gender MapGender(string? gender)
    {
        if (string.IsNullOrEmpty(gender))
            return default;

        return gender.ToLowerInvariant() switch
        {
            "male" => Gender.Male,
            "female" => Gender.Female,
            _ => default
        };
    }
}
