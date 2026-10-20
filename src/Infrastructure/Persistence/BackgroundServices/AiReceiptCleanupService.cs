using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.BackgroundServices;

internal sealed class AiReceiptCleanupService(IServiceScopeFactory scopes, ILogger<AiReceiptCleanupService> logger,
    TimeProvider clock) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5), clock);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using var scope = scopes.CreateAsyncScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var now = clock.GetUtcNow();
                    await db.Database.ExecuteSqlInterpolatedAsync($"""
                        DELETE FROM ai_connection_creations WHERE (user_id, request_id) IN (
                            SELECT user_id, request_id FROM ai_connection_creations WHERE expires_at <= {now}
                            ORDER BY expires_at LIMIT 1000 FOR UPDATE SKIP LOCKED)
                        """, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception) { logger.LogWarning("AI receipt cleanup failed; it will retry on the next scheduled run."); }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
    }
}
