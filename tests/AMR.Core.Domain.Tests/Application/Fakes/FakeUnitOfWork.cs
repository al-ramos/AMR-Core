using AMR.Core.Application.Interfaces;

namespace AMR.Core.Domain.Tests.Application.Fakes;

public class FakeUnitOfWork : IUnitOfWork
{
    public int CommitCount { get; private set; }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        CommitCount++;
        return Task.FromResult(CommitCount);
    }

    public void Dispose() { }
}
