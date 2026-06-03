namespace AMR.Core.Domain.Entities;

public class Produto
{
    public int           Id             { get; private set; }
    public string        SKU            { get; private set; }
    public string        Nome           { get; private set; }
    public string?       Descricao      { get; private set; }
    public decimal       PrecoUnitario  { get; private set; }
    public decimal       EstoqueMinimo  { get; private set; }
    public int           UnidadeMedidaId { get; private set; }
    public UnidadeMedida? UnidadeMedida  { get; private set; }
    public bool          Ativo          { get; private set; }
    public DateTime      DataCadastro   { get; private set; }
    public DateTime?     DataAtualizacao { get; private set; }

    protected Produto() { SKU = null!; Nome = null!; }

    private Produto(string sku, string nome, string? descricao,
                    decimal precoUnitario, decimal estoqueMinimo, int unidadeMedidaId)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU é obrigatório.", nameof(sku));
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (precoUnitario < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(precoUnitario));
        if (estoqueMinimo < 0)
            throw new ArgumentException("Estoque mínimo não pode ser negativo.", nameof(estoqueMinimo));
        if (unidadeMedidaId <= 0)
            throw new ArgumentException("UnidadeMedidaId inválido.", nameof(unidadeMedidaId));

        SKU             = sku.Trim().ToUpper();
        Nome            = nome.Trim();
        Descricao       = descricao?.Trim();
        PrecoUnitario   = precoUnitario;
        EstoqueMinimo   = estoqueMinimo;
        UnidadeMedidaId = unidadeMedidaId;
        Ativo           = true;
        DataCadastro    = DateTime.UtcNow;
    }

    public static Produto Criar(string sku, string nome, decimal precoUnitario,
                                int unidadeMedidaId, string? descricao = null,
                                decimal estoqueMinimo = 0)
        => new(sku, nome, descricao, precoUnitario, estoqueMinimo, unidadeMedidaId);

    public void Atualizar(string nome, string? descricao, decimal precoUnitario, decimal estoqueMinimo, int unidadeMedidaId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
        if (precoUnitario < 0)
            throw new ArgumentException("Preço não pode ser negativo.", nameof(precoUnitario));
        if (estoqueMinimo < 0)
            throw new ArgumentException("Estoque mínimo não pode ser negativo.", nameof(estoqueMinimo));
        if (unidadeMedidaId <= 0)
            throw new ArgumentException("UnidadeMedidaId inválido.", nameof(unidadeMedidaId));

        Nome            = nome.Trim();
        Descricao       = descricao?.Trim();
        PrecoUnitario   = precoUnitario;
        EstoqueMinimo   = estoqueMinimo;
        UnidadeMedidaId = unidadeMedidaId;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco < 0) throw new ArgumentException("Preço não pode ser negativo.");
        PrecoUnitario   = novoPreco;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Inativar() { Ativo = false; DataAtualizacao = DateTime.UtcNow; }
    public void Reativar() { Ativo = true;  DataAtualizacao = DateTime.UtcNow; }

    public override string ToString() => $"[{SKU}] {Nome} — R$ {PrecoUnitario:N2}";
}
