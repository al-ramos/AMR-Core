using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface ISaldoEstoqueRepository
{
    Task<SaldoEstoque?> ObterPorProdutoAsync(int produtoId, int empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<SaldoEstoque>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default);
    Task AdicionarAsync(SaldoEstoque saldo, CancellationToken ct = default);
    Task AtualizarAsync(SaldoEstoque saldo, CancellationToken ct = default);
    Task AdicionarMovimentoAsync(MovimentoEstoque movimento, CancellationToken ct = default);
}
