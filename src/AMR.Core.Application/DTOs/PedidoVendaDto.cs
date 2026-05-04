namespace AMR.Core.Application.DTOs;

public record PedidoVendaDto(
    int                    Id,
    int                    EmpresaId,
    int                    ClienteId,
    string                 Status,
    DateTime               DataEmissao,
    DateTime?              DataAprovacao,
    DateTime?              DataFaturamento,
    string?                Observacao,
    decimal                Total,
    IReadOnlyList<ItemPedidoDto> Itens
);
