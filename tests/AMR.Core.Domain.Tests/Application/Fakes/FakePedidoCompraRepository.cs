using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Tests.Application.Fakes;

public class FakePedidoCompraRepository : IPedidoCompraRepository
{
    private readonly List<PedidoCompra> _store = [];
    private int _nextId = 1;

    public Task<PedidoCompra?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_store.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<PedidoCompra>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<PedidoCompra>>(_store.Where(p => p.EmpresaId == empresaId).ToList());

    public Task AdicionarAsync(PedidoCompra pedido, CancellationToken ct = default)
    {
        SetId(pedido, _nextId++);
        _store.Add(pedido);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(PedidoCompra pedido, CancellationToken ct = default) => Task.CompletedTask;

    private static void SetId(PedidoCompra pedido, int id)
    {
        typeof(PedidoCompra).GetProperty("Id")!.SetValue(pedido, id);
    }
}
