using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.MovimentosEstoque.Queries;

public class ListarMovimentosEstoqueHandler(IMovimentoEstoqueRepository repo)
    : IRequestHandler<ListarMovimentosEstoqueQuery, Result<IReadOnlyList<MovimentoEstoqueDto>>>
{
    public async Task<Result<IReadOnlyList<MovimentoEstoqueDto>>> Handle(
        ListarMovimentosEstoqueQuery q, CancellationToken ct)
    {
        var movimentos = await repo.ListarAsync(q.EmpresaId, q.ProdutoId, q.Tipo, ct);

        var dtos = movimentos
            .Select(m => new MovimentoEstoqueDto(
                m.Id,
                m.ProdutoId,
                m.Produto?.Nome ?? string.Empty,
                m.Tipo.ToString(),
                m.Quantidade,
                m.Origem,
                m.DataHora))
            .ToList();

        return Result.Ok<IReadOnlyList<MovimentoEstoqueDto>>(dtos);
    }
}
