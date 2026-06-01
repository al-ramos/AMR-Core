import { BrowserRouter, Routes, Route, NavLink, useLocation } from 'react-router-dom'
import ProdutosPage from './pages/ProdutosPage'
import PedidosVendaPage from './pages/PedidosVendaPage'
import PedidosCompraPage from './pages/PedidosCompraPage'

const NAV = [
  { section: 'Estoque', items: [
    { to: '/',        icon: 'bi-box-seam',    label: 'Produtos',        end: true  },
  ]},
  { section: 'Comercial', items: [
    { to: '/pv',      icon: 'bi-cart-check',  label: 'Pedidos de Venda', end: false },
    { to: '/pc',      icon: 'bi-truck',       label: 'Pedidos de Compra', end: false },
  ]},
  { section: 'Relatorios', items: [
    { to: '/dashboard', icon: 'bi-bar-chart-line', label: 'Dashboard', end: false },
  ]},
]

const PAGE_LABELS: Record<string, { title: string; subtitle: string }> = {
  '/':          { title: 'Produtos',          subtitle: 'Catalogo e estoque'         },
  '/pv':        { title: 'Pedidos de Venda',  subtitle: 'Gestao de vendas'           },
  '/pc':        { title: 'Pedidos de Compra', subtitle: 'Gestao de compras'          },
  '/dashboard': { title: 'Dashboard',         subtitle: 'Visao gerencial'            },
}

function Sidebar() {
  return (
    <nav className="amr-sidebar">
      <a href="/" className="amr-sidebar-brand">
        AMR <span>Core</span>
      </a>
      {NAV.map(group => (
        <div key={group.section}>
          <p className="amr-sidebar-section">{group.section}</p>
          {group.items.map(item => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.end}
              className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}
            >
              <i className={`bi ${item.icon}`}></i>
              {item.label}
            </NavLink>
          ))}
        </div>
      ))}
      <div style={{ marginTop: 'auto', padding: '1rem 1.25rem', borderTop: '1px solid rgba(255,255,255,.06)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <div style={{
            width: 30, height: 30, borderRadius: '50%',
            background: 'var(--amr-sidebar-active)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            fontSize: 12, fontWeight: 700, color: '#fff',
          }}>A</div>
          <div>
            <div style={{ fontSize: 12, color: '#cfd8dc', fontWeight: 500 }}>AMR Sistema</div>
            <div style={{ fontSize: 10, color: '#546e7a' }}>Core</div>
          </div>
        </div>
      </div>
    </nav>
  )
}

function Topbar() {
  const loc = useLocation()
  const key = Object.keys(PAGE_LABELS)
    .filter(k => k !== '/')
    .find(k => loc.pathname.startsWith(k)) ?? loc.pathname
  const info = PAGE_LABELS[key] ?? PAGE_LABELS['/']
  return (
    <header className="amr-topbar">
      <div style={{ flex: 1 }}>
        <p className="amr-topbar-title">{info.title}</p>
        {info.subtitle && <p className="amr-topbar-subtitle">{info.subtitle}</p>}
      </div>
      <span style={{ fontSize: 11, color: '#adb5bd' }}>
        <i className="bi bi-circle-fill" style={{ color: '#4caf50', fontSize: 8, marginRight: 4 }}></i>
        Online
      </span>
    </header>
  )
}

function PlaceholderPage({ label }: { label: string }) {
  return (
    <div className="amr-empty">
      <i className="bi bi-tools"></i>
      <div style={{ fontSize: 15, fontWeight: 600, color: '#495057' }}>{label}</div>
      <div style={{ fontSize: 13, marginTop: 4 }}>Em desenvolvimento</div>
    </div>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <div className="amr-wrapper">
        <Sidebar />
        <div className="amr-content-wrapper">
          <Topbar />
          <main className="amr-content">
            <Routes>
              <Route path="/"          element={<ProdutosPage />} />
              <Route path="/pv"        element={<PedidosVendaPage />} />
              <Route path="/pc"        element={<PedidosCompraPage />} />
              <Route path="/dashboard" element={<PlaceholderPage label="Dashboard" />} />
            </Routes>
          </main>
        </div>
      </div>
    </BrowserRouter>
  )
}
