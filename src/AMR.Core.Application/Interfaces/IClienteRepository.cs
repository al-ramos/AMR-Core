using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ListarAtivosAsync(int empresaId, CancellationToken ct = default);
}
