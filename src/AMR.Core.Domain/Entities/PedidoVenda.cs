using AMR.Core.Domain.Enums;

namespace AMR.Core.Domain.Entities;

public class PedidoVenda
{
    private readonly List<ItemPedidoVenda> _itens = [];

    public int                     Id           { get; private set; }
    public int                     EmpresaId    { get; private set; }
    public int                     ClienteId    { get; private set; }
    public Cliente?                Cliente      { get; private set; }
    public StatusPedidoVenda       Status       { get; private set; }
    public DateTime                DataEmissao  { get; private set; }
    public DateTime?               DataAprovacao { get; private set; }
    public DateTime?               DataFaturamento { get; private set; }
    public string?                 Observacao   { get; private set; }
    public IReadOnlyList<ItemPedidoVenda> Itens => _itens.AsReadOnly();
    public decimal                 Total        => _itens.Sum(i => i.Total);

    protected PedidoVenda() { }

    private PedidoVenda(int empresaId, int clienteId, string? observacao)
    {
        if (empresaId <= 0)  throw new ArgumentException("EmpresaId inválido.");
        if (clienteId <= 0)  throw new ArgumentException("ClienteId inválido.");

        EmpresaId   = empresaId;
        ClienteId   = clienteId;
        Observacao  = observacao?.Trim();
        Status      = StatusPedidoVenda.Aberto;
        DataEmissao = DateTime.UtcNow;
    }

    public static PedidoVenda Criar(int empresaId, int clienteId, string? observacao = null)
        => new(empresaId, clienteId, observacao);

    // ── Itens ───────────────────────────────────────────────────────────────

    public void AdicionarItem(int produtoId, decimal quantidade, decimal precoUnitario, decimal desconto = 0)
    {
        GarantirStatus(StatusPedidoVenda.Aberto, "adicionar itens");
        _itens.Add(new ItemPedidoVenda(produtoId, quantidade, precoUnitario, desconto));
    }

    public void RemoverItem(int produtoId)
    {
        GarantirStatus(StatusPedidoVenda.Aberto, "remover itens");
        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId)
            ?? throw new InvalidOperationException($"Produto {produtoId} não encontrado no pedido.");
        _itens.Remove(item);
    }

    // ── State machine ────────────────────────────────────────────────────────

    public void Aprovar()
    {
        GarantirStatus(StatusPedidoVenda.Aberto, "aprovar");
        if (!_itens.Any()) throw new InvalidOperationException("Pedido sem itens não pode ser aprovado.");

        Status        = StatusPedidoVenda.Aprovado;
        DataAprovacao = DateTime.UtcNow;
    }

    public void Faturar()
    {
        GarantirStatus(StatusPedidoVenda.Aprovado, "faturar");

        Status          = StatusPedidoVenda.Faturado;
        DataFaturamento = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        if (Status == StatusPedidoVenda.Faturado)
            throw new InvalidOperationException("Pedido já faturado não pode ser cancelado.");
        if (Status == StatusPedidoVenda.Cancelado)
            throw new InvalidOperationException("Pedido já está cancelado.");

        Status = StatusPedidoVenda.Cancelado;
    }

    private void GarantirStatus(StatusPedidoVenda esperado, string acao)
    {
        if (Status != esperado)
            throw new InvalidOperationException(
                $"Não é possível {acao} com status '{Status}'. Esperado: '{esperado}'.");
    }

    public override string ToString() =>
        $"PV#{Id} | Cliente:{ClienteId} | {Status} | Total: R$ {Total:N2}";
}
