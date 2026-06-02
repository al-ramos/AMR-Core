using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface IUnidadeMedidaRepository
{
    Task<IReadOnlyList<UnidadeMedida>> ListarAsync(CancellationToken ct = default);
}
