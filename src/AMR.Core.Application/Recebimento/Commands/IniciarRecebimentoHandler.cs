using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Commands;

public class IniciarRecebimentoHandler(
    IOrdemRecebimentoRepository ordemRepo,
    IPedidoCompraRepository compraRepo,
    IUnitOfWork uow)
    : IRequestHandler<IniciarRecebimentoCommand, Result<OrdemRecebimentoDto>>
{
    public async Task<Result<OrdemRecebimentoDto>> Handle(IniciarRecebimentoCommand cmd, CancellationToken ct)
    {
        var pc = await compraRepo.ObterPorIdAsync(cmd.PedidoCompraId, ct);
        if (pc is null)
            return Result.Falha<OrdemRecebimentoDto>($"Pedido de compra #{cmd.PedidoCompraId} não encontrado.");

        if (pc.Status != StatusPedidoCompra.Aprovado)
            return Result.Falha<OrdemRecebimentoDto>($"Pedido de compra #{cmd.PedidoCompraId} não está Aprovado (status atual: {pc.Status}).");

        if (await ordemRepo.ExistePorPedidoCompraAsync(cmd.PedidoCompraId, ct))
            return Result.Falha<OrdemRecebimentoDto>($"Já existe uma ordem de recebimento para o pedido #{cmd.PedidoCompraId}.");

        var itens = pc.Itens.Select(i => (i.ProdutoId, i.Quantidade));
        var ordem = OrdemRecebimento.Criar(cmd.PedidoCompraId, itens);
        ordem.IniciarRecebimento();

        await ordemRepo.AdicionarAsync(ordem, ct);
        await uow.CommitAsync(ct);

        var saved = await ordemRepo.ObterPorIdAsync(ordem.Id, ct);
        return Result.Ok(ToDto(saved!));
    }

    internal static OrdemRecebimentoDto ToDto(OrdemRecebimento o) => new(
        o.Id, o.PedidoCompraId, o.Status.ToString(), o.DataCriacao, o.DataRecebimento,
        o.Itens.Select(i => new ItemRecebimentoDto(
            i.Id, i.ProdutoId, i.Produto?.Nome ?? "", i.LocalizacaoId, i.QntEsperada, i.QntRecebida
        )).ToList().AsReadOnly());
}
