import { useState, useMemo } from 'react'

export const PAGE_SIZES = [10, 25, 50] as const
export type PageSize = typeof PAGE_SIZES[number]

export function usePagination<T>(items: T[], defaultPageSize: PageSize = 10) {
  const [page, setPage]         = useState(1)
  const [pageSize, setPageSize] = useState<PageSize>(defaultPageSize)

  const totalPages = Math.max(1, Math.ceil(items.length / pageSize))

  // Reset to page 1 when items or pageSize change
  const safePage = Math.min(page, totalPages)

  const paged = useMemo(
    () => items.slice((safePage - 1) * pageSize, safePage * pageSize),
    [items, safePage, pageSize],
  )

  function changePage(p: number) {
    setPage(Math.max(1, Math.min(p, totalPages)))
  }

  function changePageSize(ps: PageSize) {
    setPageSize(ps)
    setPage(1)
  }

  return { paged, page: safePage, pageSize, totalPages, changePage, changePageSize, total: items.length }
}
