using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Produto?> ObterPorSkuAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Produto>> ListarAtivosAsync(CancellationToken ct = default);
    Task AdicionarAsync(Produto produto, CancellationToken ct = default);
    Task AtualizarAsync(Produto produto, CancellationToken ct = default);
    Task<bool> ExisteSkuAsync(string sku, int? ignorarId = null, CancellationToken ct = default);
}
