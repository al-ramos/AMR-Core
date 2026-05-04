namespace AMR.Core.Domain.Entities;

public class ItemPedidoVenda
{
    public int      Id             { get; private set; }
    public int      PedidoVendaId  { get; private set; }
    public int      ProdutoId      { get; private set; }
    public Produto? Produto        { get; private set; }
    public decimal  Quantidade     { get; private set; }
    public decimal  PrecoUnitario  { get; private set; }
    public decimal  Desconto       { get; private set; }  // percentual 0-100
    public decimal  Total          => Quantidade * PrecoUnitario * (1 - Desconto / 100);

    protected ItemPedidoVenda() { }

    internal ItemPedidoVenda(int produtoId, decimal quantidade, decimal precoUnitario, decimal desconto = 0)
    {
        if (produtoId <= 0)    throw new ArgumentException("ProdutoId inválido.");
        if (quantidade <= 0)   throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (precoUnitario < 0) throw new ArgumentException("Preço não pode ser negativo.");
        if (desconto is < 0 or > 100) throw new ArgumentException("Desconto deve estar entre 0 e 100.");

        ProdutoId     = produtoId;
        Quantidade    = quantidade;
        PrecoUnitario = precoUnitario;
        Desconto      = desconto;
    }
}
