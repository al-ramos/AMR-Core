using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.MovimentosEstoque.Queries;

namespace AMR.Core.API.Controllers;

/// <summary>Movimentos de estoque (entradas, saídas e ajustes).</summary>
[ApiController]
[Route("api/[controller]")]
public class MovimentoEstoqueController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista os movimentos de estoque com filtros opcionais.</summary>
    /// <param name="empresaId">ID da empresa (obrigatório).</param>
    /// <param name="produtoId">Filtra por produto específico.</param>
    /// <param name="tipo">Filtra por tipo: Entrada, Saida ou AjusteManual.</param>
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int     empresaId,
        [FromQuery] int?    produtoId = null,
        [FromQuery] string? tipo      = null,
        CancellationToken   ct        = default)
    {
        var result = await mediator.Send(
            new ListarMovimentosEstoqueQuery(empresaId, produtoId, tipo), ct);

        return result.Sucesso ? Ok(result.Valor) : BadRequest(result.Erro);
    }
}
