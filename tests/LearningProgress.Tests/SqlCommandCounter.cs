using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;

internal sealed class SqlCommandCounter(IHttpContextAccessor http) : DbCommandInterceptor
{
    private readonly ConcurrentDictionary<string, int> _counts = new();
    public int Count(string key) => _counts.GetValueOrDefault(key);
    private void Record()
    {
        var key = http.HttpContext?.Request.Headers["X-Test-Query"].ToString();
        if (!string.IsNullOrEmpty(key)) _counts.AddOrUpdate(key, 1, (_, count) => count + 1);
    }
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken ct = default)
    { Record(); return ValueTask.FromResult(result); }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    { Record(); return ValueTask.FromResult(result); }
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<object> result, CancellationToken ct = default)
    { Record(); return ValueTask.FromResult(result); }
}
