import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { produtosApi, type CriarProdutoPayload } from '../api/produtosApi'
import { unidadesMedidaApi } from '../api/unidadesMedidaApi'

const EMPRESA_ID = 1

function brl(v: number) {
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

function StatusBadge({ ativo }: { ativo: boolean }) {
  return ativo
    ? <span className="badge rounded-pill badge-ativo fw-semibold" style={{ fontSize: 11, padding: '4px 10px' }}>Ativo</span>
    : <span className="badge rounded-pill badge-inativo fw-semibold" style={{ fontSize: 11, padding: '4px 10px' }}>Inativo</span>
}

const FORM_VAZIO: CriarProdutoPayload = {
  sku: '', nome: '', descricao: '', precoUnitario: 0,
  estoqueMinimo: 0, unidadeMedidaId: 0, empresaId: EMPRESA_ID,
}

export default function ProdutosPage() {
  const [modalAberto, setModalAberto] = useState(false)
  const [form, setForm]               = useState<CriarProdutoPayload>(FORM_VAZIO)
  const [erro, setErro]               = useState<string | null>(null)
  const qc = useQueryClient()

  const { data: produtos = [], isLoading, isError, error } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
  })

  const { data: unidades = [] } = useQuery({
    queryKey: ['unidades-medida'],
    queryFn: unidadesMedidaApi.listar,
  })

  const criar = useMutation({
    mutationFn: produtosApi.criar,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['produtos'] })
      setModalAberto(false)
      setForm(FORM_VAZIO)
      setErro(null)
    },
    onError: (e: unknown) => {
      setErro((e as Error)?.message ?? 'Erro ao criar produto.')
    },
  })

  const ativos   = produtos.filter(p => p.ativo).length
  const inativos = produtos.length - ativos

  function submitForm(e: React.FormEvent) {
    e.preventDefault()
    setErro(null)
    if (!form.sku.trim() || !form.nome.trim() || !form.unidadeMedidaId) {
      setErro('SKU, Nome e Unidade de Medida são obrigatórios.')
      return
    }
    criar.mutate(form)
  }

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
        <div className="d-flex align-items-center justify-content-between px-3 py-3 border-bottom">
          <span style={{ fontSize: 13, fontWeight: 600, color: '#495057' }}>
            <i className="bi bi-box-seam me-2"></i>Catalogo de produtos
          </span>
          <button
            className="btn btn-sm btn-primary"
            style={{ fontSize: 12 }}
            onClick={() => { setModalAberto(true); setErro(null) }}
          >
            <i className="bi bi-plus-lg me-1"></i>Novo produto
          </button>
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

        {!isLoading && !isError && produtos.length === 0 && (
          <div className="amr-empty">
            <i className="bi bi-box-seam"></i>
            <div style={{ fontSize: 14, fontWeight: 500 }}>Nenhum produto cadastrado</div>
          </div>
        )}

        {!isLoading && produtos.length > 0 && (
          <div className="table-responsive">
            <table className="table table-hover table-sm mb-0" style={{ fontSize: 13 }}>
              <thead className="table-light">
                <tr>
                  <th>SKU</th>
                  <th>Nome</th>
                  <th>Descricao</th>
                  <th>Unidade</th>
                  <th className="text-end">Preco Unit.</th>
                  <th className="text-end">Est. Minimo</th>
                  <th>Status</th>
                </tr>
              </thead>
              <tbody>
                {produtos.map(p => (
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
        )}
      </div>

      {/* ── Modal Novo Produto ─────────────────────────────────────────────── */}
      {modalAberto && (
        <div className="modal show d-block" style={{ background: 'rgba(0,0,0,.4)' }}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content">
              <form onSubmit={submitForm}>
                <div className="modal-header">
                  <h5 className="modal-title" style={{ fontSize: 15 }}>
                    <i className="bi bi-box-seam me-2"></i>Novo produto
                  </h5>
                  <button type="button" className="btn-close" onClick={() => setModalAberto(false)}></button>
                </div>
                <div className="modal-body">
                  {erro && (
                    <div className="alert alert-danger py-2" style={{ fontSize: 13 }}>{erro}</div>
                  )}
                  <div className="row g-3">
                    <div className="col-md-4">
                      <label className="form-label" style={{ fontSize: 12 }}>SKU *</label>
                      <input
                        className="form-control form-control-sm"
                        value={form.sku}
                        onChange={e => setForm(f => ({ ...f, sku: e.target.value }))}
                        placeholder="EX: PARAF-M6"
                      />
                    </div>
                    <div className="col-md-8">
                      <label className="form-label" style={{ fontSize: 12 }}>Nome *</label>
                      <input
                        className="form-control form-control-sm"
                        value={form.nome}
                        onChange={e => setForm(f => ({ ...f, nome: e.target.value }))}
                        placeholder="Nome do produto"
                      />
                    </div>
                    <div className="col-12">
                      <label className="form-label" style={{ fontSize: 12 }}>Descrição</label>
                      <input
                        className="form-control form-control-sm"
                        value={form.descricao ?? ''}
                        onChange={e => setForm(f => ({ ...f, descricao: e.target.value }))}
                        placeholder="Descrição opcional"
                      />
                    </div>
                    <div className="col-md-5">
                      <label className="form-label" style={{ fontSize: 12 }}>Preço Unitário *</label>
                      <input
                        type="number" min="0" step="0.01"
                        className="form-control form-control-sm"
                        value={form.precoUnitario}
                        onChange={e => setForm(f => ({ ...f, precoUnitario: parseFloat(e.target.value) || 0 }))}
                      />
                    </div>
                    <div className="col-md-4">
                      <label className="form-label" style={{ fontSize: 12 }}>Estoque Mínimo</label>
                      <input
                        type="number" min="0" step="1"
                        className="form-control form-control-sm"
                        value={form.estoqueMinimo}
                        onChange={e => setForm(f => ({ ...f, estoqueMinimo: parseFloat(e.target.value) || 0 }))}
                      />
                    </div>
                    <div className="col-md-3">
                      <label className="form-label" style={{ fontSize: 12 }}>Unidade *</label>
                      <select
                        className="form-select form-select-sm"
                        value={form.unidadeMedidaId}
                        onChange={e => setForm(f => ({ ...f, unidadeMedidaId: parseInt(e.target.value) }))}
                      >
                        <option value={0}>—</option>
                        {unidades.map(u => (
                          <option key={u.id} value={u.id}>{u.sigla}</option>
                        ))}
                      </select>
                    </div>
                  </div>
                </div>
                <div className="modal-footer">
                  <button type="button" className="btn btn-sm btn-secondary" onClick={() => setModalAberto(false)}>
                    Cancelar
                  </button>
                  <button type="submit" className="btn btn-sm btn-primary" disabled={criar.isPending}>
                    {criar.isPending ? 'Salvando...' : 'Salvar produto'}
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
