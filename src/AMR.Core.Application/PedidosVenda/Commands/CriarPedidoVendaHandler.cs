using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Commands;

public class CriarPedidoVendaHandler(IPedidoVendaRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarPedidoVendaCommand, Result<PedidoVendaDto>>
{
    public async Task<Result<PedidoVendaDto>> Handle(CriarPedidoVendaCommand cmd, CancellationToken ct)
    {
        var pedido = PedidoVenda.Criar(cmd.EmpresaId, cmd.ClienteId, cmd.Observacao);

        foreach (var item in cmd.Itens)
            pedido.AdicionarItem(item.ProdutoId, item.Quantidade, item.PrecoUnitario, item.Desconto);

        await repo.AdicionarAsync(pedido, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(ToDto(pedido));
    }

    internal static PedidoVendaDto ToDto(PedidoVenda p) => new(
        p.Id, p.EmpresaId, p.ClienteId, p.Status.ToString(),
        p.DataEmissao, p.DataAprovacao, p.DataFaturamento, p.Observacao, p.Total,
        p.Itens.Select(i => new ItemPedidoDto(
            i.ProdutoId, i.Produto?.Nome ?? "", i.Quantidade, i.PrecoUnitario, i.Desconto, i.Total
        )).ToList().AsReadOnly());
}
