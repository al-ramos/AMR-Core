using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.Produtos.Commands;
using AMR.Core.Application.Produtos.Queries;
using AMR.Core.API.Extensions;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class ProdutoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarProdutosQuery(), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Listar), result.Valor)
            : result.ToActionResult(this);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd with { Id = id }, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/inativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new InativarProdutoCommand(id), ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/reativar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ReativarProdutoCommand(id), ct);
        return result.ToActionResult(this);
    }
}
