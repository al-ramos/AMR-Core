using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Queries;

public record ObterPedidoVendaQuery(int PedidoId) : IRequest<Result<PedidoVendaDto>>;
