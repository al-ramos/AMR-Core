using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;

namespace AMR.Core.Infrastructure.ExternalServices;

public class ComprasApiClient(HttpClient http, ILogger<ComprasApiClient> logger) : IComprasApiClient
{
    public async Task<string?> SugerirPedidoCompraAsync(SugerirPedidoCompraDto dto, CancellationToken ct = default)
    {
        try
        {
            var response = await http.PostAsJsonAsync("/api/pedidos-compra/sugestao", dto, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            return result.GetProperty("data").GetProperty("id").GetString();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Falha ao sugerir Pedido de Compra no AMR-Compras para produto {ProdutoId}", dto.ProdutoId);
            return null; // Fire-and-forget tolerado
        }
    }
}
