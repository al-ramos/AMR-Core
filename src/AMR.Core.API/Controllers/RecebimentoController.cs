using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.Recebimento.Commands;
using AMR.Core.Application.Recebimento.Queries;
using AMR.Core.API.Extensions;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class RecebimentoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarOrdensRecebimentoQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("pendentes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PedidosCompraAprovados(CancellationToken ct)
    {
        var result = await mediator.Send(new GetPCsAprovadosQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpPost("iniciar")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Iniciar([FromBody] IniciarRecebimentoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Listar), new { id = result.Valor!.Id }, result.Valor)
            : result.ToActionResult(this);
    }

    [HttpPut("{id:int}/receber-item")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReceberItem(int id, [FromBody] ReceberItemRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ReceberItemCommand(id, req.ItemId, req.Quantidade, req.LocalizacaoId), ct);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:int}/concluir")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Concluir(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ConcluirRecebimentoCommand(id), ct);
        return result.ToActionResult(this);
    }
}

public record ReceberItemRequest(int ItemId, decimal Quantidade, int? LocalizacaoId = null);
