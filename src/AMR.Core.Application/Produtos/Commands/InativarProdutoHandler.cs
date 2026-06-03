using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Commands;

public class InativarProdutoHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<InativarProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(InativarProdutoCommand cmd, CancellationToken ct)
    {
        var produto = await repo.ObterPorIdAsync(cmd.Id, ct);
        if (produto is null)
            return Result.Falha<ProdutoDto>($"Produto {cmd.Id} não encontrado.");

        produto.Inativar();
        await repo.AtualizarAsync(produto, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(AtualizarProdutoHandler.ToDto(produto));
    }
}
