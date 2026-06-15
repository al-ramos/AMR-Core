using Microsoft.Extensions.Logging;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;

namespace AMR.Core.Infrastructure.ExternalServices;

/// <summary>
/// Stub para desenvolvimento/testes — não requer o AMR-TMS rodando.
/// </summary>
public class LocalTmsApiClient(ILogger<LocalTmsApiClient> logger) : ITmsApiClient
{
    public Task<string?> CriarOrdemDeEntregaAsync(CriarOrdemTmsDto dto, CancellationToken ct = default)
    {
        var id = $"local-ordem-{Guid.NewGuid()}";
        logger.LogInformation(
            "[LOCAL TMS] Ordem criada (stub) para pedido {PedidoCoreId} → id={OrdemId}",
            dto.PedidoCoreId, id);
        return Task.FromResult<string?>(id);
    }
}
