using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Tests.Application.Fakes;

public class FakeSaldoEstoqueRepository : ISaldoEstoqueRepository
{
    private readonly List<SaldoEstoque> _saldos = [];
    private readonly List<MovimentoEstoque> _movimentos = [];

    public void Seed(SaldoEstoque saldo) => _saldos.Add(saldo);

    public Task<SaldoEstoque?> ObterPorProdutoAsync(int produtoId, int empresaId, CancellationToken ct = default) =>
        Task.FromResult(_saldos.FirstOrDefault(s => s.ProdutoId == produtoId && s.EmpresaId == empresaId));

    public Task<IReadOnlyList<SaldoEstoque>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SaldoEstoque>>(_saldos.Where(s => s.EmpresaId == empresaId).ToList());

    public Task AdicionarAsync(SaldoEstoque saldo, CancellationToken ct = default)
    {
        _saldos.Add(saldo);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(SaldoEstoque saldo, CancellationToken ct = default) => Task.CompletedTask;

    public Task AdicionarMovimentoAsync(MovimentoEstoque movimento, CancellationToken ct = default)
    {
        _movimentos.Add(movimento);
        return Task.CompletedTask;
    }

    public IReadOnlyList<MovimentoEstoque> Movimentos => _movimentos.AsReadOnly();
}
