using Domain.Enums;

namespace Application.Common.Models;

public class UserQueryParams : QueryParams
{
    public Gender? Gender { get; set; }
    public string? Role { get; set; }
}
