namespace AMR.Core.Application.DTOs;

public record SugerirPedidoCompraDto(
    string ProdutoId,
    string NomeProduto,
    float  QuantidadeSugerida,
    string Unidade
);
