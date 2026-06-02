import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pedidosVendaApi, type CriarPedidoVendaPayload, type ItemPedidoVendaInput } from '../api/pedidosVendaApi'
import { produtosApi } from '../api/produtosApi'
import { clientesApi } from '../api/clientesApi'

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

const ITEM_VAZIO: ItemPedidoVendaInput = { produtoId: 0, quantidade: 1, precoUnitario: 0, desconto: 0 }

function novoForm(): CriarPedidoVendaPayload {
  return { empresaId: EMPRESA_ID, clienteId: 0, observacao: '', itens: [{ ...ITEM_VAZIO }] }
}

export default function PedidosVendaPage() {
  const [filtroStatus, setFiltroStatus] = useState('')
  const [detalheId, setDetalheId]       = useState<number | null>(null)
  const [modalAberto, setModalAberto]   = useState(false)
  const [form, setForm]                 = useState<CriarPedidoVendaPayload>(novoForm)
  const [erro, setErro]                 = useState<string | null>(null)
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

  const { data: produtos = [] } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
  })

  const { data: clientes = [] } = useQuery({
    queryKey: ['clientes', EMPRESA_ID],
    queryFn: () => clientesApi.listar(EMPRESA_ID),
  })

  const aprovar = useMutation({
    mutationFn: pedidosVendaApi.aprovar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const faturar = useMutation({
    mutationFn: pedidosVendaApi.faturar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pedidos-venda'] }),
  })

  const criar = useMutation({
    mutationFn: pedidosVendaApi.criar,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['pedidos-venda'] })
      setModalAberto(false)
      setForm(novoForm())
      setErro(null)
    },
    onError: (e: unknown) => setErro((e as Error)?.message ?? 'Erro ao criar pedido.'),
  })

  const total     = pedidos.reduce((s, p) => s + p.total, 0)
  const aprovados = pedidos.filter(p => p.status === 'Aprovado').length
  const faturados = pedidos.filter(p => p.status === 'Faturado').length

  function atualizarItem(idx: number, campo: keyof ItemPedidoVendaInput, valor: number) {
    setForm(f => {
      const itens = [...f.itens]
      itens[idx] = { ...itens[idx], [campo]: valor }
      if (campo === 'produtoId') {
        const prod = produtos.find(p => p.id === valor)
        if (prod) itens[idx].precoUnitario = prod.precoUnitario
      }
      return { ...f, itens }
    })
  }

  function adicionarItem() {
    setForm(f => ({ ...f, itens: [...f.itens, { ...ITEM_VAZIO }] }))
  }

  function removerItem(idx: number) {
    setForm(f => ({ ...f, itens: f.itens.filter((_, i) => i !== idx) }))
  }

  function submitForm(e: React.FormEvent) {
    e.preventDefault()
    setErro(null)
    if (!form.clienteId) { setErro('Selecione um cliente.'); return }
    if (form.itens.some(i => !i.produtoId || i.quantidade <= 0)) {
      setErro('Todos os itens precisam de produto e quantidade válida.')
      return
    }
    criar.mutate(form)
  }

  const totalModal = form.itens.reduce((s, i) => {
    const sub = i.quantidade * i.precoUnitario
    return s + sub * (1 - i.desconto / 100)
  }, 0)

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
            <select
              value={filtroStatus}
              onChange={e => setFiltroStatus(e.target.value)}
              className="form-select form-select-sm"
              style={{ width: 160 }}
            >
              <option value="">Todos os status</option>
              <option value="Rascunho">Rascunho</option>
              <option value="Aprovado">Aprovado</option>
              <option value="Faturado">Faturado</option>
              <option value="Cancelado">Cancelado</option>
            </select>
            <button
              className="btn btn-sm btn-primary"
              style={{ fontSize: 12, whiteSpace: 'nowrap' }}
              onClick={() => { setModalAberto(true); setErro(null); setForm(novoForm()) }}
            >
              <i className="bi bi-plus-lg me-1"></i>Novo pedido
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
                  <th>Emissao</th>
                  <th>Aprovacao</th>
                  <th>Faturamento</th>
                  <th>Status</th>
                  <th className="text-end">Total</th>
                  <th className="text-end">Acoes</th>
                </tr>
              </thead>
              <tbody>
                {pedidos.map(p => (
                  <tr key={p.id}>
                    <td className="font-monospace text-muted" style={{ fontSize: 12 }}>#{p.id}</td>
                    <td>
                      {clientes.find(c => c.id === p.clienteId)?.nome ?? `Cliente ${p.clienteId}`}
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
        )}
      </div>

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

      {/* ── Modal Novo Pedido de Venda ─────────────────────────────────── */}
      {modalAberto && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,.4)' }}>
          <div className="modal-dialog modal-dialog-centered modal-lg">
            <div className="modal-content">
              <form onSubmit={submitForm}>
                <div className="modal-header">
                  <h5 className="modal-title" style={{ fontSize: 15 }}>
                    <i className="bi bi-cart-check me-2"></i>Novo pedido de venda
                  </h5>
                  <button type="button" className="btn-close" onClick={() => setModalAberto(false)}></button>
                </div>
                <div className="modal-body">
                  {erro && <div className="alert alert-danger py-2" style={{ fontSize: 13 }}>{erro}</div>}

                  <div className="row g-3 mb-3">
                    <div className="col-md-6">
                      <label className="form-label" style={{ fontSize: 12 }}>Cliente *</label>
                      <select
                        className="form-select form-select-sm"
                        value={form.clienteId}
                        onChange={e => setForm(f => ({ ...f, clienteId: parseInt(e.target.value) }))}
                      >
                        <option value={0}>Selecione...</option>
                        {clientes.map(c => (
                          <option key={c.id} value={c.id}>{c.nome}</option>
                        ))}
                      </select>
                    </div>
                    <div className="col-md-6">
                      <label className="form-label" style={{ fontSize: 12 }}>Observação</label>
                      <input
                        className="form-control form-control-sm"
                        value={form.observacao ?? ''}
                        onChange={e => setForm(f => ({ ...f, observacao: e.target.value }))}
                        placeholder="Observação opcional"
                      />
                    </div>
                  </div>

                  <div className="d-flex align-items-center justify-content-between mb-2">
                    <span style={{ fontSize: 12, fontWeight: 600, color: '#495057' }}>Itens</span>
                    <button type="button" className="btn btn-sm btn-outline-secondary" style={{ fontSize: 11 }} onClick={adicionarItem}>
                      <i className="bi bi-plus me-1"></i>Adicionar item
                    </button>
                  </div>

                  <table className="table table-sm" style={{ fontSize: 12 }}>
                    <thead className="table-light">
                      <tr>
                        <th>Produto</th>
                        <th style={{ width: 80 }}>Qtd</th>
                        <th style={{ width: 110 }}>Preço Unit.</th>
                        <th style={{ width: 80 }}>Desc. %</th>
                        <th style={{ width: 110 }} className="text-end">Subtotal</th>
                        <th style={{ width: 40 }}></th>
                      </tr>
                    </thead>
                    <tbody>
                      {form.itens.map((item, idx) => (
                        <tr key={idx}>
                          <td>
                            <select
                              className="form-select form-select-sm"
                              value={item.produtoId}
                              onChange={e => atualizarItem(idx, 'produtoId', parseInt(e.target.value))}
                            >
                              <option value={0}>—</option>
                              {produtos.map(p => (
                                <option key={p.id} value={p.id}>{p.sku} — {p.nome}</option>
                              ))}
                            </select>
                          </td>
                          <td>
                            <input type="number" min="1" step="1" className="form-control form-control-sm"
                              value={item.quantidade}
                              onChange={e => atualizarItem(idx, 'quantidade', parseFloat(e.target.value) || 1)}
                            />
                          </td>
                          <td>
                            <input type="number" min="0" step="0.01" className="form-control form-control-sm"
                              value={item.precoUnitario}
                              onChange={e => atualizarItem(idx, 'precoUnitario', parseFloat(e.target.value) || 0)}
                            />
                          </td>
                          <td>
                            <input type="number" min="0" max="100" step="0.5" className="form-control form-control-sm"
                              value={item.desconto}
                              onChange={e => atualizarItem(idx, 'desconto', parseFloat(e.target.value) || 0)}
                            />
                          </td>
                          <td className="text-end align-middle fw-semibold">
                            {brl(item.quantidade * item.precoUnitario * (1 - item.desconto / 100))}
                          </td>
                          <td className="align-middle">
                            {form.itens.length > 1 && (
                              <button type="button" className="btn btn-sm btn-link text-danger p-0" onClick={() => removerItem(idx)}>
                                <i className="bi bi-trash3"></i>
                              </button>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                    <tfoot className="table-light">
                      <tr>
                        <td colSpan={4} className="text-end fw-semibold" style={{ fontSize: 12 }}>Total</td>
                        <td className="text-end fw-bold" style={{ fontSize: 13 }}>{brl(totalModal)}</td>
                        <td></td>
                      </tr>
                    </tfoot>
                  </table>
                </div>
                <div className="modal-footer">
                  <button type="button" className="btn btn-sm btn-secondary" onClick={() => setModalAberto(false)}>
                    Cancelar
                  </button>
                  <button type="submit" className="btn btn-sm btn-primary" disabled={criar.isPending}>
                    {criar.isPending ? 'Salvando...' : 'Criar pedido'}
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
