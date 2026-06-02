using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Tests.Application.Fakes;

public class FakeProdutoRepository : IProdutoRepository
{
    private readonly List<Produto> _store = [];
    private int _nextId = 1;

    public Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_store.FirstOrDefault(p => p.Id == id));

    public Task<Produto?> ObterPorSkuAsync(string sku, CancellationToken ct = default) =>
        Task.FromResult(_store.FirstOrDefault(p => p.SKU == sku.ToUpper()));

    public Task<IReadOnlyList<Produto>> ListarAtivosAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Produto>>(_store.Where(p => p.Ativo).ToList());

    public Task AdicionarAsync(Produto produto, CancellationToken ct = default)
    {
        SetId(produto, _nextId++);
        _store.Add(produto);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Produto produto, CancellationToken ct = default) => Task.CompletedTask;

    public Task<bool> ExisteSkuAsync(string sku, int? ignorarId = null, CancellationToken ct = default) =>
        Task.FromResult(_store.Any(p => p.SKU == sku.ToUpper() && p.Id != ignorarId));

    private static void SetId(Produto produto, int id)
    {
        typeof(Produto).GetProperty("Id")!.SetValue(produto, id);
    }
}
