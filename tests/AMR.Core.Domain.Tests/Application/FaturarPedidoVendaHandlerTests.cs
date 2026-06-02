using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;
using AMR.Core.Domain.Tests.Application.Fakes;

namespace AMR.Core.Domain.Tests.Application;

public class FaturarPedidoVendaHandlerTests
{
    private const int EmpresaId  = 1;
    private const int ClienteId  = 1;
    private const int ProdutoId  = 1;

    private static (FaturarPedidoVendaHandler handler, FakePedidoVendaRepository vendaRepo, FakeSaldoEstoqueRepository estoqueRepo, FakeUnitOfWork uow) Setup()
    {
        var vendaRepo   = new FakePedidoVendaRepository();
        var estoqueRepo = new FakeSaldoEstoqueRepository();
        var uow         = new FakeUnitOfWork();
        return (new FaturarPedidoVendaHandler(vendaRepo, estoqueRepo, uow), vendaRepo, estoqueRepo, uow);
    }

    private static SaldoEstoque SaldoCom(decimal quantidade)
    {
        var saldo = SaldoEstoque.Criar(ProdutoId, EmpresaId);
        saldo.Movimentar(TipoMovimentoEstoque.Entrada, quantidade, "seed");
        return saldo;
    }

    private static async Task<PedidoVenda> SeedPedidoAprovado(FakePedidoVendaRepository repo, decimal qtd = 5)
    {
        var pedido = PedidoVenda.Criar(EmpresaId, ClienteId, null);
        pedido.AdicionarItem(ProdutoId, qtd, 10.00m);
        pedido.Aprovar();
        await repo.AdicionarAsync(pedido);
        return pedido;
    }

    [Fact]
    public async Task Faturar_ComSaldoSuficiente_RetornaPedidoFaturado()
    {
        var (handler, repo, estoqueRepo, _) = Setup();
        var pedido = await SeedPedidoAprovado(repo, qtd: 5);
        estoqueRepo.Seed(SaldoCom(100));

        var result = await handler.Handle(new FaturarPedidoVendaCommand(pedido.Id), default);

        Assert.True(result.Sucesso);
        Assert.Equal("Faturado", result.Valor!.Status);
    }

    [Fact]
    public async Task Faturar_ComSaldoInsuficiente_RetornaFalha()
    {
        var (handler, repo, estoqueRepo, _) = Setup();
        var pedido = await SeedPedidoAprovado(repo, qtd: 50);
        estoqueRepo.Seed(SaldoCom(10));

        var result = await handler.Handle(new FaturarPedidoVendaCommand(pedido.Id), default);

        Assert.False(result.Sucesso);
        Assert.Contains("insuficiente", result.Erro, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Faturar_PedidoInexistente_RetornaFalha()
    {
        var (handler, _, _, _) = Setup();

        var result = await handler.Handle(new FaturarPedidoVendaCommand(999), default);

        Assert.False(result.Sucesso);
        Assert.Contains("999", result.Erro);
    }

    [Fact]
    public async Task Faturar_ComSaldoSuficiente_BaixaEstoque()
    {
        var (handler, repo, estoqueRepo, _) = Setup();
        var pedido = await SeedPedidoAprovado(repo, qtd: 5);
        var saldo  = SaldoCom(100);
        estoqueRepo.Seed(saldo);

        await handler.Handle(new FaturarPedidoVendaCommand(pedido.Id), default);

        Assert.Equal(1, estoqueRepo.Movimentos.Count);
        Assert.Equal(TipoMovimentoEstoque.Saida, estoqueRepo.Movimentos[0].Tipo);
        Assert.Equal(5, estoqueRepo.Movimentos[0].Quantidade);
    }

    [Fact]
    public async Task Faturar_ComSaldoSuficiente_CommitaUmaVez()
    {
        var (handler, repo, estoqueRepo, uow) = Setup();
        var pedido = await SeedPedidoAprovado(repo, qtd: 3);
        estoqueRepo.Seed(SaldoCom(50));

        await handler.Handle(new FaturarPedidoVendaCommand(pedido.Id), default);

        Assert.Equal(1, uow.CommitCount);
    }
}
