import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { recebimentoApi, type OrdemRecebimentoDto, type ItemRecebimentoDto } from '../api/recebimentoApi'
import type { PedidoCompraDto } from '../api/pedidosCompraApi'

function fmt(iso?: string) {
  if (!iso) return '—'
  const [y, m, d] = iso.slice(0, 10).split('-')
  return `${d}/${m}/${y}`
}

function brl(v: number) {
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

const STATUS_COLORS: Record<string, { bg: string; color: string }> = {
  Aguardando: { bg: '#e3f2fd', color: '#1565c0' },
  Recebendo:  { bg: '#fff8e1', color: '#f57f17' },
  Concluido:  { bg: '#e8f5e9', color: '#2e7d32' },
}

function StatusBadge({ status }: { status: string }) {
  const s = STATUS_COLORS[status] ?? { bg: '#f5f5f5', color: '#616161' }
  return (
    <span style={{ background: s.bg, color: s.color, padding: '3px 10px', borderRadius: 20, fontSize: 11, fontWeight: 600 }}>
      {status}
    </span>
  )
}

// ── Modal de recebimento de itens ─────────────────────────────────────────────
function ModalRecebimento({
  ordem,
  onClose,
}: {
  ordem: OrdemRecebimentoDto
  onClose: () => void
}) {
  const qc = useQueryClient()
  const [quantidades, setQuantidades] = useState<Record<number, string>>(
    Object.fromEntries(ordem.itens.map(i => [i.id, i.qntRecebida > 0 ? String(i.qntRecebida) : String(i.qntEsperada)]))
  )
  const [salvando, setSalvando] = useState<number | null>(null)
  const [erro, setErro]         = useState('')

  const receberItem = useMutation({
    mutationFn: ({ itemId, quantidade }: { itemId: number; quantidade: number }) =>
      recebimentoApi.receberItem(ordem.id, itemId, quantidade),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['ordens-recebimento'] }),
    onError:   (e: any) => setErro(e?.response?.data?.detail ?? 'Erro ao salvar item.'),
  })

  const concluir = useMutation({
    mutationFn: () => recebimentoApi.concluir(ordem.id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['ordens-recebimento'] })
      qc.invalidateQueries({ queryKey: ['pcs-pendentes'] })
      onClose()
    },
    onError: (e: any) => setErro(e?.response?.data?.detail ?? 'Erro ao concluir.'),
  })

  const handleSalvarItem = async (item: ItemRecebimentoDto) => {
    const qnt = parseFloat(quantidades[item.id] ?? '0')
    if (isNaN(qnt) || qnt <= 0) { setErro('Quantidade inválida.'); return }
    setSalvando(item.id)
    setErro('')
    await receberItem.mutateAsync({ itemId: item.id, quantidade: qnt })
    setSalvando(null)
  }

  const concluido = ordem.status === 'Concluido'

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,.45)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div style={{ background: '#fff', borderRadius: 12, width: 680, maxHeight: '85vh', overflow: 'auto', padding: '2rem', boxShadow: '0 8px 32px rgba(0,0,0,.18)' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '1.5rem' }}>
          <div>
            <h5 style={{ margin: 0, fontWeight: 700 }}>Recebimento OR#{ordem.id}</h5>
            <p style={{ margin: '2px 0 0', fontSize: 13, color: '#6c757d' }}>PC#{ordem.pedidoCompraId} · <StatusBadge status={ordem.status} /></p>
          </div>
          <button className="btn btn-sm btn-outline-secondary" onClick={onClose}>✕</button>
        </div>

        {erro && <div className="alert alert-danger py-2 mb-3">{erro}</div>}

        <table className="table table-sm table-hover">
          <thead>
            <tr>
              <th>Produto</th>
              <th className="text-end">Esperado</th>
              <th className="text-end">Recebido</th>
              {!concluido && <th style={{ width: 160 }}>Qtd a Receber</th>}
              {!concluido && <th></th>}
            </tr>
          </thead>
          <tbody>
            {ordem.itens.map(item => (
              <tr key={item.id}>
                <td>{item.nomeProduto || `Produto #${item.produtoId}`}</td>
                <td className="text-end">{item.qntEsperada}</td>
                <td className="text-end">
                  {item.qntRecebida > 0
                    ? <span style={{ color: '#2e7d32', fontWeight: 600 }}>{item.qntRecebida}</span>
                    : <span style={{ color: '#aaa' }}>—</span>}
                </td>
                {!concluido && (
                  <td>
                    <input
                      type="number"
                      min={0}
                      step="0.01"
                      className="form-control form-control-sm"
                      value={quantidades[item.id] ?? ''}
                      onChange={e => setQuantidades(q => ({ ...q, [item.id]: e.target.value }))}
                    />
                  </td>
                )}
                {!concluido && (
                  <td>
                    <button
                      className="btn btn-sm btn-outline-primary"
                      disabled={salvando === item.id}
                      onClick={() => handleSalvarItem(item)}
                    >
                      {salvando === item.id ? '...' : 'Salvar'}
                    </button>
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>

        {!concluido && (
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={onClose}>Fechar</button>
            <button
              className="btn btn-success"
              disabled={concluir.isPending}
              onClick={() => concluir.mutate()}
            >
              {concluir.isPending ? 'Concluindo...' : 'Concluir Recebimento'}
            </button>
          </div>
        )}
        {concluido && (
          <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
            <button className="btn btn-secondary" onClick={onClose}>Fechar</button>
          </div>
        )}
      </div>
    </div>
  )
}

// ── Página principal ──────────────────────────────────────────────────────────
export default function RecebimentoPage() {
  const [filtroStatus, setFiltroStatus] = useState('')
  const [ordemSelecionada, setOrdemSelecionada] = useState<OrdemRecebimentoDto | null>(null)
  const [erroIniciar, setErroIniciar] = useState('')
  const qc = useQueryClient()

  const { data: ordens = [], isLoading } = useQuery({
    queryKey: ['ordens-recebimento'],
    queryFn: recebimentoApi.listar,
  })

  const { data: pendentes = [] } = useQuery({
    queryKey: ['pcs-pendentes'],
    queryFn: recebimentoApi.pendentes,
  })

  const iniciar = useMutation({
    mutationFn: (pedidoCompraId: number) => recebimentoApi.iniciar(pedidoCompraId),
    onSuccess: (nova) => {
      qc.invalidateQueries({ queryKey: ['ordens-recebimento'] })
      qc.invalidateQueries({ queryKey: ['pcs-pendentes'] })
      setErroIniciar('')
      setOrdemSelecionada(nova)
    },
    onError: (e: any) => setErroIniciar(e?.response?.data?.detail ?? 'Erro ao iniciar recebimento.'),
  })

  const ordensFiltradas = filtroStatus
    ? ordens.filter(o => o.status === filtroStatus)
    : ordens

  const emRecebendo = ordens.filter(o => o.status === 'Recebendo').length
  const concluidas  = ordens.filter(o => o.status === 'Concluido').length

  return (
    <div>
      {/* KPIs */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 16, marginBottom: 24 }}>
        {[
          { label: 'Disponíveis para Receber', value: pendentes.length, color: '#1565c0' },
          { label: 'Em Recebimento',           value: emRecebendo,      color: '#f57f17' },
          { label: 'Concluídas',               value: concluidas,       color: '#2e7d32' },
        ].map(k => (
          <div key={k.label} style={{ background: '#fff', border: '1px solid #e9ecef', borderRadius: 10, padding: '1rem 1.25rem' }}>
            <div style={{ fontSize: 22, fontWeight: 700, color: k.color }}>{k.value}</div>
            <div style={{ fontSize: 12, color: '#6c757d', marginTop: 2 }}>{k.label}</div>
          </div>
        ))}
      </div>

      {erroIniciar && <div className="alert alert-danger">{erroIniciar}</div>}

      {/* PCs Disponíveis */}
      {pendentes.length > 0 && (
        <div style={{ background: '#fff', border: '1px solid #e9ecef', borderRadius: 10, padding: '1.25rem', marginBottom: 24 }}>
          <h6 style={{ fontWeight: 700, marginBottom: 12 }}>
            <i className="bi bi-truck me-2" style={{ color: '#1565c0' }}></i>
            Pedidos de Compra Disponíveis para Receber
          </h6>
          <table className="table table-sm table-hover mb-0">
            <thead>
              <tr>
                <th>PC#</th>
                <th>Fornecedor ID</th>
                <th>Aprovação</th>
                <th className="text-end">Total</th>
                <th className="text-end">Itens</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {pendentes.map((pc: PedidoCompraDto) => (
                <tr key={pc.id}>
                  <td><strong>#{pc.id}</strong></td>
                  <td>Fornecedor #{pc.fornecedorId}</td>
                  <td>{fmt(pc.dataAprovacao)}</td>
                  <td className="text-end">{brl(pc.total)}</td>
                  <td className="text-end">{pc.itens?.length ?? '—'}</td>
                  <td className="text-end">
                    <button
                      className="btn btn-sm btn-primary"
                      disabled={iniciar.isPending}
                      onClick={() => iniciar.mutate(pc.id)}
                    >
                      <i className="bi bi-box-arrow-in-down me-1"></i>
                      Iniciar Recebimento
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Lista de ordens */}
      <div style={{ background: '#fff', border: '1px solid #e9ecef', borderRadius: 10, padding: '1.25rem' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16 }}>
          <h6 style={{ fontWeight: 700, margin: 0 }}>
            <i className="bi bi-clipboard-check me-2"></i>Ordens de Recebimento
          </h6>
          <div style={{ display: 'flex', gap: 8 }}>
            {['', 'Recebendo', 'Concluido'].map(s => (
              <button
                key={s}
                className={`btn btn-sm ${filtroStatus === s ? 'btn-dark' : 'btn-outline-secondary'}`}
                onClick={() => setFiltroStatus(s)}
              >
                {s || 'Todos'}
              </button>
            ))}
          </div>
        </div>

        {isLoading && <div className="text-center py-4"><div className="spinner-border spinner-border-sm"></div></div>}

        {!isLoading && ordensFiltradas.length === 0 && (
          <div className="amr-empty">
            <i className="bi bi-inbox"></i>
            <div>Nenhuma ordem encontrada</div>
          </div>
        )}

        {!isLoading && ordensFiltradas.length > 0 && (
          <table className="table table-hover mb-0">
            <thead>
              <tr>
                <th>OR#</th>
                <th>PC#</th>
                <th>Status</th>
                <th>Criação</th>
                <th>Recebimento</th>
                <th className="text-end">Itens</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {ordensFiltradas.map(o => (
                <tr key={o.id}>
                  <td><strong>#{o.id}</strong></td>
                  <td>PC#{o.pedidoCompraId}</td>
                  <td><StatusBadge status={o.status} /></td>
                  <td>{fmt(o.dataCriacao)}</td>
                  <td>{fmt(o.dataRecebimento)}</td>
                  <td className="text-end">{o.itens.length}</td>
                  <td className="text-end">
                    <button
                      className="btn btn-sm btn-outline-secondary"
                      onClick={() => setOrdemSelecionada(o)}
                    >
                      <i className="bi bi-eye me-1"></i>
                      {o.status === 'Recebendo' ? 'Receber Itens' : 'Ver Detalhes'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {ordemSelecionada && (
        <ModalRecebimento
          ordem={ordemSelecionada}
          onClose={() => setOrdemSelecionada(null)}
        />
      )}
    </div>
  )
}
