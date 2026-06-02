using AMR.Core.Application.Produtos.Commands;
using AMR.Core.Domain.Tests.Application.Fakes;

namespace AMR.Core.Domain.Tests.Application;

public class CriarProdutoHandlerTests
{
    private static CriarProdutoHandler CriarHandler(out FakeProdutoRepository repo, out FakeUnitOfWork uow)
    {
        repo = new FakeProdutoRepository();
        uow  = new FakeUnitOfWork();
        return new CriarProdutoHandler(repo, uow);
    }

    [Fact]
    public async Task Criar_ComDadosValidos_RetornaProdutoDto()
    {
        var handler = CriarHandler(out _, out _);
        var cmd = new CriarProdutoCommand
        {
            SKU = "TEST-001", Nome = "Produto Teste",
            PrecoUnitario = 10.50m, UnidadeMedidaId = 1, EstoqueMinimo = 5,
        };

        var result = await handler.Handle(cmd, default);

        Assert.True(result.Sucesso);
        Assert.Equal("TEST-001", result.Valor!.SKU);
        Assert.Equal("Produto Teste", result.Valor.Nome);
        Assert.Equal(10.50m, result.Valor.PrecoUnitario);
    }

    [Fact]
    public async Task Criar_ComDadosValidos_CommitaUmaVez()
    {
        var handler = CriarHandler(out _, out var uow);
        var cmd = new CriarProdutoCommand
        {
            SKU = "TEST-002", Nome = "Produto B",
            PrecoUnitario = 5m, UnidadeMedidaId = 1,
        };

        await handler.Handle(cmd, default);

        Assert.Equal(1, uow.CommitCount);
    }

    [Fact]
    public async Task Criar_ComSkuDuplicado_RetornaFalha()
    {
        var handler = CriarHandler(out _, out _);
        var cmd = new CriarProdutoCommand
        {
            SKU = "DUP-001", Nome = "Primeiro",
            PrecoUnitario = 1m, UnidadeMedidaId = 1,
        };
        await handler.Handle(cmd, default);

        var result = await handler.Handle(cmd with { Nome = "Segundo" }, default);

        Assert.False(result.Sucesso);
        Assert.Contains("DUP-001", result.Erro);
    }

    [Fact]
    public async Task Criar_SkuNormalizado_RetornaMaiusculo()
    {
        var handler = CriarHandler(out _, out _);
        var cmd = new CriarProdutoCommand
        {
            SKU = "lowercase-sku", Nome = "Teste",
            PrecoUnitario = 1m, UnidadeMedidaId = 1,
        };

        var result = await handler.Handle(cmd, default);

        Assert.True(result.Sucesso);
        Assert.Equal("LOWERCASE-SKU", result.Valor!.SKU);
    }
}
