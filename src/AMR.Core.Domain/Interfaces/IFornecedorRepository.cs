using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Interfaces;

public interface IFornecedorRepository
{
    Task<Fornecedor?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Fornecedor?> ObterPorCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<IReadOnlyList<Fornecedor>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<Fornecedor>> ListarPorCategoriaAsync(int empresaId, string categoria, CancellationToken ct = default);
    Task<IReadOnlyList<Fornecedor>> ListarAtivosAsync(int empresaId, CancellationToken ct = default);
    Task<IReadOnlyList<Fornecedor>> BuscarPorNomeAsync(int empresaId, string termo, CancellationToken ct = default);
    Task AdicionarAsync(Fornecedor fornecedor, CancellationToken ct = default);
    Task AtualizarAsync(Fornecedor fornecedor, CancellationToken ct = default);
    Task<bool> ExisteCnpjAsync(string cnpj, int empresaId, int? ignorarId = null, CancellationToken ct = default);
}
