using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class FornecedorRepository(AmrCoreDbContext ctx) : IFornecedorRepository
{
    public async Task<IReadOnlyList<Fornecedor>> ListarAtivosAsync(int empresaId, CancellationToken ct = default) =>
        await ctx.Fornecedores
            .Where(f => f.EmpresaId == empresaId && f.Ativo)
            .OrderBy(f => f.NomeFantasia)
            .ToListAsync(ct);
}
