using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Domain.Entities;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Produtos.Commands;

public class CriarProdutoHandler(IProdutoRepository repo, IUnitOfWork uow)
    : IRequestHandler<CriarProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(CriarProdutoCommand cmd, CancellationToken ct)
    {
        if (await repo.ExisteSkuAsync(cmd.SKU, ct: ct))
            return Result.Falha<ProdutoDto>($"Já existe um produto com SKU '{cmd.SKU}'.");

        var produto = Produto.Criar(
            cmd.SKU, cmd.Nome, cmd.PrecoUnitario,
            cmd.UnidadeMedidaId, cmd.Descricao, cmd.EstoqueMinimo);

        await repo.AdicionarAsync(produto, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(ToDto(produto));
    }

    private static ProdutoDto ToDto(Produto p) => new(
        p.Id, p.SKU, p.Nome, p.Descricao, p.PrecoUnitario,
        p.EstoqueMinimo, p.UnidadeMedida?.Sigla ?? "", p.Ativo);
}
