import { api } from './axiosInstance'

export interface ClienteDto {
  id: number
  nome: string
  tipoDocumento: string
}

export const clientesApi = {
  listar: (empresaId: number) =>
    api.get<ClienteDto[]>('/Cliente', { params: { empresaId } }).then(r => r.data),
}
