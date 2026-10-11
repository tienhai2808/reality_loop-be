namespace Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task<IApplicationTransaction> BeginTransactionAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

