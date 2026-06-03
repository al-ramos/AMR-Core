namespace AMR.Core.Application.DTOs;

public record MovimentoEstoqueDto(
    int      Id,
    int      ProdutoId,
    string   ProdutoNome,
    string   Tipo,
    decimal  Quantidade,
    string?  Origem,
    DateTime DataHora
);
