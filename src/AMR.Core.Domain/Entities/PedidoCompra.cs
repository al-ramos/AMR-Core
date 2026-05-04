using AMR.Core.Domain.Enums;

namespace AMR.Core.Domain.Entities;

public class PedidoCompra
{
    private readonly List<ItemPedidoCompra> _itens = [];

    public int                      Id           { get; private set; }
    public int                      EmpresaId    { get; private set; }
    public int                      FornecedorId { get; private set; }
    public Fornecedor?              Fornecedor   { get; private set; }
    public StatusPedidoCompra       Status       { get; private set; }
    public DateTime                 DataEmissao  { get; private set; }
    public DateTime?                DataAprovacao { get; private set; }
    public DateTime?                DataRecebimento { get; private set; }
    public string?                  Observacao   { get; private set; }
    public IReadOnlyList<ItemPedidoCompra> Itens => _itens.AsReadOnly();
    public decimal                  Total        => _itens.Sum(i => i.Total);

    protected PedidoCompra() { }

    private PedidoCompra(int empresaId, int fornecedorId, string? observacao)
    {
        if (empresaId <= 0)    throw new ArgumentException("EmpresaId inválido.");
        if (fornecedorId <= 0) throw new ArgumentException("FornecedorId inválido.");

        EmpresaId    = empresaId;
        FornecedorId = fornecedorId;
        Observacao   = observacao?.Trim();
        Status       = StatusPedidoCompra.Rascunho;
        DataEmissao  = DateTime.UtcNow;
    }

    public static PedidoCompra Criar(int empresaId, int fornecedorId, string? observacao = null)
        => new(empresaId, fornecedorId, observacao);

    // ── Itens ───────────────────────────────────────────────────────────────

    public void AdicionarItem(int produtoId, decimal quantidade, decimal precoUnitario)
    {
        GarantirStatus(StatusPedidoCompra.Rascunho, "adicionar itens");
        _itens.Add(new ItemPedidoCompra(produtoId, quantidade, precoUnitario));
    }

    public void RemoverItem(int produtoId)
    {
        GarantirStatus(StatusPedidoCompra.Rascunho, "remover itens");
        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId)
            ?? throw new InvalidOperationException($"Produto {produtoId} não encontrado no pedido.");
        _itens.Remove(item);
    }

    // ── State machine ────────────────────────────────────────────────────────

    public void Aprovar()
    {
        GarantirStatus(StatusPedidoCompra.Rascunho, "aprovar");
        if (!_itens.Any()) throw new InvalidOperationException("Pedido sem itens não pode ser aprovado.");

        Status        = StatusPedidoCompra.Aprovado;
        DataAprovacao = DateTime.UtcNow;
    }

    public void Receber()
    {
        GarantirStatus(StatusPedidoCompra.Aprovado, "receber");

        Status          = StatusPedidoCompra.Recebido;
        DataRecebimento = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        if (Status == StatusPedidoCompra.Recebido)
            throw new InvalidOperationException("Pedido já recebido não pode ser cancelado.");
        if (Status == StatusPedidoCompra.Cancelado)
            throw new InvalidOperationException("Pedido já está cancelado.");

        Status = StatusPedidoCompra.Cancelado;
    }

    private void GarantirStatus(StatusPedidoCompra esperado, string acao)
    {
        if (Status != esperado)
            throw new InvalidOperationException(
                $"Não é possível {acao} com status '{Status}'. Esperado: '{esperado}'.");
    }

    public override string ToString() =>
        $"PC#{Id} | Fornecedor:{FornecedorId} | {Status} | Total: R$ {Total:N2}";
}
