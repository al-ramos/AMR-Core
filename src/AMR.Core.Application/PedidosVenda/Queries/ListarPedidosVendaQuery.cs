using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Queries;

public record ListarPedidosVendaQuery(int EmpresaId, string? Status = null)
    : IRequest<Result<IReadOnlyList<PedidoVendaDto>>>;
