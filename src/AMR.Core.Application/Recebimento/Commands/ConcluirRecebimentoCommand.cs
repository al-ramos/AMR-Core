using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Commands;

public record ConcluirRecebimentoCommand(int OrdemId) : IRequest<Result<OrdemRecebimentoDto>>;
