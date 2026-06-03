using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AMR.Core.Infrastructure.Data;

namespace AMR.Core.API.Controllers;

[ApiController]
[Route("api")]
public class DropdownController(AmrCoreDbContext db) : ControllerBase
{
    [HttpGet("unidademedida")]
    public async Task<IActionResult> UnidadesMedida(CancellationToken ct)
    {
        var lista = await db.UnidadesMedida
            .Where(u => u.Ativo)
            .Select(u => new { u.Id, u.Sigla, u.Descricao })
            .OrderBy(u => u.Sigla)
            .ToListAsync(ct);
        return Ok(lista);
    }

    [HttpGet("fornecedor")]
    public async Task<IActionResult> Fornecedores(CancellationToken ct)
    {
        var lista = await db.Fornecedores
            .Where(f => f.Ativo)
            .Select(f => new { f.Id, f.RazaoSocial, f.NomeFantasia })
            .OrderBy(f => f.NomeFantasia)
            .ToListAsync(ct);
        return Ok(lista);
    }

    [HttpGet("cliente")]
    public async Task<IActionResult> Clientes(CancellationToken ct)
    {
        var lista = await db.Clientes
            .Where(c => c.Ativo)
            .Select(c => new { c.Id, c.Nome })
            .OrderBy(c => c.Nome)
            .ToListAsync(ct);
        return Ok(lista);
    }
}
