using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Application.PedidosVenda.Queries;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoVendaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPedidosVendaQuery(empresaId, status), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterPedidoVendaQuery(id), ct);
        return result.Sucesso ? Ok(result.Valor) : NotFound(result.Erro);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoVendaCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor)
            : BadRequest(result.Erro);
    }

    [HttpPatch("{id:int}/aprovar")]
    public async Task<IActionResult> Aprovar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new AprovarPedidoVendaCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    [HttpPatch("{id:int}/faturar")]
    public async Task<IActionResult> Faturar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new FaturarPedidoVendaCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    [HttpPatch("{id:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelarPedidoVendaCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
