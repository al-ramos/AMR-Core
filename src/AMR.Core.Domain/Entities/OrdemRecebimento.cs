using AMR.Core.Domain.Enums;

namespace AMR.Core.Domain.Entities;

public class OrdemRecebimento
{
    private readonly List<ItemRecebimento> _itens = [];

    public int                              Id              { get; private set; }
    public int                              PedidoCompraId  { get; private set; }
    public PedidoCompra?                   PedidoCompra    { get; private set; }
    public OrdemRecebimentoStatus          Status          { get; private set; }
    public DateTime                        DataCriacao     { get; private set; }
    public DateTime?                       DataRecebimento { get; private set; }
    public IReadOnlyList<ItemRecebimento>  Itens           => _itens.AsReadOnly();

    protected OrdemRecebimento() { }

    public static OrdemRecebimento Criar(int pedidoCompraId, IEnumerable<(int produtoId, decimal qntEsperada)> itens)
    {
        if (pedidoCompraId <= 0) throw new ArgumentException("PedidoCompraId inválido.");

        var ordem = new OrdemRecebimento
        {
            PedidoCompraId = pedidoCompraId,
            Status         = OrdemRecebimentoStatus.Aguardando,
            DataCriacao    = DateTime.UtcNow,
        };

        foreach (var (produtoId, qnt) in itens)
            ordem._itens.Add(new ItemRecebimento(produtoId, qnt));

        return ordem;
    }

    public void IniciarRecebimento()
    {
        if (Status != OrdemRecebimentoStatus.Aguardando)
            throw new InvalidOperationException($"Não é possível iniciar com status '{Status}'.");
        Status = OrdemRecebimentoStatus.Recebendo;
    }

    public void ReceberItem(int itemId, decimal quantidade, int? localizacaoId = null)
    {
        if (Status != OrdemRecebimentoStatus.Recebendo)
            throw new InvalidOperationException("Ordem deve estar no status Recebendo para registrar itens.");

        var item = _itens.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException($"Item {itemId} não encontrado na ordem.");

        item.Receber(quantidade, localizacaoId);
    }

    public void Concluir()
    {
        if (Status != OrdemRecebimentoStatus.Recebendo)
            throw new InvalidOperationException("Ordem deve estar no status Recebendo para ser concluída.");

        Status          = OrdemRecebimentoStatus.Concluido;
        DataRecebimento = DateTime.UtcNow;
    }
}
