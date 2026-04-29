using MediatR;
using RDS.Core.Application.DTOs;
using RDS.Core.Shared.Results;

namespace RDS.Core.Application.PedidosVenda.Queries;

public record ListarPedidosVendaQuery(int EmpresaId, string? Status = null)
    : IRequest<Result<IReadOnlyList<PedidoVendaDto>>>;
