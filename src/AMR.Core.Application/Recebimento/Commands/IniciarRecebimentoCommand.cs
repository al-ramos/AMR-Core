using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Commands;

public record IniciarRecebimentoCommand(int PedidoCompraId) : IRequest<Result<OrdemRecebimentoDto>>;
