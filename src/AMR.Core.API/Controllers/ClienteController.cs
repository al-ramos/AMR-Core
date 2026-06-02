using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.Clientes.Queries;

namespace AMR.Core.API.Controllers;

/// <summary>Clientes cadastrados na empresa.</summary>
[ApiController]
[Route("api/[controller]")]
public class ClienteController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista clientes ativos da empresa.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarClientesQuery(empresaId), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
