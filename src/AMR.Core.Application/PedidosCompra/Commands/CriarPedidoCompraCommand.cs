using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Commands;

public record ItemPedidoCompraInput
{
    public int     ProdutoId     { get; init; }
    public decimal Quantidade    { get; init; }
    public decimal PrecoUnitario { get; init; }
}

public record CriarPedidoCompraCommand : IRequest<Result<PedidoCompraDto>>
{
    public int    EmpresaId    { get; init; }
    public int    FornecedorId { get; init; }
    public string? Observacao  { get; init; }
    public IReadOnlyList<ItemPedidoCompraInput> Itens { get; init; } = [];
}
