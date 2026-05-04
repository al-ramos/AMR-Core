using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Queries;

public class ListarPedidosVendaHandler(IPedidoVendaRepository repo)
    : IRequestHandler<ListarPedidosVendaQuery, Result<IReadOnlyList<PedidoVendaDto>>>
{
    public async Task<Result<IReadOnlyList<PedidoVendaDto>>> Handle(ListarPedidosVendaQuery q, CancellationToken ct)
    {
        var pedidos = await repo.ListarPorEmpresaAsync(q.EmpresaId, q.Status, ct);
        var dtos = pedidos.Select(CriarPedidoVendaHandler.ToDto).ToList();
        return Result.Ok<IReadOnlyList<PedidoVendaDto>>(dtos);
    }
}
