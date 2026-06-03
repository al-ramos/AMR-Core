using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.MovimentosEstoque.Queries;

public record ListarMovimentosEstoqueQuery(
    int     EmpresaId,
    int?    ProdutoId = null,
    string? Tipo      = null
) : IRequest<Result<IReadOnlyList<MovimentoEstoqueDto>>>;
