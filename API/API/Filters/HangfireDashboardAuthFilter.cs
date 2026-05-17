using Hangfire.Dashboard;

namespace API.Filters
{
    public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            
            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.IsInRole("SuperAdmin");
        }
    }
}
