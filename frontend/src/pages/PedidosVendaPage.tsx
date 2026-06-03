import { useState, useMemo } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pedidosVendaApi } from '../api/pedidosVendaApi'
import { clienteApi } from '../api/clienteApi'
import { usePagination } from '../hooks/usePagination'
import { Pagination } from '../components/Pagination'

const EMPRESA_ID = 1

function brl(v: number) {
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

function fmt(iso?: string) {
  if (!iso) return '—'
  const [y, m, d] = iso.slice(0, 10).split('-')
  return `${d}/${m}/${y}`
}

function StatusBadge({ status }: { status: string }) {
  const map: Record<string, string> = {
    Rascunho:  'badge-rascunho',
    Aprovado:  'badge-aprovado',
    Faturado:  'badge-faturado',
    Cancelado: 'badge-cancelado',
  }
  return (
    <span className={`badge rounded-pill fw-semibold ${map[status] ?? 'badge-rascunho'}`}
      style={{ fontSize: 11, padding: '4px 10px' }}>
      {status}
    </span>
  )
}

export default function PedidosVendaPage() {
  const [filtroStatus, setFiltroStatus] = useState('')
  const [detalheId, setDetalheId]       = useState<number | null>(null)
  const qc = useQueryClient()

  const { data: pedidos = [], isLoading, isError, error } = useQuery({
    queryKey: ['pedidos-venda', EMPRESA_ID, filtroStatus],
    queryFn:  () => pedidosVendaApi.listar(EMPRESA_ID, filtroStatus || undefined),
  })

  // Busca clientes para resolver nomes
  const { data: clientes = [] } = useQuery({
    queryKey: ['clientes'],
    queryFn: clienteApi.listar,
  })

  const clienteMap = useMemo(
    () => Object.fromEntries(clientes.map(c => [c.id, c.nome])),
    [clientes],
  )

  const { data: detalhe } = useQuery({
    queryKey: ['pedido-venda', detalheId],
    queryFn:  () => pedidosVendaApi.obter(detalheId!),
    enabled:  !!detalheId,
  })

  const aprovar = useMutation({
    mutationFn: pedidosVendaApi.aprovar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const faturar = useMutation({
    mutationFn: pedidosVendaApi.faturar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const { paged, page, pageSize, totalPages, total, changePage, changePageSize } = usePagination(pedidos)

  const totalVal  = pedidos.reduce((s, p) => s + p.total, 0)
  const aprovados = pedidos.filter(p => p.status === 'Aprovado').length
  const faturados = pedidos.filter(p => p.status === 'Faturado').length

  return (
    <>
      <div className="row g-3 mb-4">
        {[
          { label: 'Total de pedidos', value: pedidos.length, color: '#212529' },
          { label: 'Aprovados',        value: aprovados,      color: '#1565c0' },
          { label: 'Faturados',        value: faturados,      color: '#2e7d32' },
          { label: 'Valor total',      value: brl(totalVal),  color: '#1565c0' },
        ].map(m => (
          <div key={m.label} className="col-md-3">
            <div className="amr-metric-card">
              <p className="amr-metric-label">{m.label}</p>
              <p className="amr-metric-value" style={{ color: m.color }}>{m.value}</p>
            </div>
          </div>
        ))}
      </div>

      <div className="amr-table-card">
        <div className="d-flex align-items-center justify-content-between px-3 py-3 border-bottom">
          <span style={{ fontSize: 13, fontWeight: 600, color: '#495057' }}>
            <i className="bi bi-cart-check me-2"></i>Pedidos de venda
          </span>
          <select
            value={filtroStatus}
            onChange={e => { setFiltroStatus(e.target.value); changePage(1) }}
            className="form-select form-select-sm"
            style={{ width: 160 }}
          >
            <option value="">Todos os status</option>
            <option value="Rascunho">Rascunho</option>
            <option value="Aprovado">Aprovado</option>
            <option value="Faturado">Faturado</option>
            <option value="Cancelado">Cancelado</option>
          </select>
        </div>

        {isLoading && (
          <div className="amr-empty">
            <div className="spinner-border spinner-border-sm text-primary mb-2" role="status"></div>
            <span style={{ fontSize: 13 }}>Carregando...</span>
          </div>
        )}

        {isError && (
          <div className="p-3">
            <div className="alert alert-danger d-flex align-items-center gap-2 mb-0" style={{ fontSize: 13 }}>
              <i className="bi bi-exclamation-triangle-fill"></i>
              {(error as Error)?.message ?? 'Erro ao carregar pedidos.'}
            </div>
          </div>
        )}

        {!isLoading && !isError && pedidos.length === 0 && (
          <div className="amr-empty">
            <i className="bi bi-cart-x"></i>
            <div style={{ fontSize: 14, fontWeight: 500 }}>Nenhum pedido encontrado</div>
          </div>
        )}

        {!isLoading && pedidos.length > 0 && (
          <>
            <div className="table-responsive">
              <table className="table table-hover table-sm mb-0" style={{ fontSize: 13 }}>
                <thead className="table-light">
                  <tr>
                    <th>#</th>
                    <th>Cliente</th>
                    <th>Emissão</th>
                    <th>Aprovação</th>
                    <th>Faturamento</th>
                    <th>Status</th>
                    <th className="text-end">Total</th>
                    <th className="text-end">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {paged.map(p => (
                    <tr key={p.id}>
                      <td className="font-monospace text-muted" style={{ fontSize: 12 }}>#{p.id}</td>
                      <td className="fw-medium">
                        {clienteMap[p.clienteId] ?? `Cliente ${p.clienteId}`}
                      </td>
                      <td className="text-nowrap">{fmt(p.dataEmissao)}</td>
                      <td className="text-nowrap text-muted">{fmt(p.dataAprovacao)}</td>
                      <td className="text-nowrap text-muted">{fmt(p.dataFaturamento)}</td>
                      <td><StatusBadge status={p.status} /></td>
                      <td className="text-end fw-semibold">{brl(p.total)}</td>
                      <td className="text-end text-nowrap">
                        <button
                          className="btn btn-sm btn-outline-secondary me-1"
                          style={{ fontSize: 11 }}
                          onClick={() => setDetalheId(detalheId === p.id ? null : p.id)}
                        >
                          <i className="bi bi-eye me-1"></i>Itens
                        </button>
                        {p.status === 'Rascunho' && (
                          <button
                            className="btn btn-sm btn-outline-primary me-1"
                            style={{ fontSize: 11 }}
                            disabled={aprovar.isPending}
                            onClick={() => aprovar.mutate(p.id)}
                          >Aprovar</button>
                        )}
                        {p.status === 'Aprovado' && (
                          <button
                            className="btn btn-sm btn-outline-success"
                            style={{ fontSize: 11 }}
                            disabled={faturar.isPending}
                            onClick={() => faturar.mutate(p.id)}
                          >Faturar</button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <Pagination
              page={page} totalPages={totalPages} pageSize={pageSize} total={total}
              onPage={changePage} onPageSize={changePageSize}
            />
          </>
        )}
      </div>

      {/* Painel de itens */}
      {detalheId && detalhe && (
        <div className="amr-table-card mt-3">
          <div className="d-flex align-items-center justify-content-between px-3 py-3 border-bottom">
            <span style={{ fontSize: 13, fontWeight: 600, color: '#495057' }}>
              <i className="bi bi-list-ul me-2"></i>Itens do pedido #{detalheId}
            </span>
            <button className="btn-close btn-sm" onClick={() => setDetalheId(null)}></button>
          </div>
          <div className="table-responsive">
            <table className="table table-sm mb-0" style={{ fontSize: 13 }}>
              <thead className="table-light">
                <tr><th>Produto</th><th className="text-end">Qtd</th><th className="text-end">Unit.</th><th className="text-end">Subtotal</th></tr>
              </thead>
              <tbody>
                {detalhe.itens.map((item, i) => (
                  <tr key={i}>
                    <td>{item.produtoNome}</td>
                    <td className="text-end">{item.quantidade}</td>
                    <td className="text-end">{brl(item.precoUnitario)}</td>
                    <td className="text-end fw-semibold">{brl(item.subtotal)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </>
  )
}
