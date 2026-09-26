using Domain.Enums.Identity;

namespace Application.Common.Models;

public class AdminQueryParams : QueryParams
{
    public Role? Role { get; set; }
    public Gender? Gender { get; set; }
}