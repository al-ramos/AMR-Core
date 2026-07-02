using AMR.Core.Application.DTOs;

namespace AMR.Core.Application.Interfaces;

public interface IComprasApiClient
{
    Task<string?> SugerirPedidoCompraAsync(SugerirPedidoCompraDto dto, CancellationToken ct = default);
}
