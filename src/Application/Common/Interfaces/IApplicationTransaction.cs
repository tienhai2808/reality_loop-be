namespace Application.Common.Interfaces;

public interface IApplicationTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
}
