import { useQuery } from '@tanstack/react-query'
import { produtosApi } from '../api/produtosApi'
import { pedidosCompraApi } from '../api/pedidosCompraApi'
import { pedidosVendaApi } from '../api/pedidosVendaApi'

const EMPRESA_ID = 1

function brl(v: number) {
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

interface MetricCardProps {
  label: string
  value: string | number
  color?: string
  icon?: string
}

function MetricCard({ label, value, color = '#212529', icon }: MetricCardProps) {
  return (
    <div className="col-md-3 col-sm-6">
      <div className="amr-metric-card">
        {icon && <i className={`bi ${icon}`} style={{ fontSize: 18, color, marginBottom: 6, display: 'block' }}></i>}
        <p className="amr-metric-label">{label}</p>
        <p className="amr-metric-value" style={{ color }}>{value}</p>
      </div>
    </div>
  )
}

function Section({ title, icon, children }: { title: string; icon: string; children: React.ReactNode }) {
  return (
    <div className="amr-table-card mb-4">
      <div className="px-3 py-3 border-bottom">
        <span style={{ fontSize: 13, fontWeight: 600, color: '#495057' }}>
          <i className={`bi ${icon} me-2`}></i>{title}
        </span>
      </div>
      <div className="p-3">
        <div className="row g-3">{children}</div>
      </div>
    </div>
  )
}

export default function DashboardPage() {
  const { data: produtos = [], isLoading: loadProd } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
  })

  const { data: compras = [], isLoading: loadPC } = useQuery({
    queryKey: ['pedidos-compra', EMPRESA_ID, undefined],
    queryFn: () => pedidosCompraApi.listar(EMPRESA_ID),
  })

  const { data: vendas = [], isLoading: loadPV } = useQuery({
    queryKey: ['pedidos-venda', EMPRESA_ID, undefined],
    queryFn: () => pedidosVendaApi.listar(EMPRESA_ID),
  })

  const isLoading = loadProd || loadPC || loadPV

  const ativos   = produtos.filter(p => p.ativo).length
  const inativos = produtos.filter(p => !p.ativo).length

  const pcRascunho = compras.filter(p => p.status === 'Rascunho').length
  const pcAprovado = compras.filter(p => p.status === 'Aprovado').length
  const pcRecebido = compras.filter(p => p.status === 'Recebido').length
  const pcTotal    = compras.reduce((s, p) => s + p.total, 0)

  const pvRascunho = vendas.filter(p => p.status === 'Rascunho').length
  const pvAprovado = vendas.filter(p => p.status === 'Aprovado').length
  const pvFaturado = vendas.filter(p => p.status === 'Faturado').length
  const pvFaturado$ = vendas.filter(p => p.status === 'Faturado').reduce((s, p) => s + p.total, 0)
  const pvTotal    = vendas.reduce((s, p) => s + p.total, 0)

  if (isLoading) {
    return (
      <div className="amr-empty">
        <div className="spinner-border spinner-border-sm text-primary mb-2" role="status"></div>
        <span style={{ fontSize: 13 }}>Carregando dashboard...</span>
      </div>
    )
  }

  return (
    <>
      <Section title="Estoque — Produtos" icon="bi-box-seam">
        <MetricCard label="Total de produtos" value={produtos.length} color="#212529" />
        <MetricCard label="Ativos"             value={ativos}          color="#2e7d32" />
        <MetricCard label="Inativos"           value={inativos}        color="#c62828" />
      </Section>

      <Section title="Compras — Pedidos de Compra" icon="bi-truck">
        <MetricCard label="Em rascunho" value={pcRascunho} color="#616161" />
        <MetricCard label="Aprovados"   value={pcAprovado} color="#1565c0" />
        <MetricCard label="Recebidos"   value={pcRecebido} color="#2e7d32" />
        <MetricCard label="Valor total" value={brl(pcTotal)} color="#1565c0" />
      </Section>

      <Section title="Vendas — Pedidos de Venda" icon="bi-cart-check">
        <MetricCard label="Em rascunho"   value={pvRascunho}   color="#616161" />
        <MetricCard label="Aprovados"     value={pvAprovado}   color="#1565c0" />
        <MetricCard label="Faturados"     value={pvFaturado}   color="#2e7d32" />
        <MetricCard label="Total faturado" value={brl(pvFaturado$)} color="#2e7d32" />
        <MetricCard label="Total geral"   value={brl(pvTotal)} color="#1565c0" />
      </Section>
    </>
  )
}
