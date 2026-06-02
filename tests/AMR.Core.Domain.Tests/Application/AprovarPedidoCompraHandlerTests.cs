using AMR.Core.Application.PedidosCompra.Commands;
using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Tests.Application.Fakes;

namespace AMR.Core.Domain.Tests.Application;

public class AprovarPedidoCompraHandlerTests
{
    private static (AprovarPedidoCompraHandler handler, FakePedidoCompraRepository repo, FakeUnitOfWork uow) Setup()
    {
        var repo = new FakePedidoCompraRepository();
        var uow  = new FakeUnitOfWork();
        return (new AprovarPedidoCompraHandler(repo, uow), repo, uow);
    }

    private static async Task<PedidoCompra> SeedPedido(FakePedidoCompraRepository repo)
    {
        var pedido = PedidoCompra.Criar(1, 1, null);
        pedido.AdicionarItem(1, 10, 5.00m);
        await repo.AdicionarAsync(pedido);
        return pedido;
    }

    [Fact]
    public async Task Aprovar_PedidoRascunho_RetornaPedidoAprovado()
    {
        var (handler, repo, _) = Setup();
        var pedido = await SeedPedido(repo);

        var result = await handler.Handle(new AprovarPedidoCompraCommand(pedido.Id), default);

        Assert.True(result.Sucesso);
        Assert.Equal("Aprovado", result.Valor!.Status);
    }

    [Fact]
    public async Task Aprovar_PedidoInexistente_RetornaFalha()
    {
        var (handler, _, _) = Setup();

        var result = await handler.Handle(new AprovarPedidoCompraCommand(999), default);

        Assert.False(result.Sucesso);
        Assert.Contains("999", result.Erro);
    }

    [Fact]
    public async Task Aprovar_PedidoJaAprovado_RetornaFalha()
    {
        var (handler, repo, _) = Setup();
        var pedido = await SeedPedido(repo);
        await handler.Handle(new AprovarPedidoCompraCommand(pedido.Id), default);

        var result = await handler.Handle(new AprovarPedidoCompraCommand(pedido.Id), default);

        Assert.False(result.Sucesso);
    }

    [Fact]
    public async Task Aprovar_PedidoValido_CommitaUmaVez()
    {
        var (handler, repo, uow) = Setup();
        var pedido = await SeedPedido(repo);

        await handler.Handle(new AprovarPedidoCompraCommand(pedido.Id), default);

        Assert.Equal(1, uow.CommitCount);
    }
}
