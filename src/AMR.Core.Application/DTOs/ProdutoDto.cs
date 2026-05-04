namespace AMR.Core.Application.DTOs;

public record ProdutoDto(
    int     Id,
    string  SKU,
    string  Nome,
    string? Descricao,
    decimal PrecoUnitario,
    decimal EstoqueMinimo,
    string  UnidadeMedida,
    bool    Ativo
);
