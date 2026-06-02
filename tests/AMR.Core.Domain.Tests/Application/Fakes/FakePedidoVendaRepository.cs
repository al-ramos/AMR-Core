using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;

namespace AMR.Core.Domain.Tests.Application.Fakes;

public class FakePedidoVendaRepository : IPedidoVendaRepository
{
    private readonly List<PedidoVenda> _store = [];
    private int _nextId = 1;

    public Task<PedidoVenda?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_store.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<PedidoVenda>> ListarPorEmpresaAsync(int empresaId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<PedidoVenda>>(_store.Where(p => p.EmpresaId == empresaId).ToList());

    public Task<IReadOnlyList<PedidoVenda>> ListarPorEmpresaAsync(int empresaId, string? status, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<PedidoVenda>>(_store
            .Where(p => p.EmpresaId == empresaId && (status == null || p.Status.ToString() == status))
            .ToList());

    public Task<IReadOnlyList<PedidoVenda>> ListarPorClienteAsync(int clienteId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<PedidoVenda>>(_store.Where(p => p.ClienteId == clienteId).ToList());

    public Task AdicionarAsync(PedidoVenda pedido, CancellationToken ct = default)
    {
        SetId(pedido, _nextId++);
        _store.Add(pedido);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(PedidoVenda pedido, CancellationToken ct = default) => Task.CompletedTask;

    private static void SetId(PedidoVenda pedido, int id)
    {
        typeof(PedidoVenda).GetProperty("Id")!.SetValue(pedido, id);
    }
}
