using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Commands;

public record ReativarProdutoCommand(int Id) : IRequest<Result<ProdutoDto>>;
