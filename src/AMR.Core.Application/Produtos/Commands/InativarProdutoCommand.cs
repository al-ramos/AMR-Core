using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Commands;

public record InativarProdutoCommand(int Id) : IRequest<Result<ProdutoDto>>;
