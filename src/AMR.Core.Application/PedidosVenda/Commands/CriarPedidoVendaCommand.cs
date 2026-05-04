using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Commands;

public record ItemPedidoVendaInput
{
    public int     ProdutoId     { get; init; }
    public decimal Quantidade    { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal Desconto      { get; init; }
}

public record CriarPedidoVendaCommand : IRequest<Result<PedidoVendaDto>>
{
    public int     EmpresaId  { get; init; }
    public int     ClienteId  { get; init; }
    public string? Observacao { get; init; }
    public IReadOnlyList<ItemPedidoVendaInput> Itens { get; init; } = [];
}
