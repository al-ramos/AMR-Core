using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.PedidosVenda.Commands;
using AMR.Core.Application.PedidosVenda.Queries;

namespace AMR.Core.API.Controllers;

/// <summary>Gestão de pedidos de venda (Rascunho → Aprovado → Faturado).</summary>
[ApiController]
[Route("api/[controller]")]
public class PedidoVendaController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista pedidos de venda da empresa, com filtro opcional de status.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPedidosVendaQuery(empresaId, status), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    /// <summary>Retorna um pedido de venda com seus itens.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterPedidoVendaQuery(id), ct);
        return result.Sucesso ? Ok(result.Valor) : NotFound(result.Erro);
    }

    /// <summary>Cria um novo pedido de venda em rascunho.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoVendaCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor)
            : BadRequest(result.Erro);
    }

    /// <summary>Aprova um pedido de venda (Rascunho → Aprovado).</summary>
    [HttpPatch("{id:int}/aprovar")]
    public async Task<IActionResult> Aprovar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new AprovarPedidoVendaCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    /// <summary>Fatura o pedido de venda e realiza baixa no estoque (Aprovado → Faturado).</summary>
    [HttpPatch("{id:int}/faturar")]
    public async Task<IActionResult> Faturar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new FaturarPedidoVendaCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
