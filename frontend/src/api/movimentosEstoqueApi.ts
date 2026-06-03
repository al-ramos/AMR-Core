import { api } from './axiosInstance'

export interface MovimentoEstoqueDto {
  id:          number
  produtoId:   number
  produtoNome: string
  tipo:        'Entrada' | 'Saida' | 'AjusteManual'
  quantidade:  number
  origem?:     string
  dataHora:    string
}

export interface FiltrosMovimento {
  empresaId:  number
  produtoId?: number
  tipo?:      string
}

export const movimentosEstoqueApi = {
  listar: (filtros: FiltrosMovimento) =>
    api.get<MovimentoEstoqueDto[]>('/MovimentoEstoque', { params: filtros }).then(r => r.data),
}
