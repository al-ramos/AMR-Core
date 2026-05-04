using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Queries;

public record ObterPedidoCompraQuery(int PedidoId) : IRequest<Result<PedidoCompraDto>>;
