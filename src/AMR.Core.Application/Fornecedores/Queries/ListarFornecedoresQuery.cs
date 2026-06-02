using MediatR;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;
using AMR.Core.Shared.Results;

namespace AMR.Core.Application.Fornecedores.Queries;

public record ListarFornecedoresQuery(int EmpresaId) : IRequest<Result<IReadOnlyList<FornecedorDto>>>;

public class ListarFornecedoresHandler(IFornecedorRepository repo)
    : IRequestHandler<ListarFornecedoresQuery, Result<IReadOnlyList<FornecedorDto>>>
{
    public async Task<Result<IReadOnlyList<FornecedorDto>>> Handle(ListarFornecedoresQuery q, CancellationToken ct)
    {
        var lista = await repo.ListarAtivosAsync(q.EmpresaId, ct);
        var dtos  = lista.Select(f => new FornecedorDto(f.Id, f.NomeFantasia, f.Categoria))
                         .ToList().AsReadOnly();
        return Result.Ok<IReadOnlyList<FornecedorDto>>(dtos);
    }
}
