import { useState, useMemo } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { produtosApi, type ProdutoDto } from '../api/produtosApi'
import { dropdownApi } from '../api/dropdownApi'
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

interface FormState {
  sku: string; nome: string; descricao: string
  precoUnitario: string; estoqueMinimo: string; unidadeMedidaId: string
}
const FORM_VAZIO: FormState = { sku: '', nome: '', descricao: '', precoUnitario: '', estoqueMinimo: '0', unidadeMedidaId: '' }

function produtoParaForm(p: ProdutoDto, unidadeId: number): FormState {
  return {
    sku: p.sku, nome: p.nome, descricao: p.descricao ?? '',
    precoUnitario: String(p.precoUnitario), estoqueMinimo: String(p.estoqueMinimo),
    unidadeMedidaId: String(unidadeId),
  }
}

export default function ProdutosPage() {
  const [modal, setModal]       = useState<'novo' | 'editar' | null>(null)
  const [editando, setEditando] = useState<ProdutoDto | null>(null)
  const [form, setForm]         = useState<FormState>(FORM_VAZIO)
  const [erro, setErro]         = useState('')
  const [search, setSearch]     = useState('')
  const qc = useQueryClient()

  const { data: produtos = [], isLoading, isError, error } = useQuery({
    queryKey: ['produtos'],
    queryFn: produtosApi.listar,
  })

  const { data: unidades = [] } = useQuery({
    queryKey: ['unidades-medida'],
    queryFn: dropdownApi.unidadesMedida,
  })

  const criar = useMutation({
    mutationFn: produtosApi.criar,
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['produtos'] }); fecharModal() },
    onError: (e: Error) => setErro(e.message),
  })

  const atualizar = useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: Parameters<typeof produtosApi.atualizar>[1] }) =>
      produtosApi.atualizar(id, payload),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['produtos'] }); fecharModal() },
    onError: (e: Error) => setErro(e.message),
  })

  const inativar = useMutation({
    mutationFn: produtosApi.inativar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['produtos'] }),
  })

  const reativar = useMutation({
    mutationFn: produtosApi.reativar,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['produtos'] }),
  })

  // Filtro por nome/SKU em tempo real
  const filtrados = useMemo(() => {
    const q = search.trim().toLowerCase()
    if (!q) return produtos
    return produtos.filter(p =>
      p.nome.toLowerCase().includes(q) || p.sku.toLowerCase().includes(q)
    )
  }, [produtos, search])

  const { paged, page, pageSize, totalPages, total, changePage, changePageSize } = usePagination(filtrados)

  function abrirNovo() {
    setForm(FORM_VAZIO); setErro(''); setEditando(null); setModal('novo')
  }

  function abrirEditar(p: ProdutoDto) {
    const unidade = unidades.find(u => u.sigla === p.unidadeMedida)
    setForm(produtoParaForm(p, unidade?.id ?? 0)); setErro(''); setEditando(p); setModal('editar')
  }

  function fecharModal() { setModal(null); setEditando(null); setForm(FORM_VAZIO); setErro('') }

  function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) {
    setForm(f => ({ ...f, [e.target.name]: e.target.value }))
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault(); setErro('')
    const payload = {
      nome: form.nome, descricao: form.descricao || undefined,
      precoUnitario: parseFloat(form.precoUnitario) || 0,
      estoqueMinimo: parseFloat(form.estoqueMinimo) || 0,
      unidadeMedidaId: parseInt(form.unidadeMedidaId),
    }
    if (modal === 'novo') {
      criar.mutate({ ...payload, sku: form.sku, empresaId: 1 })
    } else if (editando) {
      atualizar.mutate({ id: editando.id, payload })
    }
  }

  const ativos    = produtos.filter(p => p.ativo).length
  const inativos  = produtos.length - ativos
  const isPending = criar.isPending || atualizar.isPending

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
          <div className="d-flex gap-2 align-items-center">
            {/* Busca por nome/SKU */}
            <div className="input-group input-group-sm" style={{ width: 260 }}>
              <span className="input-group-text"><i className="bi bi-search"></i></span>
              <input type="text" className="form-control" placeholder="Buscar por nome ou SKU…"
                value={search} onChange={e => { setSearch(e.target.value); changePage(1) }} />
              {search && (
                <button className="btn btn-outline-secondary" onClick={() => setSearch('')}>
                  <i className="bi bi-x"></i>
                </button>
              )}
            </div>
            <button className="btn btn-primary btn-sm" style={{ fontSize: 12, whiteSpace: 'nowrap' }} onClick={abrirNovo}>
              <i className="bi bi-plus-lg me-1"></i>Novo produto
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
                    <th>SKU</th><th>Nome</th><th>Descrição</th><th>Unidade</th>
                    <th className="text-end">Preço Unit.</th><th className="text-end">Est. Mínimo</th>
                    <th>Status</th><th className="text-end">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {paged.map(p => (
                    <tr key={p.id}>
                      <td className="font-monospace text-muted" style={{ fontSize: 12 }}>{p.sku}</td>
                      <td className="fw-medium">{p.nome}</td>
                      <td className="text-muted" style={{ maxWidth: 180, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {p.descricao ?? '—'}
                      </td>
                      <td>{p.unidadeMedida}</td>
                      <td className="text-end">{brl(p.precoUnitario)}</td>
                      <td className="text-end">{p.estoqueMinimo}</td>
                      <td><StatusBadge ativo={p.ativo} /></td>
                      <td className="text-end text-nowrap">
                        <button className="btn btn-sm btn-outline-secondary me-1" style={{ fontSize: 11 }}
                          onClick={() => abrirEditar(p)}>
                          <i className="bi bi-pencil me-1"></i>Editar
                        </button>
                        {p.ativo ? (
                          <button className="btn btn-sm btn-outline-danger" style={{ fontSize: 11 }}
                            disabled={inativar.isPending} onClick={() => inativar.mutate(p.id)}>
                            Inativar
                          </button>
                        ) : (
                          <button className="btn btn-sm btn-outline-success" style={{ fontSize: 11 }}
                            disabled={reativar.isPending} onClick={() => reativar.mutate(p.id)}>
                            Reativar
                          </button>
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

      {/* Modal Novo / Editar */}
      {modal && (
        <div className="modal d-block" style={{ background: 'rgba(0,0,0,.4)' }} onClick={e => e.target === e.currentTarget && fecharModal()}>
          <div className="modal-dialog modal-dialog-centered">
            <div className="modal-content">
              <div className="modal-header py-2 px-3">
                <h6 className="modal-title mb-0" style={{ fontSize: 14 }}>
                  {modal === 'novo' ? 'Novo produto' : `Editar: ${editando?.nome}`}
                </h6>
                <button type="button" className="btn-close btn-sm" onClick={fecharModal}></button>
              </div>
              <form onSubmit={handleSubmit}>
                <div className="modal-body px-3 py-2" style={{ fontSize: 13 }}>
                  {erro && <div className="alert alert-danger py-1 px-2 mb-2" style={{ fontSize: 12 }}>{erro}</div>}
                  {modal === 'novo' && (
                    <div className="mb-2">
                      <label className="form-label mb-1">SKU *</label>
                      <input name="sku" value={form.sku} onChange={handleChange} required
                        className="form-control form-control-sm" placeholder="Ex: PROD-001" />
                    </div>
                  )}
                  <div className="mb-2">
                    <label className="form-label mb-1">Nome *</label>
                    <input name="nome" value={form.nome} onChange={handleChange} required className="form-control form-control-sm" />
                  </div>
                  <div className="mb-2">
                    <label className="form-label mb-1">Descrição</label>
                    <textarea name="descricao" value={form.descricao} onChange={handleChange} className="form-control form-control-sm" rows={2} />
                  </div>
                  <div className="row g-2 mb-2">
                    <div className="col-6">
                      <label className="form-label mb-1">Preço unitário *</label>
                      <input name="precoUnitario" type="number" min="0" step="0.01" value={form.precoUnitario}
                        onChange={handleChange} required className="form-control form-control-sm" />
                    </div>
                    <div className="col-6">
                      <label className="form-label mb-1">Estoque mínimo</label>
                      <input name="estoqueMinimo" type="number" min="0" step="1" value={form.estoqueMinimo}
                        onChange={handleChange} className="form-control form-control-sm" />
                    </div>
                  </div>
                  <div className="mb-2">
                    <label className="form-label mb-1">Unidade de medida *</label>
                    <select name="unidadeMedidaId" value={form.unidadeMedidaId} onChange={handleChange}
                      required className="form-select form-select-sm">
                      <option value="">Selecione...</option>
                      {unidades.map(u => <option key={u.id} value={u.id}>{u.sigla} — {u.descricao}</option>)}
                    </select>
                  </div>
                </div>
                <div className="modal-footer py-2 px-3">
                  <button type="button" className="btn btn-sm btn-outline-secondary" onClick={fecharModal}>Cancelar</button>
                  <button type="submit" className="btn btn-sm btn-primary" disabled={isPending}>
                    {isPending ? 'Salvando...' : modal === 'novo' ? 'Criar produto' : 'Salvar alterações'}
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
