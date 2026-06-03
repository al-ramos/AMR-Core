import { api } from './axiosInstance'

export interface ClienteDto {
  id: number
  nome: string
  cpfCnpj?: string
}

export const clienteApi = {
  listar: () => api.get<ClienteDto[]>('/Cliente').then(r => r.data),
}
