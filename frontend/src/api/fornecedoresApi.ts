import { api } from './axiosInstance'

export interface FornecedorDto {
  id: number
  nome: string
  categoria: string
}

export const fornecedoresApi = {
  listar: (empresaId: number) =>
    api.get<FornecedorDto[]>('/Fornecedor', { params: { empresaId } }).then(r => r.data),
}
