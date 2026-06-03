import { api } from './axiosInstance'

export interface ItemPedidoDto {
  produtoId: number
  produtoNome: string
  quantidade: number
  precoUnitario: number
  subtotal: number
}

export interface PedidoCompraDto {
  id: number
  empresaId: number
  fornecedorId: number
  status: string
  dataEmissao: string
  dataAprovacao?: string
  dataRecebimento?: string
  observacao?: string
  total: number
  itens: ItemPedidoDto[]
}

export interface ItemPedidoPayload {
  produtoId: number
  quantidade: number
  precoUnitario: number
}

export interface CriarPedidoCompraPayload {
  empresaId: number
  fornecedorId: number
  observacao?: string
  itens: ItemPedidoPayload[]
}

export const pedidosCompraApi = {
  listar:   (empresaId: number, status?: string) =>
    api.get<PedidoCompraDto[]>('/PedidoCompra', { params: { empresaId, status } }).then(r => r.data),
  obter:    (id: number) => api.get<PedidoCompraDto>(`/PedidoCompra/${id}`).then(r => r.data),
  criar:    (payload: CriarPedidoCompraPayload) => api.post<PedidoCompraDto>('/PedidoCompra', payload).then(r => r.data),
  aprovar:  (id: number) => api.patch<PedidoCompraDto>(`/PedidoCompra/${id}/aprovar`).then(r => r.data),
  receber:  (id: number) => api.patch<PedidoCompraDto>(`/PedidoCompra/${id}/receber`).then(r => r.data),
  cancelar: (id: number) => api.patch<PedidoCompraDto>(`/PedidoCompra/${id}/cancelar`).then(r => r.data),
}
