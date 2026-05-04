using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Interfaces;

public interface IEmpresaRepository
{
    Task<Empresa?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Empresa?> ObterPorCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IReadOnlyList<Empresa>> ListarTodasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Empresa>> ListarAtivasAsync(CancellationToken ct = default);
    Task AdicionarAsync(Empresa empresa, CancellationToken ct = default);
    Task AtualizarAsync(Empresa empresa, CancellationToken ct = default);
    Task<bool> ExisteCnpjAsync(string cnpj, int? ignorarId = null, CancellationToken ct = default);
}
