using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Commands;

public record CriarProdutoCommand : IRequest<Result<ProdutoDto>>
{
    public string  SKU            { get; init; } = string.Empty;
    public string  Nome           { get; init; } = string.Empty;
    public string? Descricao      { get; init; }
    public decimal PrecoUnitario  { get; init; }
    public int     UnidadeMedidaId { get; init; }
    public decimal EstoqueMinimo  { get; init; }
}
