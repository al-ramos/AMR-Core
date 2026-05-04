using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Infrastructure.Data;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class SaldoEstoqueRepository(AmrCoreDbContext ctx) : ISaldoEstoqueRepository
{
    public Task<SaldoEstoque?> ObterPorProdutoAsync(int produtoId, int empresaId, CancellationToken ct = default) =>
        ctx.SaldosEstoque.FirstOrDefaultAsync(s => s.ProdutoId == produtoId && s.EmpresaId == empresaId, ct);

    public async Task<IReadOnlyList<SaldoEstoque>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default) =>
        await ctx.SaldosEstoque
            .Include(s => s.Produto).ThenInclude(p => p!.UnidadeMedida)
            .Where(s => s.EmpresaId == empresaId)
            .ToListAsync(ct);

    public Task AdicionarAsync(SaldoEstoque saldo, CancellationToken ct = default) =>
        ctx.SaldosEstoque.AddAsync(saldo, ct).AsTask();

    public Task AtualizarAsync(SaldoEstoque saldo, CancellationToken ct = default)
    {
        // Se a entidade já está sendo rastreada (ex: recém-criada com AddAsync),
        // o EF Core detecta as mudanças automaticamente — chamar Update() causaria
        // erro ao tentar transitar de Added para Modified com PK = 0.
        if (ctx.Entry(saldo).State == EntityState.Detached)
            ctx.SaldosEstoque.Update(saldo);

        return Task.CompletedTask;
    }

    public Task AdicionarMovimentoAsync(MovimentoEstoque movimento, CancellationToken ct = default) =>
        ctx.MovimentosEstoque.AddAsync(movimento, ct).AsTask();
}
