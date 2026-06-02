using Microsoft.EntityFrameworkCore;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Infrastructure.Data.Repositories;

public class ClienteRepository(AmrCoreDbContext ctx) : IClienteRepository
{
    public async Task<IReadOnlyList<Cliente>> ListarAtivosAsync(int empresaId, CancellationToken ct = default) =>
        await ctx.Clientes
            .Where(c => c.EmpresaId == empresaId && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync(ct);
}
