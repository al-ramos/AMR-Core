using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface IOrdemRecebimentoRepository
{
    Task<OrdemRecebimento?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<OrdemRecebimento>> ListarAsync(CancellationToken ct = default);
    Task<bool> ExistePorPedidoCompraAsync(int pedidoCompraId, CancellationToken ct = default);
    Task AdicionarAsync(OrdemRecebimento ordem, CancellationToken ct = default);
    Task AtualizarAsync(OrdemRecebimento ordem, CancellationToken ct = default);
}
