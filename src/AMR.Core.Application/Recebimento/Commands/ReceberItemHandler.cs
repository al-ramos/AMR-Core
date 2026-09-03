using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Commands;

public class ReceberItemHandler(IOrdemRecebimentoRepository ordemRepo, IUnitOfWork uow)
    : IRequestHandler<ReceberItemCommand, Result<OrdemRecebimentoDto>>
{
    public async Task<Result<OrdemRecebimentoDto>> Handle(ReceberItemCommand cmd, CancellationToken ct)
    {
        var ordem = await ordemRepo.ObterPorIdAsync(cmd.OrdemId, ct);
        if (ordem is null)
            return Result.Falha<OrdemRecebimentoDto>($"Ordem de recebimento #{cmd.OrdemId} não encontrada.");

        try { ordem.ReceberItem(cmd.ItemId, cmd.Quantidade, cmd.LocalizacaoId); }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException or ArgumentException)
        { return Result.Falha<OrdemRecebimentoDto>(ex.Message); }

        await ordemRepo.AtualizarAsync(ordem, ct);
        await uow.CommitAsync(ct);

        return Result.Ok(IniciarRecebimentoHandler.ToDto(ordem));
    }
}
