using System.Collections.Concurrent;
using System.Data.Common;
using Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;

internal sealed class QueryCounter(IHttpContextAccessor accessor) : DbCommandInterceptor, IMaterializationInterceptor
{
    private readonly ConcurrentDictionary<string, int> _commands = new();
    private readonly ConcurrentDictionary<string, int> _tokens = new();
    public int Commands(string key) => _commands.GetValueOrDefault(key);
    public int Tokens(string key) => _tokens.GetValueOrDefault(key);

    private void Count()
    {
        var key = accessor.HttpContext?.Request.Headers["X-Test-Request"].ToString();
        if (!string.IsNullOrEmpty(key)) _commands.AddOrUpdate(key, 1, (_, value) => value + 1);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
        CommandEventData data, InterceptionResult<DbDataReader> result, CancellationToken ct = default)
    { Count(); return ValueTask.FromResult(result); }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command,
        CommandEventData data, InterceptionResult<int> result, CancellationToken ct = default)
    { Count(); return ValueTask.FromResult(result); }
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command,
        CommandEventData data, InterceptionResult<object> result, CancellationToken ct = default)
    { Count(); return ValueTask.FromResult(result); }

    public object InitializedInstance(MaterializationInterceptionData data, object entity)
    {
        var key = accessor.HttpContext?.Request.Headers["X-Test-Request"].ToString();
        if (entity is RefreshToken && !string.IsNullOrEmpty(key)) _tokens.AddOrUpdate(key, 1, (_, value) => value + 1);
        return entity;
    }
}
