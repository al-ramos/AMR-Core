using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;

namespace AMR.Core.Infrastructure.ExternalServices;

public class TmsApiClient(HttpClient http, ILogger<TmsApiClient> logger) : ITmsApiClient
{
    public async Task<string?> CriarOrdemDeEntregaAsync(CriarOrdemTmsDto dto, CancellationToken ct = default)
    {
        try
        {
            var response = await http.PostAsJsonAsync("/api/ordens", dto, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            return result.GetProperty("id").GetString();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Falha ao criar OrdemDeEntrega no TMS para pedido {PedidoCoreId}", dto.PedidoCoreId);
            return null; // Fire-and-forget tolerado
        }
    }
}
