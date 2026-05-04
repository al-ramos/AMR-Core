using AMR.Core.Domain.Enums;

namespace AMR.Core.Domain.Entities;

public class SaldoEstoque
{
    public int     Id         { get; private set; }
    public int     ProdutoId  { get; private set; }
    public Produto? Produto   { get; private set; }
    public int     EmpresaId  { get; private set; }
    public decimal Quantidade { get; private set; }

    protected SaldoEstoque() { }

    private SaldoEstoque(int produtoId, int empresaId)
    {
        if (produtoId <= 0) throw new ArgumentException("ProdutoId inválido.");
        if (empresaId <= 0) throw new ArgumentException("EmpresaId inválido.");

        ProdutoId  = produtoId;
        EmpresaId  = empresaId;
        Quantidade = 0;
    }

    public static SaldoEstoque Criar(int produtoId, int empresaId)
        => new(produtoId, empresaId);

    /// <summary>Aplica um movimento e retorna o movimento gerado.</summary>
    public MovimentoEstoque Movimentar(TipoMovimentoEstoque tipo, decimal quantidade, string origem)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.");

        if (tipo == TipoMovimentoEstoque.Saida && quantidade > Quantidade)
            throw new InvalidOperationException(
                $"Saldo insuficiente. Disponível: {Quantidade}, Solicitado: {quantidade}.");

        Quantidade += tipo == TipoMovimentoEstoque.Saida ? -quantidade : quantidade;

        return tipo switch
        {
            TipoMovimentoEstoque.Entrada      => MovimentoEstoque.CriarEntrada(ProdutoId, EmpresaId, quantidade, origem),
            TipoMovimentoEstoque.Saida        => MovimentoEstoque.CriarSaida(ProdutoId, EmpresaId, quantidade, origem),
            TipoMovimentoEstoque.AjusteManual => MovimentoEstoque.CriarAjuste(ProdutoId, EmpresaId, quantidade, origem),
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
    }

    public bool EstoqueAbaixoDoMinimo(decimal minimo) => Quantidade < minimo;

    public override string ToString() =>
        $"Produto:{ProdutoId} | Empresa:{EmpresaId} | Saldo: {Quantidade}";
}
