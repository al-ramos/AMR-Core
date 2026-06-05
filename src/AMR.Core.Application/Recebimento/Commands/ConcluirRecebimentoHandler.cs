using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Commands;

public class ConcluirRecebimentoHandler(
    IOrdemRecebimentoRepository ordemRepo,
    IPedidoCompraRepository compraRepo,
    ISaldoEstoqueRepository estoqueRepo,
    IUnitOfWork uow)
    : IRequestHandler<ConcluirRecebimentoCommand, Result<OrdemRecebimentoDto>>
{
    public async Task<Result<OrdemRecebimentoDto>> Handle(ConcluirRecebimentoCommand cmd, CancellationToken ct)
    {
        var ordem = await ordemRepo.ObterPorIdAsync(cmd.OrdemId, ct);
        if (ordem is null)
            return Result.Falha<OrdemRecebimentoDto>($"Ordem de recebimento #{cmd.OrdemId} não encontrada.");

        try { ordem.Concluir(); }
        catch (InvalidOperationException ex) { return Result.Falha<OrdemRecebimentoDto>(ex.Message); }

        var pc = await compraRepo.ObterPorIdAsync(ordem.PedidoCompraId, ct);
        if (pc is null)
            return Result.Falha<OrdemRecebimentoDto>($"Pedido de compra #{ordem.PedidoCompraId} não encontrado.");

        try { pc.Receber(); }
        catch (InvalidOperationException ex) { return Result.Falha<OrdemRecebimentoDto>(ex.Message); }

        foreach (var item in ordem.Itens.Where(i => i.QntRecebida > 0))
        {
            var saldo = await estoqueRepo.ObterPorProdutoAsync(item.ProdutoId, pc.EmpresaId, ct);
            if (saldo is null)
            {
                saldo = SaldoEstoque.Criar(item.ProdutoId, pc.EmpresaId);
                await estoqueRepo.AdicionarAsync(saldo, ct);
            }
            var movimento = saldo.Movimentar(TipoMovimentoEstoque.Entrada, item.QntRecebida, $"OR#{cmd.OrdemId}/PC#{pc.Id}");
            await estoqueRepo.AdicionarMovimentoAsync(movimento, ct);
            await estoqueRepo.AtualizarAsync(saldo, ct);
        }

        await ordemRepo.AtualizarAsync(ordem, ct);
        await compraRepo.AtualizarAsync(pc, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(IniciarRecebimentoHandler.ToDto(ordem));
    }
}
