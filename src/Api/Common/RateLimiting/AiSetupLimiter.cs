using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;

namespace Api.Common.RateLimiting;

internal sealed class AiSetupLimiter : IDisposable
{
    public PartitionedRateLimiter<HttpContext> Requests { get; }
    public PartitionedRateLimiter<HttpContext> Concurrency { get; }
    public AiSetupLimiter(IOptions<AiSetupLimitsOptions> options)
    {
        var settings = options.Value;
        Requests = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(context.User.FindFirst("sub")!.Value, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = settings.UserPermitLimit, Window = TimeSpan.FromSeconds(settings.WindowSeconds),
                QueueLimit = 0, AutoReplenishment = true
            }));
        Concurrency = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetConcurrencyLimiter(context.User.FindFirst("sub")!.Value, _ => new ConcurrencyLimiterOptions
            { PermitLimit = settings.ConcurrentRequestsPerUser, QueueLimit = 0 }));
    }
    public void Dispose() { Requests.Dispose(); Concurrency.Dispose(); }
}
