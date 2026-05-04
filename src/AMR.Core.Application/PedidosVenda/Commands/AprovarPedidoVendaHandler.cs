using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Commands;

public class AprovarPedidoVendaHandler(IPedidoVendaRepository repo, IUnitOfWork uow)
    : IRequestHandler<AprovarPedidoVendaCommand, Result<PedidoVendaDto>>
{
    public async Task<Result<PedidoVendaDto>> Handle(AprovarPedidoVendaCommand cmd, CancellationToken ct)
    {
        var pedido = await repo.ObterPorIdAsync(cmd.PedidoId, ct);
        if (pedido is null)
            return Result.Falha<PedidoVendaDto>($"Pedido de venda #{cmd.PedidoId} não encontrado.");

        try { pedido.Aprovar(); }
        catch (InvalidOperationException ex) { return Result.Falha<PedidoVendaDto>(ex.Message); }

        await repo.AtualizarAsync(pedido, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(CriarPedidoVendaHandler.ToDto(pedido));
    }
}
