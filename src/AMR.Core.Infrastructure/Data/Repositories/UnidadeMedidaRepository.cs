using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class UnidadeMedidaRepository(AmrCoreDbContext ctx) : IUnidadeMedidaRepository
{
    public async Task<IReadOnlyList<UnidadeMedida>> ListarAsync(CancellationToken ct = default) =>
        await ctx.UnidadesMedida
            .OrderBy(u => u.Sigla)
            .ToListAsync(ct);
}
