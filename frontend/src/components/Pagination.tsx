import { PAGE_SIZES, type PageSize } from '../hooks/usePagination'

interface Props {
  page: number
  totalPages: number
  pageSize: PageSize
  total: number
  onPage: (p: number) => void
  onPageSize: (ps: PageSize) => void
}

export function Pagination({ page, totalPages, pageSize, total, onPage, onPageSize }: Props) {
  if (total === 0) return null

  const start = (page - 1) * pageSize + 1
  const end   = Math.min(page * pageSize, total)

  return (
    <div className="d-flex align-items-center justify-content-between px-3 py-2 border-top"
      style={{ fontSize: 12, color: '#6c757d' }}>

      {/* Itens por página */}
      <div className="d-flex align-items-center gap-2">
        <span>Itens por página:</span>
        <select
          value={pageSize}
          onChange={e => onPageSize(Number(e.target.value) as PageSize)}
          className="form-select form-select-sm"
          style={{ width: 70 }}
        >
          {PAGE_SIZES.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
      </div>

      {/* Info + controles */}
      <div className="d-flex align-items-center gap-3">
        <span>{start}–{end} de {total}</span>
        <div className="btn-group btn-group-sm">
          <button className="btn btn-outline-secondary" disabled={page <= 1} onClick={() => onPage(1)} style={{ fontSize: 11 }}>«</button>
          <button className="btn btn-outline-secondary" disabled={page <= 1} onClick={() => onPage(page - 1)} style={{ fontSize: 11 }}>‹</button>
          <button className="btn btn-outline-secondary" disabled style={{ fontSize: 11, minWidth: 60 }}>
            {page} / {totalPages}
          </button>
          <button className="btn btn-outline-secondary" disabled={page >= totalPages} onClick={() => onPage(page + 1)} style={{ fontSize: 11 }}>›</button>
          <button className="btn btn-outline-secondary" disabled={page >= totalPages} onClick={() => onPage(totalPages)} style={{ fontSize: 11 }}>»</button>
        </div>
      </div>
    </div>
  )
}
