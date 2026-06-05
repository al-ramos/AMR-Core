using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Infrastructure.Data;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class OrdemRecebimentoRepository(AmrCoreDbContext ctx) : IOrdemRecebimentoRepository
{
    public Task<OrdemRecebimento?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        ctx.OrdensRecebimento
            .Include(o => o.Itens).ThenInclude(i => i.Produto)
            .Include(o => o.PedidoCompra)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<OrdemRecebimento>> ListarAsync(CancellationToken ct = default) =>
        await ctx.OrdensRecebimento
            .Include(o => o.Itens).ThenInclude(i => i.Produto)
            .Include(o => o.PedidoCompra).ThenInclude(pc => pc!.Fornecedor)
            .OrderByDescending(o => o.DataCriacao)
            .ToListAsync(ct);

    public Task<bool> ExistePorPedidoCompraAsync(int pedidoCompraId, CancellationToken ct = default) =>
        ctx.OrdensRecebimento.AnyAsync(o => o.PedidoCompraId == pedidoCompraId, ct);

    public Task AdicionarAsync(OrdemRecebimento ordem, CancellationToken ct = default) =>
        ctx.OrdensRecebimento.AddAsync(ordem, ct).AsTask();

    public Task AtualizarAsync(OrdemRecebimento ordem, CancellationToken ct = default)
    {
        ctx.OrdensRecebimento.Update(ordem);
        return Task.CompletedTask;
    }
}
