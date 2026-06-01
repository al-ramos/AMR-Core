using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Application.PedidosCompra.Commands;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Queries;

public class ListarPedidosCompraHandler(IPedidoCompraRepository repo)
    : IRequestHandler<ListarPedidosCompraQuery, Result<IReadOnlyList<PedidoCompraDto>>>
{
    public async Task<Result<IReadOnlyList<PedidoCompraDto>>> Handle(ListarPedidosCompraQuery q, CancellationToken ct)
    {
        var pedidos = await repo.ListarPorEmpresaAsync(q.EmpresaId, ct);

        var dtos = pedidos
            .Where(p => q.Status is null || p.Status.ToString() == q.Status)
            .Select(CriarPedidoCompraHandler.ToDto)
            .ToList();

        return Result.Ok<IReadOnlyList<PedidoCompraDto>>(dtos);
    }
}
