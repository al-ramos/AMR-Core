using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosCompra.Queries;

public record ListarPedidosCompraQuery(int EmpresaId, string? Status = null)
    : IRequest<Result<IReadOnlyList<PedidoCompraDto>>>;
