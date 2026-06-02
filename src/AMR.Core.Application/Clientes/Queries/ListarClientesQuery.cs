using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Clientes.Queries;

public record ListarClientesQuery(int EmpresaId) : IRequest<Result<IReadOnlyList<ClienteDto>>>;

public class ListarClientesHandler(IClienteRepository repo)
    : IRequestHandler<ListarClientesQuery, Result<IReadOnlyList<ClienteDto>>>
{
    public async Task<Result<IReadOnlyList<ClienteDto>>> Handle(ListarClientesQuery q, CancellationToken ct)
    {
        var lista = await repo.ListarAtivosAsync(q.EmpresaId, ct);
        var dtos  = lista.Select(c => new ClienteDto(c.Id, c.Nome, c.TipoDocumento))
                         .ToList().AsReadOnly();
        return Result.Ok<IReadOnlyList<ClienteDto>>(dtos);
    }
}
