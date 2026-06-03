import { useState, useMemo } from 'react'
import { useQuery } from '@tanstack/react-query'
import { produtosApi } from '../api/produtosApi'
import { usePagination } from '../hooks/usePagination'
import { Pagination } from '../components/Pagination'

function brl(v: number) {
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

function StatusBadge({ ativo }: { ativo: boolean }) {
  return ativo
    ? <span className="badge rounded-pill badge-ativo fw-semibold" style={{ fontSize: 11, padding: '4px 10px' }}>Ativo</span>
    : <span className="badge rounded-pill badge-inativo fw-semibold" style={{ fontSize: 11, padding: '4px 10px' }}>Inativo</span>
}

export default function ProdutosPage() {
  const [search, setSearch] = useState('')

  const { data: produtos = [], isLoading, isError, error } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
  })

  // Filtro por nome ou SKU em tempo real
  const filtrados = useMemo(() => {
    const q = search.trim().toLowerCase()
    if (!q) return produtos
    return produtos.filter(p =>
      p.nome.toLowerCase().includes(q) ||
      p.sku.toLowerCase().includes(q)
    )
  }, [produtos, search])

  const { paged, page, pageSize, totalPages, total, changePage, changePageSize } = usePagination(filtrados)

  const ativos   = produtos.filter(p => p.ativo).length
  const inativos = produtos.length - ativos

  return (
    <>
      <div className="row g-3 mb-4">
        {[
          { label: 'Total de produtos', value: produtos.length, color: '#212529' },
          { label: 'Ativos',            value: ativos,          color: '#2e7d32' },
          { label: 'Inativos',          value: inativos,        color: '#757575' },
        ].map(m => (
          <div key={m.label} className="col-md-4">
            <div className="amr-metric-card">
              <p className="amr-metric-label">{m.label}</p>
              <p className="amr-metric-value" style={{ color: m.color }}>{m.value}</p>
            </div>
          </div>
        ))}
      </div>

      <div className="amr-table-card">
        <div className="d-flex align-items-center justify-content-between px-3 py-3 border-bottom gap-3">
          <span style={{ fontSize: 13, fontWeight: 600, color: '#495057', whiteSpace: 'nowrap' }}>
            <i className="bi bi-box-seam me-2"></i>Catálogo de produtos
          </span>
          {/* Campo de busca por nome/SKU */}
          <div className="input-group input-group-sm" style={{ maxWidth: 280 }}>
            <span className="input-group-text"><i className="bi bi-search"></i></span>
            <input
              type="text"
              className="form-control"
              placeholder="Buscar por nome ou SKU…"
              value={search}
              onChange={e => { setSearch(e.target.value); changePage(1) }}
            />
            {search && (
              <button className="btn btn-outline-secondary" onClick={() => setSearch('')}>
                <i className="bi bi-x"></i>
              </button>
            )}
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
              {(error as Error)?.message ?? 'Erro ao carregar produtos.'}
            </div>
          </div>
        )}

        {!isLoading && !isError && filtrados.length === 0 && (
          <div className="amr-empty">
            <i className="bi bi-box-seam"></i>
            <div style={{ fontSize: 14, fontWeight: 500 }}>
              {search ? `Nenhum produto encontrado para "${search}"` : 'Nenhum produto cadastrado'}
            </div>
          </div>
        )}

        {!isLoading && filtrados.length > 0 && (
          <>
            <div className="table-responsive">
              <table className="table table-hover table-sm mb-0" style={{ fontSize: 13 }}>
                <thead className="table-light">
                  <tr>
                    <th>SKU</th>
                    <th>Nome</th>
                    <th>Descrição</th>
                    <th>Unidade</th>
                    <th className="text-end">Preço Unit.</th>
                    <th className="text-end">Est. Mínimo</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {paged.map(p => (
                    <tr key={p.id}>
                      <td className="font-monospace text-muted" style={{ fontSize: 12 }}>{p.sku}</td>
                      <td className="fw-medium">{p.nome}</td>
                      <td className="text-muted" style={{ maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {p.descricao ?? '—'}
                      </td>
                      <td>{p.unidadeMedida}</td>
                      <td className="text-end">{brl(p.precoUnitario)}</td>
                      <td className="text-end">{p.estoqueMinimo}</td>
                      <td><StatusBadge ativo={p.ativo} /></td>
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
    </>
  )
}
