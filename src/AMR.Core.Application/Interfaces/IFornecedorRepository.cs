using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface IFornecedorRepository
{
    Task<IReadOnlyList<Fornecedor>> ListarAtivosAsync(int empresaId, CancellationToken ct = default);
}
