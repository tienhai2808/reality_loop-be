using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Handwriting.Tests;

internal sealed class SqlCommandCounter(IHttpContextAccessor http) : DbCommandInterceptor
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _commands = new();
    public int Count(string key) => _commands.TryGetValue(key, out var commands) ? commands.Count : 0;
    public string Sql(string key) => _commands.TryGetValue(key, out var commands) ? string.Join("\n", commands) : "";
    private void Record(DbCommand command)
    {
        var key = http.HttpContext?.Request.Headers["X-Test-Query"].ToString();
        if (!string.IsNullOrEmpty(key)) _commands.GetOrAdd(key, _ => new()).Enqueue(command.CommandText);
    }
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken ct = default)
    { Record(command); return ValueTask.FromResult(result); }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    { Record(command); return ValueTask.FromResult(result); }
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command,
        CommandEventData eventData, InterceptionResult<object> result, CancellationToken ct = default)
    { Record(command); return ValueTask.FromResult(result); }
}
