using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Queries;

public record ListarProdutosQuery() : IRequest<Result<IReadOnlyList<ProdutoDto>>>;
