using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Commands;

public record ReceberItemCommand(
    int     OrdemId,
    int     ItemId,
    decimal Quantidade,
    int?    LocalizacaoId = null
) : IRequest<Result<OrdemRecebimentoDto>>;
