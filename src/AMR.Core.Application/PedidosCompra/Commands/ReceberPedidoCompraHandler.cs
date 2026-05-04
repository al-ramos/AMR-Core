using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Enums;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Commands;

public class ReceberPedidoCompraHandler(
    IPedidoCompraRepository compraRepo,
    ISaldoEstoqueRepository estoqueRepo,
    IUnitOfWork uow)
    : IRequestHandler<ReceberPedidoCompraCommand, Result<PedidoCompraDto>>
{
    public async Task<Result<PedidoCompraDto>> Handle(ReceberPedidoCompraCommand cmd, CancellationToken ct)
    {
        var pedido = await compraRepo.ObterPorIdAsync(cmd.PedidoId, ct);
        if (pedido is null)
            return Result.Falha<PedidoCompraDto>($"Pedido de compra #{cmd.PedidoId} não encontrado.");

        try { pedido.Receber(); }
        catch (InvalidOperationException ex) { return Result.Falha<PedidoCompraDto>(ex.Message); }

        // Gera entrada no estoque para cada item
        foreach (var item in pedido.Itens)
        {
            var saldo = await estoqueRepo.ObterPorProdutoAsync(item.ProdutoId, pedido.EmpresaId, ct);

            if (saldo is null)
            {
                saldo = Domain.Entities.SaldoEstoque.Criar(item.ProdutoId, pedido.EmpresaId);
                await estoqueRepo.AdicionarAsync(saldo, ct);
            }

            var movimento = saldo.Movimentar(TipoMovimentoEstoque.Entrada, item.Quantidade, $"PC#{pedido.Id}");
            await estoqueRepo.AdicionarMovimentoAsync(movimento, ct);
            await estoqueRepo.AtualizarAsync(saldo, ct);
        }

        await compraRepo.AtualizarAsync(pedido, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(CriarPedidoCompraHandler.ToDto(pedido));
    }
}
