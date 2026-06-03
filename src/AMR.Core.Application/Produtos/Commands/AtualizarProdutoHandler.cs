using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Commands;

public class AtualizarProdutoHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<AtualizarProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(AtualizarProdutoCommand cmd, CancellationToken ct)
    {
        var produto = await repo.ObterPorIdAsync(cmd.Id, ct);
        if (produto is null)
            return Result.Falha<ProdutoDto>($"Produto {cmd.Id} não encontrado.");

        try { produto.Atualizar(cmd.Nome, cmd.Descricao, cmd.PrecoUnitario, cmd.EstoqueMinimo, cmd.UnidadeMedidaId); }
        catch (ArgumentException ex) { return Result.Falha<ProdutoDto>(ex.Message); }

        await repo.AtualizarAsync(produto, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(ToDto(produto));
    }

    internal static ProdutoDto ToDto(Produto p) => new(
        p.Id, p.SKU, p.Nome, p.Descricao, p.PrecoUnitario,
        p.EstoqueMinimo, p.UnidadeMedida?.Sigla ?? "", p.Ativo);
}
