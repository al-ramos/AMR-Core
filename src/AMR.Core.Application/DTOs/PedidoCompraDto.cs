namespace AMR.Core.Application.DTOs;

public record PedidoCompraDto(
    int                    Id,
    int                    EmpresaId,
    int                    FornecedorId,
    string                 Status,
    DateTime               DataEmissao,
    DateTime?              DataAprovacao,
    DateTime?              DataRecebimento,
    string?                Observacao,
    decimal                Total,
    IReadOnlyList<ItemPedidoDto> Itens
);
