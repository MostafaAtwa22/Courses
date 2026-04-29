using System.Threading.RateLimiting;
using Microsoft.Extensions.Primitives;
using API.RateLimiting.Options;

namespace API.RateLimiting
{
    public static class RateLimiterExtension
    {
        public static IServiceCollection AddRateLimiter(
            this IServiceCollection services, IConfiguration configuration)
        {
            var authOpts = configuration.GetSection("RateLimiter:Auth").Get<AuthPolicyOptions>() ?? new AuthPolicyOptions();
            var forgotOpts = configuration.GetSection("RateLimiter:ForgotPassword").Get<ForgotPasswordPolicyOptions>() ?? new ForgotPasswordPolicyOptions();
            var searchOpts = configuration.GetSection("RateLimiter:Search").Get<SearchPolicyOptions>() ?? new SearchPolicyOptions();
            var purchaseOpts = configuration.GetSection("RateLimiter:Purchase").Get<PurchasePolicyOptions>() ?? new PurchasePolicyOptions();
            var mediaOpts = configuration.GetSection("RateLimiter:Media").Get<MediaPolicyOptions>() ?? new MediaPolicyOptions();
            var instructorOpts = configuration.GetSection("RateLimiter:InstructorWrite").Get<InstructorWritePolicyOptions>() ?? new InstructorWritePolicyOptions();
            var reviewOpts = configuration.GetSection("RateLimiter:Review").Get<ReviewPolicyOptions>() ?? new ReviewPolicyOptions();
            var globalOpts = configuration.GetSection("RateLimiter:Global").Get<GlobalPolicyOptions>() ?? new GlobalPolicyOptions();

            services.Configure<AuthPolicyOptions>(configuration.GetSection("RateLimiter:Auth"));
            services.Configure<ForgotPasswordPolicyOptions>(configuration.GetSection("RateLimiter:ForgotPassword"));
            services.Configure<SearchPolicyOptions>(configuration.GetSection("RateLimiter:Search"));
            services.Configure<PurchasePolicyOptions>(configuration.GetSection("RateLimiter:Purchase"));
            services.Configure<MediaPolicyOptions>(configuration.GetSection("RateLimiter:Media"));
            services.Configure<InstructorWritePolicyOptions>(configuration.GetSection("RateLimiter:InstructorWrite"));
            services.Configure<ReviewPolicyOptions>(configuration.GetSection("RateLimiter:Review"));
            services.Configure<GlobalPolicyOptions>(configuration.GetSection("RateLimiter:Global"));

            services.AddRateLimiter(options =>
            {
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    if (context.Lease.TryGetMetadata(
                            MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString();
                    }

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        title = "Too Many Requests",
                        status = 429,
                        detail = "You have exceeded the allowed number of requests. Please wait and try again.",
                        retryAfterSeconds = context.Lease.TryGetMetadata(
                            MetadataName.RetryAfter, out var r) ? (int)r.TotalSeconds : 0
                    }, cancellationToken: cancellationToken);
                };
                options.AddPolicy(RateLimiterPolicies.Auth, httpContext =>
                {
                    var ipAddress = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth:{ipAddress}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = authOpts.PermitLimit,
                            Window = TimeSpan.FromMinutes(authOpts.WindowMinutes),

                            QueueLimit = authOpts.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,

                            AutoReplenishment = true
                        });
                });

                options.AddPolicy(RateLimiterPolicies.ForgotPassword, httpContext =>
                {
                    var ipAddress = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"forgot:{ipAddress}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = forgotOpts.PermitLimit,
                            Window = TimeSpan.FromHours(forgotOpts.WindowHours),
                            QueueLimit = forgotOpts.QueueLimit,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy(RateLimiterPolicies.Search, httpContext =>
                {
                    var key = GetUserOrIpKey(httpContext, prefix: "search");

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: key,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = searchOpts.PermitLimit,
                            Window = TimeSpan.FromMinutes(searchOpts.WindowMinutes),
                            SegmentsPerWindow = searchOpts.SegmentsPerWindow,  
                            QueueLimit = searchOpts.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy(RateLimiterPolicies.Purchase, httpContext =>
                {
                    var userId = httpContext.User?.FindFirst("sub")?.Value
                                ?? GetClientIp(httpContext);

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: $"purchase:{userId}",
                        factory: _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = purchaseOpts.TokenLimit,
                            TokensPerPeriod = purchaseOpts.TokensPerPeriod,
                            ReplenishmentPeriod = TimeSpan.FromMinutes(purchaseOpts.ReplenishmentMinutes),
                            QueueLimit = purchaseOpts.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy(RateLimiterPolicies.Media, httpContext =>
                {
                    var userId = httpContext.User?.FindFirst("sub")?.Value
                                ?? GetClientIp(httpContext);

                    return RateLimitPartition.GetConcurrencyLimiter(
                        partitionKey: $"media:{userId}",
                        factory: _ => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = mediaOpts.PermitLimit,
                            QueueLimit = mediaOpts.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });

                options.AddPolicy(RateLimiterPolicies.InstructorWrite, httpContext =>
                {
                    var userId = httpContext.User?.FindFirst("sub")?.Value
                                ?? GetClientIp(httpContext);

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: $"instructor:{userId}",
                        factory: _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = instructorOpts.TokenLimit,
                            TokensPerPeriod = instructorOpts.TokensPerPeriod,
                            ReplenishmentPeriod = TimeSpan.FromHours(instructorOpts.ReplenishmentHours),
                            QueueLimit = instructorOpts.QueueLimit,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy(RateLimiterPolicies.Review, httpContext =>
                {
                    var userId = httpContext.User?.FindFirst("sub")?.Value
                                ?? GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"review:{userId}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = reviewOpts.PermitLimit,
                            Window = TimeSpan.FromHours(reviewOpts.WindowHours),
                            QueueLimit = reviewOpts.QueueLimit,
                            AutoReplenishment = true
                        });
                });

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                    {
                        var ipAddress = GetClientIp(httpContext);

                        return RateLimitPartition.GetSlidingWindowLimiter(
                            partitionKey: $"global:{ipAddress}",
                            factory: _ => new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = globalOpts.PermitLimit,
                                Window = TimeSpan.FromMinutes(globalOpts.WindowMinutes),
                                SegmentsPerWindow = globalOpts.SegmentsPerWindow,
                                QueueLimit = globalOpts.QueueLimit,
                                AutoReplenishment = true
                            });
                    });
            });

            return services;
        }

        private static string GetClientIp(HttpContext ctx)
        {
            if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwarded)
                && !string.IsNullOrWhiteSpace(forwarded))
            {
                var firstIp = forwarded.ToString().Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(firstIp))
                    return firstIp;
            }

            return ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
        private static string GetUserOrIpKey(HttpContext ctx, string prefix)
        {
            var userId = ctx.User?.FindFirst("sub")?.Value;

            return string.IsNullOrWhiteSpace(userId)
                ? $"{prefix}:ip:{GetClientIp(ctx)}"
                : $"{prefix}:user:{userId}";
        }
    }
}