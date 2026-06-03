using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Infrastructure.Data;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class ProdutoRepository(AmrCoreDbContext ctx) : IProdutoRepository
{
    public Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        ctx.Produtos.Include(p => p.UnidadeMedida).FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Produto?> ObterPorSkuAsync(string sku, CancellationToken ct = default) =>
        ctx.Produtos.FirstOrDefaultAsync(p => p.SKU == sku.ToUpper(), ct);

    public async Task<IReadOnlyList<Produto>> ListarAtivosAsync(CancellationToken ct = default) =>
        await ctx.Produtos.Include(p => p.UnidadeMedida).Where(p => p.Ativo).ToListAsync(ct);

    public async Task<IReadOnlyList<Produto>> ListarTodosAsync(CancellationToken ct = default) =>
        await ctx.Produtos.Include(p => p.UnidadeMedida).OrderBy(p => p.Nome).ToListAsync(ct);

    public Task AdicionarAsync(Produto produto, CancellationToken ct = default) =>
        ctx.Produtos.AddAsync(produto, ct).AsTask();

    public Task AtualizarAsync(Produto produto, CancellationToken ct = default)
    {
        ctx.Produtos.Update(produto);
        return Task.CompletedTask;
    }

    public Task<bool> ExisteSkuAsync(string sku, int? ignorarId = null, CancellationToken ct = default) =>
        ctx.Produtos.AnyAsync(p => p.SKU == sku.ToUpper() && (ignorarId == null || p.Id != ignorarId), ct);
}
