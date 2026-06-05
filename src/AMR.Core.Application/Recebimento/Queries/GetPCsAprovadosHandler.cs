using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Application.PedidosCompra.Commands;
using AMR.Core.Domain.Enums;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Recebimento.Queries;

public class GetPCsAprovadosHandler(IPedidoCompraRepository compraRepo, IOrdemRecebimentoRepository ordemRepo)
    : IRequestHandler<GetPCsAprovadosQuery, Result<IReadOnlyList<PedidoCompraDto>>>
{
    public async Task<Result<IReadOnlyList<PedidoCompraDto>>> Handle(GetPCsAprovadosQuery _, CancellationToken ct)
    {
        var todos = await compraRepo.ListarPorEmpresaAsync(1, ct);
        var aprovados = todos.Where(p => p.Status == StatusPedidoCompra.Aprovado).ToList();

        var semOrdem = new List<PedidoCompraDto>();
        foreach (var pc in aprovados)
        {
            if (!await ordemRepo.ExistePorPedidoCompraAsync(pc.Id, ct))
                semOrdem.Add(CriarPedidoCompraHandler.ToDto(pc));
        }

        return Result.Ok<IReadOnlyList<PedidoCompraDto>>(semOrdem.AsReadOnly());
    }
}
