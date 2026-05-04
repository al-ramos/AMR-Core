namespace AMR.Core.Domain.Entities;

public class ItemPedidoCompra
{
    public int      Id              { get; private set; }
    public int      PedidoCompraId  { get; private set; }
    public int      ProdutoId       { get; private set; }
    public Produto? Produto         { get; private set; }
    public decimal  Quantidade      { get; private set; }
    public decimal  PrecoUnitario   { get; private set; }
    public decimal  Total           => Quantidade * PrecoUnitario;

    protected ItemPedidoCompra() { }

    internal ItemPedidoCompra(int produtoId, decimal quantidade, decimal precoUnitario)
    {
        if (produtoId <= 0)    throw new ArgumentException("ProdutoId inválido.");
        if (quantidade <= 0)   throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (precoUnitario < 0) throw new ArgumentException("Preço não pode ser negativo.");

        ProdutoId     = produtoId;
        Quantidade    = quantidade;
        PrecoUnitario = precoUnitario;
    }
}
