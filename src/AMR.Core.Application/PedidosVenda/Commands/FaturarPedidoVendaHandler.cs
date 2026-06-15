using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Enums;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Commands;

public class FaturarPedidoVendaHandler(
    IPedidoVendaRepository vendaRepo,
    ISaldoEstoqueRepository estoqueRepo,
    IUnitOfWork uow,
    ITmsApiClient tmsApiClient)
    : IRequestHandler<FaturarPedidoVendaCommand, Result<PedidoVendaDto>>
{
    public async Task<Result<PedidoVendaDto>> Handle(FaturarPedidoVendaCommand cmd, CancellationToken ct)
    {
        var pedido = await vendaRepo.ObterPorIdAsync(cmd.PedidoId, ct);
        if (pedido is null)
            return Result.Falha<PedidoVendaDto>($"Pedido de venda #{cmd.PedidoId} não encontrado.");

        // Valida saldo antes de faturar
        foreach (var item in pedido.Itens)
        {
            var saldo = await estoqueRepo.ObterPorProdutoAsync(item.ProdutoId, pedido.EmpresaId, ct);
            if (saldo is null || saldo.Quantidade < item.Quantidade)
                return Result.Falha<PedidoVendaDto>(
                    $"Saldo insuficiente para o produto #{item.ProdutoId}. " +
                    $"Disponível: {saldo?.Quantidade ?? 0}, Necessário: {item.Quantidade}.");
        }

        try { pedido.Faturar(); }
        catch (InvalidOperationException ex) { return Result.Falha<PedidoVendaDto>(ex.Message); }

        // Baixa estoque
        foreach (var item in pedido.Itens)
        {
            var saldo = await estoqueRepo.ObterPorProdutoAsync(item.ProdutoId, pedido.EmpresaId, ct);
            var movimento = saldo!.Movimentar(TipoMovimentoEstoque.Saida, item.Quantidade, $"PV#{pedido.Id}");
            await estoqueRepo.AdicionarMovimentoAsync(movimento, ct);
            await estoqueRepo.AtualizarAsync(saldo, ct);
        }

        await vendaRepo.AtualizarAsync(pedido, ct);
        await uow.CommitAsync(ct);

        // Fire-and-forget resiliente — falha no TMS não bloqueia o Core
        var tmsDto = new CriarOrdemTmsDto(
            Origem:           "AMR-Core",
            Destino:          $"Cliente:{pedido.ClienteId}",
            ClienteId:        pedido.ClienteId.ToString(),
            TransportadoraId: null,
            PesoKg:           0.001m,
            VolumeM3:         0.001m,
            ValorFrete:       0m,
            PrevisaoEntrega:  null,
            PedidoCoreId:     pedido.Id.ToString()
        );
        _ = Task.Run(async () =>
            await tmsApiClient.CriarOrdemDeEntregaAsync(tmsDto), CancellationToken.None);

        return Result.Ok(CriarPedidoVendaHandler.ToDto(pedido));
    }
}
