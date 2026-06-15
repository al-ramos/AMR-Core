using Moq;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;

namespace AMR.Core.Application.Tests.PedidosVenda;

public class FaturarPedidoVendaHandlerTests
{
    // ── helpers ──────────────────────────────────────────────────────────────

    private static PedidoVenda CriarPedidoAprovado(int empresaId = 1, int clienteId = 42)
    {
        var p = PedidoVenda.Criar(empresaId, clienteId);
        p.AdicionarItem(produtoId: 1, quantidade: 5, precoUnitario: 100m);
        p.Aprovar();
        return p;
    }

    private static SaldoEstoque CriarSaldoSuficiente(int produtoId, int empresaId, decimal qtd)
    {
        var s = SaldoEstoque.Criar(produtoId, empresaId);
        s.Movimentar(TipoMovimentoEstoque.Entrada, qtd, "Seed de teste");
        return s;
    }

    // ── Cenário 1: TMS disponível ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_TmsDisponivel_PedidoFaturadoEOrdemCriada()
    {
        // Arrange
        var pedido = CriarPedidoAprovado();
        var saldo  = CriarSaldoSuficiente(produtoId: 1, empresaId: 1, qtd: 10);

        var vendaRepo  = new Mock<IPedidoVendaRepository>();
        var estoqueRepo = new Mock<ISaldoEstoqueRepository>();
        var uow        = new Mock<IUnitOfWork>();
        var tmsClient  = new Mock<ITmsApiClient>();

        vendaRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(pedido);
        estoqueRepo.Setup(r => r.ObterPorProdutoAsync(1, 1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(saldo);
        estoqueRepo.Setup(r => r.AdicionarMovimentoAsync(It.IsAny<MovimentoEstoque>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);
        estoqueRepo.Setup(r => r.AtualizarAsync(It.IsAny<SaldoEstoque>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);
        vendaRepo.Setup(r => r.AtualizarAsync(It.IsAny<PedidoVenda>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        tmsClient.Setup(t => t.CriarOrdemDeEntregaAsync(It.IsAny<CriarOrdemTmsDto>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync("tms-ordem-001");

        var handler = new FaturarPedidoVendaHandler(vendaRepo.Object, estoqueRepo.Object, uow.Object, tmsClient.Object);

        // Act
        var result = await handler.Handle(new FaturarPedidoVendaCommand(1), CancellationToken.None);

        // Aguarda fire-and-forget concluir
        await Task.Delay(200);

        // Assert — pedido confirmado
        Assert.True(result.Sucesso);
        Assert.Equal("Faturado", result.Valor!.Status);

        // Assert — TMS chamado com PedidoCoreId correto
        tmsClient.Verify(t => t.CriarOrdemDeEntregaAsync(
            It.Is<CriarOrdemTmsDto>(dto => dto.PedidoCoreId == pedido.Id.ToString()),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Cenário 2: TMS offline (exceção) ─────────────────────────────────────

    [Fact]
    public async Task Handle_TmsOffline_PedidoFaturadoMesmoAssim()
    {
        // Arrange
        var pedido = CriarPedidoAprovado();
        var saldo  = CriarSaldoSuficiente(produtoId: 1, empresaId: 1, qtd: 10);

        var vendaRepo   = new Mock<IPedidoVendaRepository>();
        var estoqueRepo = new Mock<ISaldoEstoqueRepository>();
        var uow         = new Mock<IUnitOfWork>();
        var tmsClient   = new Mock<ITmsApiClient>();

        vendaRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(pedido);
        estoqueRepo.Setup(r => r.ObterPorProdutoAsync(1, 1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(saldo);
        estoqueRepo.Setup(r => r.AdicionarMovimentoAsync(It.IsAny<MovimentoEstoque>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);
        estoqueRepo.Setup(r => r.AtualizarAsync(It.IsAny<SaldoEstoque>(), It.IsAny<CancellationToken>()))
                   .Returns(Task.CompletedTask);
        vendaRepo.Setup(r => r.AtualizarAsync(It.IsAny<PedidoVenda>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // TMS lança exceção (serviço offline)
        tmsClient.Setup(t => t.CriarOrdemDeEntregaAsync(It.IsAny<CriarOrdemTmsDto>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new HttpRequestException("TMS indisponível"));

        var handler = new FaturarPedidoVendaHandler(vendaRepo.Object, estoqueRepo.Object, uow.Object, tmsClient.Object);

        // Act
        var result = await handler.Handle(new FaturarPedidoVendaCommand(1), CancellationToken.None);

        // Aguarda fire-and-forget concluir (mesmo que com falha)
        await Task.Delay(200);

        // Assert — pedido foi confirmado mesmo com TMS offline
        Assert.True(result.Sucesso);
        Assert.Equal("Faturado", result.Valor!.Status);

        // Assert — commit foi feito antes do TMS falhar
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
