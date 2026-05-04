using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Infrastructure.Data;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class PedidoCompraRepository(AmrCoreDbContext ctx) : IPedidoCompraRepository
{
    public Task<PedidoCompra?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        ctx.PedidosCompra
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<PedidoCompra>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default) =>
        await ctx.PedidosCompra
            .Include(p => p.Itens)
            .Where(p => p.EmpresaId == empresaId)
            .OrderByDescending(p => p.DataEmissao)
            .ToListAsync(ct);

    public Task AdicionarAsync(PedidoCompra pedido, CancellationToken ct = default) =>
        ctx.PedidosCompra.AddAsync(pedido, ct).AsTask();

    public Task AtualizarAsync(PedidoCompra pedido, CancellationToken ct = default)
    {
        ctx.PedidosCompra.Update(pedido);
        return Task.CompletedTask;
    }
}
