function Pagination({ page, totalPages, onChange }) {
  if (totalPages <= 1) return null

  const pages = []
  for (let i = 1; i <= totalPages; i++) {
    pages.push(i)
  }

  return (
    <nav className="pagination" aria-label="Leheküljed">
      <button className="page-btn" disabled={page === 1} onClick={() => onChange(page - 1)}>
        ‹ Eelmine
      </button>
      {pages.map((p) => (
        <button
          key={p}
          className={'page-btn' + (p === page ? ' active' : '')}
          onClick={() => onChange(p)}
        >
          {p}
        </button>
      ))}
      <button className="page-btn" disabled={page === totalPages} onClick={() => onChange(page + 1)}>
        Järgmine ›
      </button>
    </nav>
  )
}

export default Pagination
