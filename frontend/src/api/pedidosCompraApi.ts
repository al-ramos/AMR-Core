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

export const pedidosCompraApi = {
  listar:  (empresaId: number, status?: string) =>
    api.get<PedidoCompraDto[]>('/PedidoCompra', { params: { empresaId, status } }).then(r => r.data),
  obter:   (id: number) => api.get<PedidoCompraDto>(`/PedidoCompra/${id}`).then(r => r.data),
  aprovar: (id: number) => api.patch<PedidoCompraDto>(`/PedidoCompra/${id}/aprovar`).then(r => r.data),
  receber: (id: number) => api.patch<PedidoCompraDto>(`/PedidoCompra/${id}/receber`).then(r => r.data),
}
