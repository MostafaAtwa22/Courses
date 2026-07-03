using Application.Common.Models;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Extensions;

public static class ApplicationUserExtensions
{
    public static IQueryable<ApplicationUser> Search(this IQueryable<ApplicationUser> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var term = searchTerm.Trim().ToLower();

        return query.Where(u =>
            u.FirstName.ToLower().Contains(term) ||
            u.LastName.ToLower().Contains(term) ||
            u.Email!.ToLower().Contains(term) ||
            u.UserName!.ToLower().Contains(term));
    }

    public static IQueryable<ApplicationUser> FilterByGender(this IQueryable<ApplicationUser> query, Domain.Enums.Gender? gender)
    {
        if (!gender.HasValue)
            return query;

        return query.Where(u => u.Gender == gender.Value);
    }

    public static async Task<IQueryable<ApplicationUser>> FilterByRoleAsync(this IQueryable<ApplicationUser> query, UserManager<ApplicationUser> userManager, string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return query;

        var userIdsInRole = (await userManager.GetUsersInRoleAsync(role))
            .Select(u => u.Id)
            .ToHashSet();

        return query.Where(u => userIdsInRole.Contains(u.Id));
    }
}
