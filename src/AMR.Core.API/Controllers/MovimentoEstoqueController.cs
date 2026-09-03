using MediatR;
using Microsoft.AspNetCore.Mvc;
using AMR.Core.Application.MovimentosEstoque.Queries;
using AMR.Core.API.Extensions;

namespace AMR.Core.API.Controllers;

/// <summary>Movimentos de estoque (entradas, saídas e ajustes).</summary>
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class MovimentoEstoqueController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista os movimentos de estoque com filtros opcionais.</summary>
    /// <param name="empresaId">ID da empresa (obrigatório).</param>
    /// <param name="produtoId">Filtra por produto específico.</param>
    /// <param name="tipo">Filtra por tipo: Entrada, Saida ou AjusteManual.</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int     empresaId,
        [FromQuery] int?    produtoId = null,
        [FromQuery] string? tipo      = null,
        CancellationToken   ct        = default)
    {
        var result = await mediator.Send(
            new ListarMovimentosEstoqueQuery(empresaId, produtoId, tipo), ct);

        return result.ToActionResult(this);
    }
}
