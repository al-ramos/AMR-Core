using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;
using AMR.Core.Infrastructure.Data;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class PedidoVendaRepository(AmrCoreDbContext ctx) : IPedidoVendaRepository
{
    public Task<PedidoVenda?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        ctx.PedidosVenda
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<PedidoVenda>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default) =>
        await ctx.PedidosVenda
            .Include(p => p.Itens)
            .Where(p => p.EmpresaId == empresaId)
            .OrderByDescending(p => p.DataEmissao)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<PedidoVenda>> ListarPorEmpresaAsync(int empresaId, string? status, CancellationToken ct = default)
    {
        StatusPedidoVenda? statusEnum = null;

        if (status is not null && Enum.TryParse<StatusPedidoVenda>(status, ignoreCase: true, out var parsed))
            statusEnum = parsed;

        return await ctx.PedidosVenda
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .Where(p => p.EmpresaId == empresaId &&
                        (statusEnum == null || p.Status == statusEnum))
            .OrderByDescending(p => p.DataEmissao)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PedidoVenda>> ListarPorClienteAsync(int clienteId, CancellationToken ct = default) =>
        await ctx.PedidosVenda
            .Where(p => p.ClienteId == clienteId)
            .OrderByDescending(p => p.DataEmissao)
            .ToListAsync(ct);

    public Task AdicionarAsync(PedidoVenda pedido, CancellationToken ct = default) =>
        ctx.PedidosVenda.AddAsync(pedido, ct).AsTask();

    public Task AtualizarAsync(PedidoVenda pedido, CancellationToken ct = default)
    {
        ctx.PedidosVenda.Update(pedido);
        return Task.CompletedTask;
    }
}
