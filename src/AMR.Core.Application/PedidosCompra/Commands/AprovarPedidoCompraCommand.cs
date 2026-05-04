using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Commands;

public record AprovarPedidoCompraCommand(int PedidoId) : IRequest<Result<PedidoCompraDto>>;
