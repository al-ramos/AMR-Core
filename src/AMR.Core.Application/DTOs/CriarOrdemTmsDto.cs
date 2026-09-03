namespace AMR.Core.Application.DTOs;

public record CriarOrdemTmsDto(
    string  Origem,
    string  Destino,
    string  ClienteId,
    string? TransportadoraId,
    decimal PesoKg,
    decimal VolumeM3,
    decimal ValorFrete,
    DateTime? PrevisaoEntrega,
    string  PedidoCoreId
);
