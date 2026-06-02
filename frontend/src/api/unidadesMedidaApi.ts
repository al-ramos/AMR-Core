import { api } from './axiosInstance'

export interface UnidadeMedidaDto {
  id: number
  sigla: string
  descricao: string
}

export const unidadesMedidaApi = {
  listar: () => api.get<UnidadeMedidaDto[]>('/UnidadeMedida').then(r => r.data),
}
