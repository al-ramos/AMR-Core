using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Application.PedidosVenda.Queries;
using AMR.Core.API.Extensions;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class PedidoVendaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPedidosVendaQuery(empresaId, status), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterPedidoVendaQuery(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoVendaCommand cmd, CancellationToken ct)
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
        var result = await mediator.Send(new AprovarPedidoVendaCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/faturar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Faturar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new FaturarPedidoVendaCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelarPedidoVendaCommand(id), ct);
        return result.ToActionResult(this);
    }
}
