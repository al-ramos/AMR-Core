using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class MovimentoEstoqueRepository(AmrCoreDbContext ctx) : IMovimentoEstoqueRepository
{
    public async Task<IReadOnlyList<MovimentoEstoque>> ListarAsync(
        int empresaId, int? produtoId, string? tipo, CancellationToken ct = default)
    {
        var query = ctx.MovimentosEstoque
            .Include(m => m.Produto)
            .Where(m => m.EmpresaId == empresaId);

        if (produtoId.HasValue)
            query = query.Where(m => m.ProdutoId == produtoId.Value);

        if (!string.IsNullOrWhiteSpace(tipo) &&
            Enum.TryParse<TipoMovimentoEstoque>(tipo, ignoreCase: true, out var tipoEnum))
            query = query.Where(m => m.Tipo == tipoEnum);

        return await query
            .OrderByDescending(m => m.DataHora)
            .ToListAsync(ct);
    }
}
