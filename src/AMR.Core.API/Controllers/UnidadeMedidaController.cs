using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.UnidadesMedida.Queries;

namespace AMR.Core.API.Controllers;

/// <summary>Unidades de medida disponíveis no sistema.</summary>
[ApiController]
[Route("api/[controller]")]
public class UnidadeMedidaController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todas as unidades de medida ativas.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarUnidadesMedidaQuery(), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
