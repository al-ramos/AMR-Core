using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Cliente?> ObterPorDocumentoAsync(string numeroDocumento, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> ListarAtivosAsync(int empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> BuscarPorNomeAsync(int empresaId, string termo, CancellationToken ct = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken ct = default);
    Task AtualizarAsync(Cliente cliente, CancellationToken ct = default);
    Task<bool> ExisteDocumentoAsync(string numeroDocumento, int empresaId, int? ignorarId = null, CancellationToken ct = default);
}
