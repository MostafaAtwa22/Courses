using Role =  Domain.Enums.Identity.Role;

namespace Application.Common.Models;

public class AdminQueryParams : QueryParams
{
    public Role? Role { get; set; }
    public Gender? Gender { get; set; }
}