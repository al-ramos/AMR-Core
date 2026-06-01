using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.PedidosCompra.Commands;
using AMR.Core.Application.PedidosCompra.Queries;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoCompraController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPedidosCompraQuery(empresaId, status), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterPedidoCompraQuery(id), ct);
        return result.Sucesso ? Ok(result.Valor) : NotFound(result.Erro);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoCompraCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor)
            : BadRequest(result.Erro);
    }

    [HttpPatch("{id:int}/aprovar")]
    public async Task<IActionResult> Aprovar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new AprovarPedidoCompraCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    [HttpPatch("{id:int}/receber")]
    public async Task<IActionResult> Receber(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ReceberPedidoCompraCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
