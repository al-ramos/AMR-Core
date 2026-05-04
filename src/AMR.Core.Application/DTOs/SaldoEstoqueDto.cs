namespace AMR.Core.Application.DTOs;

public record SaldoEstoqueDto(
    int     ProdutoId,
    string  ProdutoNome,
    string  SKU,
    decimal Quantidade,
    string  UnidadeMedida,
    bool    AbaixoDoMinimo
);
