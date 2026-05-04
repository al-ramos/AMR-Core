using AMR.Core.Domain.Entities;
using AMR.Core.Domain.Enums;

namespace AMR.Core.Domain.Tests;

/// <summary>
/// Testa o ciclo completo de integração entre domínios:
/// Produto → PedidoCompra → Estoque sobe → PedidoVenda → Estoque cai
/// </summary>
public class CicloCompraVendaEstoqueTests
{
    // ── Helpers ─────────────────────────────────────────────────────────────

    private static UnidadeMedida CriarUnidade() =>
        UnidadeMedida.Criar("UN", "Unidade");

    private static Produto CriarProduto(decimal preco = 100m) =>
        Produto.Criar("SKU-001", "Produto Teste", preco, unidadeMedidaId: 1, estoqueMinimo: 5);

    // ── Produto ─────────────────────────────────────────────────────────────

    [Fact]
    public void Produto_Criar_DeveIniciarAtivo()
    {
        var produto = CriarProduto();
        Assert.True(produto.Ativo);
        Assert.Equal("SKU-001", produto.SKU);
        Assert.Equal(100m, produto.PrecoUnitario);
    }

    [Fact]
    public void Produto_SKU_DeveSerSempreUpperCase()
    {
        var produto = Produto.Criar("sku-abc", "Teste", 50m, 1);
        Assert.Equal("SKU-ABC", produto.SKU);
    }

    [Fact]
    public void Produto_AtualizarPreco_NegativoDeveLancarExcecao()
    {
        var produto = CriarProduto();
        Assert.Throws<ArgumentException>(() => produto.AtualizarPreco(-1));
    }

    // ── PedidoCompra ─────────────────────────────────────────────────────────

    [Fact]
    public void PedidoCompra_CriarEAprovar_DeveMudarStatus()
    {
        var pedido = PedidoCompra.Criar(empresaId: 1, fornecedorId: 1);
        Assert.Equal(StatusPedidoCompra.Rascunho, pedido.Status);

        pedido.AdicionarItem(produtoId: 1, quantidade: 10, precoUnitario: 50m);
        pedido.Aprovar();

        Assert.Equal(StatusPedidoCompra.Aprovado, pedido.Status);
        Assert.NotNull(pedido.DataAprovacao);
    }

    [Fact]
    public void PedidoCompra_AprovarSemItens_DeveLancarExcecao()
    {
        var pedido = PedidoCompra.Criar(1, 1);
        Assert.Throws<InvalidOperationException>(() => pedido.Aprovar());
    }

    [Fact]
    public void PedidoCompra_TotalDeveReflitirItens()
    {
        var pedido = PedidoCompra.Criar(1, 1);
        pedido.AdicionarItem(produtoId: 1, quantidade: 3, precoUnitario: 100m);
        pedido.AdicionarItem(produtoId: 2, quantidade: 2, precoUnitario: 50m);

        Assert.Equal(400m, pedido.Total); // 300 + 100
    }

    [Fact]
    public void PedidoCompra_NaoPodeAdicionarItemAposAprovacao()
    {
        var pedido = PedidoCompra.Criar(1, 1);
        pedido.AdicionarItem(1, 5, 10m);
        pedido.Aprovar();

        Assert.Throws<InvalidOperationException>(() =>
            pedido.AdicionarItem(2, 3, 20m));
    }

    // ── PedidoVenda ──────────────────────────────────────────────────────────

    [Fact]
    public void PedidoVenda_TotalComDesconto_DeveCalcularCorretamente()
    {
        var pedido = PedidoVenda.Criar(empresaId: 1, clienteId: 1);
        // 10 un × R$100 com 10% de desconto = R$900
        pedido.AdicionarItem(produtoId: 1, quantidade: 10, precoUnitario: 100m, desconto: 10m);

        Assert.Equal(900m, pedido.Total);
    }

    [Fact]
    public void PedidoVenda_CicloCompleto_DevePercorrerTodosStatus()
    {
        var pedido = PedidoVenda.Criar(1, 1);
        Assert.Equal(StatusPedidoVenda.Aberto, pedido.Status);

        pedido.AdicionarItem(1, 5, 200m);
        pedido.Aprovar();
        Assert.Equal(StatusPedidoVenda.Aprovado, pedido.Status);

        pedido.Faturar();
        Assert.Equal(StatusPedidoVenda.Faturado, pedido.Status);
    }

    // ── Estoque ──────────────────────────────────────────────────────────────

    [Fact]
    public void SaldoEstoque_EntradaPorCompra_DeveAumentarSaldo()
    {
        var saldo = SaldoEstoque.Criar(produtoId: 1, empresaId: 1);
        Assert.Equal(0, saldo.Quantidade);

        saldo.Movimentar(TipoMovimentoEstoque.Entrada, 50, "PC#1");
        Assert.Equal(50, saldo.Quantidade);
    }

    [Fact]
    public void SaldoEstoque_SaidaPorVenda_DeveReduzirSaldo()
    {
        var saldo = SaldoEstoque.Criar(1, 1);
        saldo.Movimentar(TipoMovimentoEstoque.Entrada, 50, "PC#1");

        saldo.Movimentar(TipoMovimentoEstoque.Saida, 20, "PV#1");
        Assert.Equal(30, saldo.Quantidade);
    }

    [Fact]
    public void SaldoEstoque_SaidaSemSaldo_DeveLancarExcecao()
    {
        var saldo = SaldoEstoque.Criar(1, 1);
        Assert.Throws<InvalidOperationException>(() =>
            saldo.Movimentar(TipoMovimentoEstoque.Saida, 10, "PV#1"));
    }

    // ── Ciclo completo integrado ─────────────────────────────────────────────

    [Fact]
    public void CicloCompleto_ComprarReceberVender_SaldoDeveEstarCorreto()
    {
        // 1. Criar produto
        var produto = CriarProduto(preco: 80m);

        // 2. Pedido de compra: 100 unidades
        var compra = PedidoCompra.Criar(empresaId: 1, fornecedorId: 1);
        compra.AdicionarItem(produtoId: 1, quantidade: 100, precoUnitario: 80m);
        compra.Aprovar();
        compra.Receber();
        Assert.Equal(StatusPedidoCompra.Recebido, compra.Status);

        // 3. Entrada no estoque
        var saldo = SaldoEstoque.Criar(produtoId: 1, empresaId: 1);
        saldo.Movimentar(TipoMovimentoEstoque.Entrada, 100, $"PC#{compra.Id}");
        Assert.Equal(100, saldo.Quantidade);

        // 4. Pedido de venda: 30 unidades
        var venda = PedidoVenda.Criar(empresaId: 1, clienteId: 1);
        venda.AdicionarItem(produtoId: 1, quantidade: 30, precoUnitario: 120m);
        venda.Aprovar();
        venda.Faturar();

        // 5. Saída do estoque
        saldo.Movimentar(TipoMovimentoEstoque.Saida, 30, $"PV#{venda.Id}");
        Assert.Equal(70, saldo.Quantidade); // 100 - 30

        // 6. Verificar margem
        decimal custoTotal  = compra.Total;         // 8000
        decimal receitaTotal = venda.Total;          // 3600
        decimal margem = receitaTotal - (30 * 80m); // 3600 - 2400 = 1200
        Assert.Equal(1200m, margem);
    }
}
