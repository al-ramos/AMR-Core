using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.PedidosCompra.Commands;
using AMR.Core.Application.PedidosCompra.Queries;
using AMR.Core.API.Extensions;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class PedidoCompraController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPedidosCompraQuery(empresaId, status), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterPedidoCompraQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoCompraCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor)
            : result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/aprovar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Aprovar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new AprovarPedidoCompraCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/receber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Receber(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ReceberPedidoCompraCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelarPedidoCompraCommand(id), ct);
        return result.ToActionResult(this);
    }
}
