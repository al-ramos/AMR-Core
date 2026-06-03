import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { movimentosEstoqueApi, MovimentoEstoqueDto } from '../api/movimentosEstoqueApi'

const TIPOS = ['', 'Entrada', 'Saida', 'AjusteManual']

const TIPO_CONFIG: Record<string, { label: string; cls: string }> = {
  Entrada:      { label: 'Entrada',  cls: 'badge-entrada'  },
  Saida:        { label: 'Saída',    cls: 'badge-saida'    },
  AjusteManual: { label: 'Ajuste',   cls: 'badge-ajuste'   },
}

function TipoBadge({ tipo }: { tipo: MovimentoEstoqueDto['tipo'] }) {
  const cfg = TIPO_CONFIG[tipo] ?? { label: tipo, cls: '' }
  return (
    <span className={`badge rounded-pill fw-semibold ${cfg.cls}`} style={{ fontSize: 11, padding: '4px 10px' }}>
      {cfg.label}
    </span>
  )
}

function fmtQtd(v: number, tipo: MovimentoEstoqueDto['tipo']) {
  const sinal = tipo === 'Saida' ? '−' : '+'
  return `${sinal}${v.toLocaleString('pt-BR')}`
}

function fmtData(iso: string) {
  return new Date(iso).toLocaleString('pt-BR', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  })
}

export default function MovimentosEstoquePage() {
  const [filtroTipo, setFiltroTipo] = useState('')

  const { data: movimentos = [], isLoading, isError, error } = useQuery({
    queryKey: ['movimentos-estoque', filtroTipo],
    queryFn: () => movimentosEstoqueApi.listar({
      empresaId: 1,
      tipo: filtroTipo || undefined,
    }),
  })

  const entradas = movimentos.filter(m => m.tipo === 'Entrada').length
  const saidas   = movimentos.filter(m => m.tipo === 'Saida').length
  const ajustes  = movimentos.filter(m => m.tipo === 'AjusteManual').length

  return (
    <>
      <div className="row g-3 mb-4">
        {[
          { label: 'Total',    value: movimentos.length, color: '#212529' },
          { label: 'Entradas', value: entradas,          color: '#2e7d32' },
          { label: 'Saídas',   value: saidas,            color: '#c62828' },
          { label: 'Ajustes',  value: ajustes,           color: '#e65100' },
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
        <div className="d-flex align-items-center justify-content-between px-3 py-3 border-bottom flex-wrap gap-2">
          <span style={{ fontSize: 13, fontWeight: 600, color: '#495057' }}>
            <i className="bi bi-arrow-left-right me-2"></i>Movimentos de estoque
          </span>
          <select
            className="form-select form-select-sm"
            style={{ width: 180, fontSize: 13 }}
            value={filtroTipo}
            onChange={e => setFiltroTipo(e.target.value)}
          >
            <option value="">Todos os tipos</option>
            {TIPOS.filter(Boolean).map(t => (
              <option key={t} value={t}>{TIPO_CONFIG[t]?.label ?? t}</option>
            ))}
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
              {(error as Error)?.message ?? 'Erro ao carregar movimentos.'}
            </div>
          </div>
        )}

        {!isLoading && !isError && movimentos.length === 0 && (
          <div className="amr-empty">
            <i className="bi bi-arrow-left-right"></i>
            <div style={{ fontSize: 14, fontWeight: 500 }}>Nenhum movimento registrado</div>
          </div>
        )}

        {!isLoading && movimentos.length > 0 && (
          <div className="table-responsive">
            <table className="table table-hover table-sm mb-0" style={{ fontSize: 13 }}>
              <thead className="table-light">
                <tr>
                  <th>#</th>
                  <th>Produto</th>
                  <th>Tipo</th>
                  <th className="text-end">Quantidade</th>
                  <th>Origem</th>
                  <th>Data/Hora</th>
                </tr>
              </thead>
              <tbody>
                {movimentos.map(m => (
                  <tr key={m.id}>
                    <td className="font-monospace text-muted" style={{ fontSize: 12 }}>{m.id}</td>
                    <td className="fw-medium">{m.produtoNome}</td>
                    <td><TipoBadge tipo={m.tipo} /></td>
                    <td className={`text-end fw-semibold ${m.tipo === 'Saida' ? 'text-danger' : 'text-success'}`}>
                      {fmtQtd(m.quantidade, m.tipo)}
                    </td>
                    <td className="text-muted font-monospace" style={{ fontSize: 12 }}>{m.origem ?? '—'}</td>
                    <td className="text-muted" style={{ fontSize: 12 }}>{fmtData(m.dataHora)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </>
  )
}
