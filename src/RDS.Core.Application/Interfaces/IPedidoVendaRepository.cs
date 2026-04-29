using RDS.Core.Domain.Entities;

namespace RDS.Core.Application.Interfaces;

public interface IPedidoVendaRepository
{
    Task<PedidoVenda?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PedidoVenda>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<PedidoVenda>> ListarPorEmpresaAsync(int empresaId, string? status, CancellationToken ct = default);
    Task<IReadOnlyList<PedidoVenda>> ListarPorClienteAsync(int clienteId, CancellationToken ct = default);
    Task AdicionarAsync(PedidoVenda pedido, CancellationToken ct = default);
    Task AtualizarAsync(PedidoVenda pedido, CancellationToken ct = default);
}
