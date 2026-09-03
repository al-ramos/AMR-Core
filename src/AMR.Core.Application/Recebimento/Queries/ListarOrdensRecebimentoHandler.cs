using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Application.Recebimento.Commands;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Queries;

public class ListarOrdensRecebimentoHandler(IOrdemRecebimentoRepository ordemRepo)
    : IRequestHandler<ListarOrdensRecebimentoQuery, Result<IReadOnlyList<OrdemRecebimentoDto>>>
{
    public async Task<Result<IReadOnlyList<OrdemRecebimentoDto>>> Handle(ListarOrdensRecebimentoQuery _, CancellationToken ct)
    {
        var ordens = await ordemRepo.ListarAsync(ct);
        return Result.Ok<IReadOnlyList<OrdemRecebimentoDto>>(
            ordens.Select(IniciarRecebimentoHandler.ToDto).ToList().AsReadOnly());
    }
}
