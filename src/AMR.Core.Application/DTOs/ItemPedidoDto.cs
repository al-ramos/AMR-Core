namespace AMR.Core.Application.DTOs;

public record ItemPedidoDto(
    int     ProdutoId,
    string  ProdutoNome,
    decimal Quantidade,
    decimal PrecoUnitario,
    decimal Desconto,
    decimal Total
);
