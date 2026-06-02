import { api } from './axiosInstance'
import type { ItemPedidoDto } from './pedidosCompraApi'

export interface PedidoVendaDto {
  id: number
  empresaId: number
  clienteId: number
  status: string
  dataEmissao: string
  dataAprovacao?: string
  dataFaturamento?: string
  observacao?: string
  total: number
  itens: ItemPedidoDto[]
}

export interface ItemPedidoVendaInput {
  produtoId: number
  quantidade: number
  precoUnitario: number
  desconto: number
}

export interface CriarPedidoVendaPayload {
  empresaId: number
  clienteId: number
  observacao?: string
  itens: ItemPedidoVendaInput[]
}

export const pedidosVendaApi = {
  listar:  (empresaId: number, status?: string) =>
    api.get<PedidoVendaDto[]>('/PedidoVenda', { params: { empresaId, status } }).then(r => r.data),
  obter:   (id: number) => api.get<PedidoVendaDto>(`/PedidoVenda/${id}`).then(r => r.data),
  criar:   (payload: CriarPedidoVendaPayload) => api.post<PedidoVendaDto>('/PedidoVenda', payload).then(r => r.data),
  aprovar: (id: number) => api.patch<PedidoVendaDto>(`/PedidoVenda/${id}/aprovar`).then(r => r.data),
  faturar: (id: number) => api.patch<PedidoVendaDto>(`/PedidoVenda/${id}/faturar`).then(r => r.data),
}
