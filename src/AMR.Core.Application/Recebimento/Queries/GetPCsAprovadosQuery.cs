using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Queries;

public record GetPCsAprovadosQuery : IRequest<Result<IReadOnlyList<PedidoCompraDto>>>;
