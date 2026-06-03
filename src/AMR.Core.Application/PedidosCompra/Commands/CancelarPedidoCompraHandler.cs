using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Commands;

public class CancelarPedidoCompraHandler(IPedidoCompraRepository repo, IUnitOfWork uow)
    : IRequestHandler<CancelarPedidoCompraCommand, Result<PedidoCompraDto>>
{
    public async Task<Result<PedidoCompraDto>> Handle(CancelarPedidoCompraCommand cmd, CancellationToken ct)
    {
        var pedido = await repo.ObterPorIdAsync(cmd.PedidoId, ct);
        if (pedido is null)
            return Result.Falha<PedidoCompraDto>($"Pedido de compra #{cmd.PedidoId} não encontrado.");

        try   { pedido.Cancelar(); }
        catch (InvalidOperationException ex) { return Result.Falha<PedidoCompraDto>(ex.Message); }

        await repo.AtualizarAsync(pedido, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(CriarPedidoCompraHandler.ToDto(pedido));
    }
}
