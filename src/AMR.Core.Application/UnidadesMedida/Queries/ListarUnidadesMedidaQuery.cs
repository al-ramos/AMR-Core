using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.UnidadesMedida.Queries;

public record ListarUnidadesMedidaQuery : IRequest<Result<IReadOnlyList<UnidadeMedidaDto>>>;

public class ListarUnidadesMedidaHandler(IUnidadeMedidaRepository repo)
    : IRequestHandler<ListarUnidadesMedidaQuery, Result<IReadOnlyList<UnidadeMedidaDto>>>
{
    public async Task<Result<IReadOnlyList<UnidadeMedidaDto>>> Handle(ListarUnidadesMedidaQuery _, CancellationToken ct)
    {
        var lista = await repo.ListarAsync(ct);
        var dtos  = lista.Where(u => u.Ativo)
                         .Select(u => new UnidadeMedidaDto(u.Id, u.Sigla, u.Descricao))
                         .ToList().AsReadOnly();
        return Result.Ok<IReadOnlyList<UnidadeMedidaDto>>(dtos);
    }
}
