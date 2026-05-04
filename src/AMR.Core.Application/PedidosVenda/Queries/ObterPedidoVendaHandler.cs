using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Queries;

public class ObterPedidoVendaHandler(IPedidoVendaRepository repo)
    : IRequestHandler<ObterPedidoVendaQuery, Result<PedidoVendaDto>>
{
    public async Task<Result<PedidoVendaDto>> Handle(ObterPedidoVendaQuery q, CancellationToken ct)
    {
        var pedido = await repo.ObterPorIdAsync(q.PedidoId, ct);
        if (pedido is null)
            return Result.Falha<PedidoVendaDto>($"Pedido de venda #{q.PedidoId} não encontrado.");

        return Result.Ok(CriarPedidoVendaHandler.ToDto(pedido));
    }
}
