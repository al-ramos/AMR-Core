using AMR.Core.Application.DTOs;

namespace AMR.Core.Application.Interfaces;

public interface ITmsApiClient
{
    Task<string?> CriarOrdemDeEntregaAsync(CriarOrdemTmsDto dto, CancellationToken ct = default);
}
