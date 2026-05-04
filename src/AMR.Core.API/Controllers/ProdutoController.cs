using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.Produtos.Commands;
using AMR.Core.Application.Produtos.Queries;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var result = await mediator.Send(new ListarProdutosQuery(), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Listar), result.Valor)
            : BadRequest(result.Erro);
    }
}
