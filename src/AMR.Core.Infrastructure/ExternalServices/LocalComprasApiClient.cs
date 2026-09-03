using Microsoft.Extensions.Logging;
using AMR.Core.Application.DTOs;
using AMR.Core.Application.Interfaces;

namespace AMR.Core.Infrastructure.ExternalServices;

/// <summary>
/// Stub para desenvolvimento/testes — não requer o AMR-Compras rodando.
/// </summary>
public class LocalComprasApiClient(ILogger<LocalComprasApiClient> logger) : IComprasApiClient
{
    public Task<string?> SugerirPedidoCompraAsync(SugerirPedidoCompraDto dto, CancellationToken ct = default)
    {
        var id = $"local-sugestao-{Guid.NewGuid()}";
        logger.LogInformation(
            "[LOCAL COMPRAS] Sugestão de pedido de compra registrada (stub) para produto {ProdutoId} → id={SugestaoId}",
            dto.ProdutoId, id);
        return Task.FromResult<string?>(id);
    }
}
