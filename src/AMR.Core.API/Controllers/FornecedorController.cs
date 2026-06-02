using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.Fornecedores.Queries;

namespace AMR.Core.API.Controllers;

/// <summary>Fornecedores cadastrados na empresa.</summary>
[ApiController]
[Route("api/[controller]")]
public class FornecedorController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista fornecedores ativos da empresa.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarFornecedoresQuery(empresaId), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
