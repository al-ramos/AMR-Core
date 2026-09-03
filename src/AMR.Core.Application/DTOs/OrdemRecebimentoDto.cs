namespace AMR.Core.Application.DTOs;

public record ItemRecebimentoDto(
    int     Id,
    int     ProdutoId,
    string  NomeProduto,
    int?    LocalizacaoId,
    decimal QntEsperada,
    decimal QntRecebida
);

public record OrdemRecebimentoDto(
    int                              Id,
    int                              PedidoCompraId,
    string                           Status,
    DateTime                         DataCriacao,
    DateTime?                        DataRecebimento,
    IReadOnlyList<ItemRecebimentoDto> Itens
);
