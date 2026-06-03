import { api } from './axiosInstance'

export interface ProdutoDto {
  id: number
  sku: string
  nome: string
  descricao?: string
  precoUnitario: number
  estoqueMinimo: number
  unidadeMedida: string
  ativo: boolean
}

export interface CriarProdutoPayload {
  sku: string
  nome: string
  descricao?: string
  precoUnitario: number
  estoqueMinimo: number
  unidadeMedidaId: number
  empresaId: number
}

export interface AtualizarProdutoPayload {
  nome: string
  descricao?: string
  precoUnitario: number
  estoqueMinimo: number
  unidadeMedidaId: number
}

export const produtosApi = {
  listar:   () => api.get<ProdutoDto[]>('/Produto').then(r => r.data),
  criar:    (payload: CriarProdutoPayload) => api.post<ProdutoDto>('/Produto', payload).then(r => r.data),
  atualizar:(id: number, payload: AtualizarProdutoPayload) => api.put<ProdutoDto>(`/Produto/${id}`, payload).then(r => r.data),
  inativar: (id: number) => api.patch<ProdutoDto>(`/Produto/${id}/inativar`).then(r => r.data),
  reativar: (id: number) => api.patch<ProdutoDto>(`/Produto/${id}/reativar`).then(r => r.data),
}
