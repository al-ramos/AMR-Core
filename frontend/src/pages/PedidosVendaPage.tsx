import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pedidosVendaApi } from '../api/pedidosVendaApi'
import { produtosApi } from '../api/produtosApi'
import { dropdownApi } from '../api/dropdownApi'

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
    Aberto:    'badge-rascunho',
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

interface ItemForm { produtoId: string; quantidade: string; precoUnitario: string }
const ITEM_VAZIO: ItemForm = { produtoId: '', quantidade: '1', precoUnitario: '' }

export default function PedidosVendaPage() {
  const [filtroStatus, setFiltroStatus] = useState('')
  const [detalheId, setDetalheId]       = useState<number | null>(null)
  const [modalNovo, setModalNovo]       = useState(false)
  const [clienteId, setClienteId]       = useState('')
  const [observacao, setObservacao]     = useState('')
  const [itens, setItens]               = useState<ItemForm[]>([{ ...ITEM_VAZIO }])
  const [erroCriar, setErroCriar]       = useState('')
  const qc = useQueryClient()

  const { data: pedidos = [], isLoading, isError, error } = useQuery({
    queryKey: ['pedidos-venda', EMPRESA_ID, filtroStatus],
    queryFn:  () => pedidosVendaApi.listar(EMPRESA_ID, filtroStatus || undefined),
  })

  const { data: detalhe } = useQuery({
    queryKey: ['pedido-venda', detalheId],
    queryFn:  () => pedidosVendaApi.obter(detalheId!),
    enabled:  !!detalheId,
  })

  const { data: clientes = [] } = useQuery({
    queryKey: ['clientes'],
    queryFn: dropdownApi.clientes,
    enabled: modalNovo,
  })

  const { data: produtos = [] } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
    enabled: modalNovo,
  })

  const aprovar = useMutation({
    mutationFn: pedidosVendaApi.aprovar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const faturar = useMutation({
    mutationFn: pedidosVendaApi.faturar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const cancelar = useMutation({
    mutationFn: pedidosVendaApi.cancelar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const criar = useMutation({
    mutationFn: pedidosVendaApi.criar,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['pedidos-venda'] })
      fecharModal()
    },
    onError: (e: Error) => setErroCriar(e.message),
  })

  function fecharModal() {
    setModalNovo(false)
    setClienteId('')
    setObservacao('')
    setItens([{ ...ITEM_VAZIO }])
    setErroCriar('')
  }

  function addItem() { setItens(i => [...i, { ...ITEM_VAZIO }]) }
  function removeItem(idx: number) { setItens(i => i.filter((_, j) => j !== idx)) }
  function updateItem(idx: number, field: keyof ItemForm, val: string) {
    setItens(i => i.map((item, j) => j === idx ? { ...item, [field]: val } : item))
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setErroCriar('')
    const itensPayload = itens.map(i => ({
      produtoId: parseInt(i.produtoId),
      quantidade: parseFloat(i.quantidade),
      precoUnitario: parseFloat(i.precoUnitario),
    }))
    criar.mutate({ empresaId: EMPRESA_ID, clienteId: parseInt(clienteId), observacao: observacao || undefined, itens: itensPayload })
  }

  const total     = pedidos.reduce((s, p) => s + p.total, 0)
  const aprovados = pedidos.filter(p => p.status === 'Aprovado').length
  const faturados = pedidos.filter(p => p.status === 'Faturado').length

  return (
    <>
      <div className="row g-3 mb-4">
        {[
          { label: 'Total de pedidos', value: pedidos.length, color: '#212529' },
          { label: 'Aprovados',        value: aprovados,      color: '#1565c0' },
          { label: 'Faturados',        value: faturados,      color: '#2e7d32' },
          { label: 'Valor total',      value: brl(total),     color: '#1565c0' },
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
          <div className="d-flex gap-2">
            <select value={filtroStatus} onChange={e => setFiltroStatus(e.target.value)}
              className="form-select form-select-sm" style={{ width: 160 }}>
              <option value="">Todos os status</option>
              <option value="Rascunho">Rascunho</option>
              <option value="Aprovado">Aprovado</option>
              <option value="Faturado">Faturado</option>
              <option value="Cancelado">Cancelado</option>
            </select>
            <button className="btn btn-primary btn-sm" style={{ fontSize: 12 }} onClick={() => setModalNovo(true)}>
              <i className="bi bi-plus-lg me-1"></i>Novo PV
            </button>
          </div>
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
                {pedidos.map(p => (
                  <tr key={p.id}>
                    <td className="font-monospace text-muted" style={{ fontSize: 12 }}>#{p.id}</td>
                    <td>Cliente {p.clienteId}</td>
                    <td className="text-nowrap">{fmt(p.dataEmissao)}</td>
                    <td className="text-nowrap text-muted">{fmt(p.dataAprovacao)}</td>
                    <td className="text-nowrap text-muted">{fmt(p.dataFaturamento)}</td>
                    <td><StatusBadge status={p.status} /></td>
                    <td className="text-end fw-semibold">{brl(p.total)}</td>
                    <td className="text-end text-nowrap">
                      <button className="btn btn-sm btn-outline-secondary me-1" style={{ fontSize: 11 }}
                        onClick={() => setDetalheId(detalheId === p.id ? null : p.id)}>
                        <i className="bi bi-eye me-1"></i>Itens
                      </button>
                      {(p.status === 'Rascunho' || p.status === 'Aberto') && (
                        <button className="btn btn-sm btn-outline-primary me-1" style={{ fontSize: 11 }}
                          disabled={aprovar.isPending} onClick={() => aprovar.mutate(p.id)}>Aprovar</button>
                      )}
                      {p.status === 'Aprovado' && (
                        <button className="btn btn-sm btn-outline-success me-1" style={{ fontSize: 11 }}
                          disabled={faturar.isPending} onClick={() => faturar.mutate(p.id)}>Faturar</button>
                      )}
                      {p.status !== 'Faturado' && p.status !== 'Cancelado' && (
                        <button className="btn btn-sm btn-outline-danger" style={{ fontSize: 11 }}
                          disabled={cancelar.isPending} onClick={() => cancelar.mutate(p.id)}>Cancelar</button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
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

      {/* Modal Novo PV */}
      {modalNovo && (
        <div className="modal d-block" style={{ background: 'rgba(0,0,0,.4)' }} onClick={e => e.target === e.currentTarget && fecharModal()}>
          <div className="modal-dialog modal-dialog-centered modal-lg">
            <div className="modal-content">
              <div className="modal-header py-2 px-3">
                <h6 className="modal-title mb-0" style={{ fontSize: 14 }}>Novo pedido de venda</h6>
                <button type="button" className="btn-close btn-sm" onClick={fecharModal}></button>
              </div>
              <form onSubmit={handleSubmit}>
                <div className="modal-body px-3 py-2" style={{ fontSize: 13 }}>
                  {erroCriar && <div className="alert alert-danger py-1 px-2 mb-2" style={{ fontSize: 12 }}>{erroCriar}</div>}

                  <div className="row g-2 mb-2">
                    <div className="col-8">
                      <label className="form-label mb-1">Cliente *</label>
                      <select value={clienteId} onChange={e => setClienteId(e.target.value)} required className="form-select form-select-sm">
                        <option value="">Selecione...</option>
                        {clientes.map(c => <option key={c.id} value={c.id}>{c.nome}</option>)}
                      </select>
                    </div>
                    <div className="col-4">
                      <label className="form-label mb-1">Observação</label>
                      <input value={observacao} onChange={e => setObservacao(e.target.value)} className="form-control form-control-sm" />
                    </div>
                  </div>

                  <div className="mb-1 fw-semibold" style={{ fontSize: 12 }}>Itens</div>
                  {itens.map((item, idx) => (
                    <div key={idx} className="row g-2 mb-1 align-items-end">
                      <div className="col-5">
                        <select value={item.produtoId} onChange={e => updateItem(idx, 'produtoId', e.target.value)} required className="form-select form-select-sm">
                          <option value="">Produto...</option>
                          {produtos.filter(p => p.ativo).map(p => <option key={p.id} value={p.id}>{p.nome}</option>)}
                        </select>
                      </div>
                      <div className="col-2">
                        <input type="number" min="0.01" step="0.01" placeholder="Qtd" value={item.quantidade}
                          onChange={e => updateItem(idx, 'quantidade', e.target.value)} required className="form-control form-control-sm" />
                      </div>
                      <div className="col-3">
                        <input type="number" min="0" step="0.01" placeholder="Preço unit." value={item.precoUnitario}
                          onChange={e => updateItem(idx, 'precoUnitario', e.target.value)} required className="form-control form-control-sm" />
                      </div>
                      <div className="col-2">
                        {itens.length > 1 && (
                          <button type="button" className="btn btn-sm btn-outline-danger w-100" onClick={() => removeItem(idx)}>
                            <i className="bi bi-trash"></i>
                          </button>
                        )}
                      </div>
                    </div>
                  ))}
                  <button type="button" className="btn btn-sm btn-outline-secondary mt-1" style={{ fontSize: 11 }} onClick={addItem}>
                    <i className="bi bi-plus me-1"></i>Adicionar item
                  </button>
                </div>
                <div className="modal-footer py-2 px-3">
                  <button type="button" className="btn btn-sm btn-outline-secondary" onClick={fecharModal}>Cancelar</button>
                  <button type="submit" className="btn btn-sm btn-primary" disabled={criar.isPending}>
                    {criar.isPending ? 'Criando...' : 'Criar pedido'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </>
  )
}
