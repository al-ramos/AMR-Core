using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.PedidosCompra.Commands;
using AMR.Core.Application.PedidosCompra.Queries;

namespace AMR.Core.API.Controllers;

/// <summary>Gestão de pedidos de compra (Rascunho → Aprovado → Recebido).</summary>
[ApiController]
[Route("api/[controller]")]
public class PedidoCompraController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista pedidos de compra da empresa, com filtro opcional de status.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int empresaId, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await mediator.Send(new ListarPedidosCompraQuery(empresaId, status), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    /// <summary>Retorna um pedido de compra com seus itens.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ObterPedidoCompraQuery(id), ct);
        return result.Sucesso ? Ok(result.Valor) : NotFound(result.Erro);
    }

    /// <summary>Cria um novo pedido de compra em rascunho.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoCompraCommand cmd, CancellationToken ct)
    {
        var result = await mediator.Send(cmd, ct);
        return result.Sucesso
            ? CreatedAtAction(nameof(Obter), new { id = result.Valor!.Id }, result.Valor)
            : BadRequest(result.Erro);
    }

    /// <summary>Aprova um pedido de compra (Rascunho → Aprovado).</summary>
    [HttpPatch("{id:int}/aprovar")]
    public async Task<IActionResult> Aprovar(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new AprovarPedidoCompraCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }

    /// <summary>Registra o recebimento da mercadoria (Aprovado → Recebido). Atualiza estoque automaticamente.</summary>
    [HttpPatch("{id:int}/receber")]
    public async Task<IActionResult> Receber(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ReceberPedidoCompraCommand(id), ct);
        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
