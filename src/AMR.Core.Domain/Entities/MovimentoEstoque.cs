using AMR.Core.Domain.Enums;

namespace AMR.Core.Domain.Entities;

public class MovimentoEstoque
{
    public int                    Id          { get; private set; }
    public int                    ProdutoId   { get; private set; }
    public Produto?               Produto     { get; private set; }
    public int                    EmpresaId   { get; private set; }
    public TipoMovimentoEstoque   Tipo        { get; private set; }
    public decimal                Quantidade  { get; private set; }  // sempre positivo
    public string?                Origem      { get; private set; }  // "PC#10", "PV#5", "AJUSTE"
    public DateTime               DataHora    { get; private set; }

    protected MovimentoEstoque() { }

    private MovimentoEstoque(int produtoId, int empresaId,
                             TipoMovimentoEstoque tipo, decimal quantidade, string? origem)
    {
        if (produtoId <= 0)   throw new ArgumentException("ProdutoId inválido.");
        if (empresaId <= 0)   throw new ArgumentException("EmpresaId inválido.");
        if (quantidade <= 0)  throw new ArgumentException("Quantidade deve ser maior que zero.");

        ProdutoId  = produtoId;
        EmpresaId  = empresaId;
        Tipo       = tipo;
        Quantidade = quantidade;
        Origem     = origem;
        DataHora   = DateTime.UtcNow;
    }

    public static MovimentoEstoque CriarEntrada(int produtoId, int empresaId, decimal qtd, string origem)
        => new(produtoId, empresaId, TipoMovimentoEstoque.Entrada, qtd, origem);

    public static MovimentoEstoque CriarSaida(int produtoId, int empresaId, decimal qtd, string origem)
        => new(produtoId, empresaId, TipoMovimentoEstoque.Saida, qtd, origem);

    public static MovimentoEstoque CriarAjuste(int produtoId, int empresaId, decimal qtd, string motivo)
        => new(produtoId, empresaId, TipoMovimentoEstoque.AjusteManual, qtd, motivo);
}
