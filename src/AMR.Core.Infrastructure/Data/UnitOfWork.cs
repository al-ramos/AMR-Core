using AMR.Core.Application.Interfaces;

namespace AMR.Core.Infrastructure.Data;

public class UnitOfWork(AmrCoreDbContext ctx) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default) => ctx.SaveChangesAsync(ct);
    public void Dispose() => ctx.Dispose();
}
