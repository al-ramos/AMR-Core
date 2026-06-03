import { api } from './axiosInstance'

export interface UnidadeMedidaDto { id: number; sigla: string; descricao: string }
export interface FornecedorDto    { id: number; razaoSocial: string; nomeFantasia: string }
export interface ClienteDto       { id: number; nome: string }

export const dropdownApi = {
  unidadesMedida: () => api.get<UnidadeMedidaDto[]>('/unidademedida').then(r => r.data),
  fornecedores:   () => api.get<FornecedorDto[]>('/fornecedor').then(r => r.data),
  clientes:       () => api.get<ClienteDto[]>('/cliente').then(r => r.data),
}
