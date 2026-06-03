using AMR.Core.Domain.Entities;

namespace AMR.Core.Application.Interfaces;

public interface IMovimentoEstoqueRepository
{
    Task<IReadOnlyList<MovimentoEstoque>> ListarAsync(
        int empresaId, int? produtoId, string? tipo, CancellationToken ct = default);
}
