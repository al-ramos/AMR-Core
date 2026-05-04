using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.PedidosVenda.Commands;

public record AprovarPedidoVendaCommand(int PedidoId) : IRequest<Result<PedidoVendaDto>>;
