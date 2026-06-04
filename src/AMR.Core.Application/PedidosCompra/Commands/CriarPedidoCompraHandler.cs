using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Commands;

public class CriarPedidoCompraHandler(IPedidoCompraRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarPedidoCompraCommand, Result<PedidoCompraDto>>
{
    public async Task<Result<PedidoCompraDto>> Handle(CriarPedidoCompraCommand cmd, CancellationToken ct)
    {
        var pedido = PedidoCompra.Criar(cmd.EmpresaId, cmd.FornecedorId, cmd.Observacao);

        foreach (var item in cmd.Itens)
            pedido.AdicionarItem(item.ProdutoId, item.Quantidade, item.PrecoUnitario);

        await repo.AdicionarAsync(pedido, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(ToDto(pedido));
    }

    internal static PedidoCompraDto ToDto(PedidoCompra p) => new(
        p.Id, p.EmpresaId, p.FornecedorId, p.Status.ToString(),
        p.DataEmissao, p.DataAprovacao, p.DataRecebimento, p.Observacao, p.Total,
        p.Itens.Select(i => new ItemPedidoDto(
            i.ProdutoId, i.Produto?.Nome ?? "", i.Quantidade, i.PrecoUnitario, 0, i.Total
        )).ToList().AsReadOnly());
}
