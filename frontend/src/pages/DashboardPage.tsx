import { useQuery } from '@tanstack/react-query'
import { produtosApi } from '../api/produtosApi'
import { pedidosCompraApi } from '../api/pedidosCompraApi'
import { pedidosVendaApi } from '../api/pedidosVendaApi'

const EMPRESA_ID = 1

function brl(v: number) {
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

function KpiCard({
  icon, label, value, color, sub,
}: { icon: string; label: string; value: string | number; color: string; sub?: string }) {
  return (
    <div className="amr-metric-card" style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 4 }}>
        <i className={`bi ${icon}`} style={{ fontSize: 18, color }}></i>
        <span style={{ fontSize: 11, color: '#6c757d', fontWeight: 500 }}>{label}</span>
      </div>
      <p className="amr-metric-value mb-0" style={{ color }}>{value}</p>
      {sub && <p style={{ fontSize: 11, color: '#adb5bd', margin: 0 }}>{sub}</p>}
    </div>
  )
}

function SectionTitle({ icon, label }: { icon: string; label: string }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 12 }}>
      <i className={`bi ${icon}`} style={{ color: '#546e7a' }}></i>
      <span style={{ fontSize: 12, fontWeight: 700, color: '#495057', textTransform: 'uppercase', letterSpacing: 1 }}>
        {label}
      </span>
    </div>
  )
}

export default function DashboardPage() {
  const { data: produtos = [], isLoading: loadP } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
  })

  const { data: pedidosCompra = [], isLoading: loadC } = useQuery({
    queryKey: ['pedidos-compra', EMPRESA_ID, ''],
    queryFn: () => pedidosCompraApi.listar(EMPRESA_ID),
  })

  const { data: pedidosVenda = [], isLoading: loadV } = useQuery({
    queryKey: ['pedidos-venda', EMPRESA_ID, ''],
    queryFn: () => pedidosVendaApi.listar(EMPRESA_ID),
  })

  const isLoading = loadP || loadC || loadV

  // Produtos
  const prodAtivos   = produtos.filter(p => p.ativo).length
  const prodInativos = produtos.length - prodAtivos

  // Pedidos de Compra
  const pcRascunho = pedidosCompra.filter(p => p.status === 'Rascunho').length
  const pcAprovado = pedidosCompra.filter(p => p.status === 'Aprovado').length
  const pcRecebido = pedidosCompra.filter(p => p.status === 'Recebido').length
  const pcTotal    = pedidosCompra.reduce((s, p) => s + p.total, 0)

  // Pedidos de Venda
  const pvRascunho = pedidosVenda.filter(p => p.status === 'Rascunho').length
  const pvAprovado = pedidosVenda.filter(p => p.status === 'Aprovado').length
  const pvFaturado = pedidosVenda.filter(p => p.status === 'Faturado').length
  const pvTotalFaturado = pedidosVenda
    .filter(p => p.status === 'Faturado')
    .reduce((s, p) => s + p.total, 0)
  const pvTotalGeral = pedidosVenda.reduce((s, p) => s + p.total, 0)

  if (isLoading) {
    return (
      <div className="amr-empty">
        <div className="spinner-border spinner-border-sm text-primary mb-2" role="status"></div>
        <span style={{ fontSize: 13 }}>Carregando dashboard...</span>
      </div>
    )
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 32 }}>

      {/* Produtos */}
      <div>
        <SectionTitle icon="bi-box-seam" label="Produtos" />
        <div className="row g-3">
          <div className="col-md-4">
            <KpiCard icon="bi-boxes" label="Total de produtos" value={produtos.length} color="#212529" />
          </div>
          <div className="col-md-4">
            <KpiCard icon="bi-check-circle" label="Ativos" value={prodAtivos} color="#2e7d32" />
          </div>
          <div className="col-md-4">
            <KpiCard icon="bi-pause-circle" label="Inativos" value={prodInativos} color="#757575" />
          </div>
        </div>
      </div>

      {/* Pedidos de Compra */}
      <div>
        <SectionTitle icon="bi-truck" label="Pedidos de Compra" />
        <div className="row g-3">
          <div className="col-md-3">
            <KpiCard icon="bi-file-earmark" label="Em rascunho" value={pcRascunho} color="#1565c0"
              sub={`${pedidosCompra.length} pedidos no total`} />
          </div>
          <div className="col-md-3">
            <KpiCard icon="bi-check2-circle" label="Aprovados" value={pcAprovado} color="#e65100"
              sub="Aguardando entrega" />
          </div>
          <div className="col-md-3">
            <KpiCard icon="bi-box-arrow-in-down" label="Recebidos" value={pcRecebido} color="#6a1b9a" />
          </div>
          <div className="col-md-3">
            <KpiCard icon="bi-currency-dollar" label="Valor total" value={brl(pcTotal)} color="#2e7d32" />
          </div>
        </div>
      </div>

      {/* Pedidos de Venda */}
      <div>
        <SectionTitle icon="bi-cart-check" label="Pedidos de Venda" />
        <div className="row g-3">
          <div className="col-md-3">
            <KpiCard icon="bi-file-earmark" label="Em rascunho" value={pvRascunho} color="#1565c0"
              sub={`${pedidosVenda.length} pedidos no total`} />
          </div>
          <div className="col-md-3">
            <KpiCard icon="bi-check2-circle" label="Aprovados" value={pvAprovado} color="#e65100"
              sub="Aguardando faturamento" />
          </div>
          <div className="col-md-3">
            <KpiCard icon="bi-receipt" label="Faturados" value={pvFaturado} color="#2e7d32" />
          </div>
          <div className="col-md-3">
            <KpiCard icon="bi-graph-up-arrow" label="Total faturado" value={brl(pvTotalFaturado)} color="#2e7d32"
              sub={`Geral: ${brl(pvTotalGeral)}`} />
          </div>
        </div>
      </div>

    </div>
  )
}
