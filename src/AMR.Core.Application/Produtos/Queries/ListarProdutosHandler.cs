using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Queries;

public class ListarProdutosHandler(IProdutoRepository repo)
    : IRequestHandler<ListarProdutosQuery, Result<IReadOnlyList<ProdutoDto>>>
{
    public async Task<Result<IReadOnlyList<ProdutoDto>>> Handle(ListarProdutosQuery _, CancellationToken ct)
    {
        var produtos = await repo.ListarTodosAsync(ct);
        var dtos = produtos.Select(ToDto).ToList().AsReadOnly();
        return Result.Ok<IReadOnlyList<ProdutoDto>>(dtos);
    }

    private static ProdutoDto ToDto(Produto p) => new(
        p.Id, p.SKU, p.Nome, p.Descricao, p.PrecoUnitario,
        p.EstoqueMinimo, p.UnidadeMedida?.Sigla ?? "", p.Ativo);
}
