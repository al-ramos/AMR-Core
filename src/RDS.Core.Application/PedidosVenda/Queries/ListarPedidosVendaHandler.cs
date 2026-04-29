using MediatR;
using RDS.Core.Application.DTOs;
using RDS.Core.Application.Interfaces;
using RDS.Core.Application.PedidosVenda.Commands;
using RDS.Core.Shared.Results;

namespace RDS.Core.Application.PedidosVenda.Queries;

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
