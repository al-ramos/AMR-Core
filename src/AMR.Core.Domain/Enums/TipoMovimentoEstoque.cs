namespace AMR.Core.Domain.Enums;

public enum TipoMovimentoEstoque
{
    Entrada      = 1,   // recebimento de compra
    Saida        = 2,   // faturamento de venda
    AjusteManual = 3    // inventário, quebra, devolução
}
