namespace AMR.Core.Domain.Entities;

public class ItemRecebimento
{
    public int      Id                 { get; private set; }
    public int      OrdemRecebimentoId { get; private set; }
    public int      ProdutoId          { get; private set; }
    public Produto? Produto            { get; private set; }
    public int?     LocalizacaoId      { get; private set; }
    public decimal  QntEsperada        { get; private set; }
    public decimal  QntRecebida        { get; private set; }

    protected ItemRecebimento() { }

    internal ItemRecebimento(int produtoId, decimal qntEsperada)
    {
        if (produtoId <= 0)   throw new ArgumentException("ProdutoId inválido.");
        if (qntEsperada <= 0) throw new ArgumentException("Quantidade esperada deve ser maior que zero.");

        ProdutoId   = produtoId;
        QntEsperada = qntEsperada;
        QntRecebida = 0;
    }

    public void Receber(decimal quantidade, int? localizacaoId = null)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade recebida deve ser maior que zero.");
        QntRecebida   = quantidade;
        LocalizacaoId = localizacaoId;
    }
}
