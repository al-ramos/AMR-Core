import { api } from './axiosInstance'
import type { PedidoCompraDto } from './pedidosCompraApi'

export interface ItemRecebimentoDto {
  id: number
  produtoId: number
  nomeProduto: string
  localizacaoId?: number
  qntEsperada: number
  qntRecebida: number
}

export interface OrdemRecebimentoDto {
  id: number
  pedidoCompraId: number
  status: string
  dataCriacao: string
  dataRecebimento?: string
  itens: ItemRecebimentoDto[]
}

export const recebimentoApi = {
  listar:    () =>
    api.get<OrdemRecebimentoDto[]>('/Recebimento').then(r => r.data),

  pendentes: () =>
    api.get<PedidoCompraDto[]>('/Recebimento/pendentes').then(r => r.data),

  iniciar:   (pedidoCompraId: number) =>
    api.post<OrdemRecebimentoDto>('/Recebimento/iniciar', { pedidoCompraId }).then(r => r.data),

  receberItem: (ordemId: number, itemId: number, quantidade: number, localizacaoId?: number) =>
    api.put<OrdemRecebimentoDto>(`/Recebimento/${ordemId}/receber-item`, {
      itemId, quantidade, localizacaoId,
    }).then(r => r.data),

  concluir:  (ordemId: number) =>
    api.put<OrdemRecebimentoDto>(`/Recebimento/${ordemId}/concluir`).then(r => r.data),
}
