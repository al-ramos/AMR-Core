import { api } from './axiosInstance'

export interface FornecedorDto {
  id: number
  nome: string
  cnpj?: string
}

export const fornecedorApi = {
  listar: () => api.get<FornecedorDto[]>('/Fornecedor').then(r => r.data),
}
