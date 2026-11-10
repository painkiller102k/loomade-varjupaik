import { useEffect, useState } from 'react'
import { api } from '../api'
import AnimalCard from '../components/AnimalCard'
import AnimalFilters from '../components/AnimalFilters'
import ApplyModal from '../components/ApplyModal'
import Pagination from '../components/Pagination'

function HomePage() {
  const [filters, setFilters] = useState({
    species: '',
    status: '',
    search: '',
    sortBy: 'newest',
    page: 1,
    pageSize: 9,
  })
  const [result, setResult] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [selected, setSelected] = useState(null)

  // Laeme loomad iga kord, kui filtrid muutuvad (otsingule väike viivitus)
  useEffect(() => {
    const timer = setTimeout(() => {
      setLoading(true)
      api.getAnimals(filters)
        .then((data) => {
          setResult(data)
          setError('')
        })
        .catch((err) => setError(err.message))
        .finally(() => setLoading(false))
    }, 300)
    return () => clearTimeout(timer)
  }, [filters])

  function changePage(page) {
    setFilters({ ...filters, page })
    document.getElementById('loomad').scrollIntoView({ behavior: 'smooth' })
  }

  return (
    <section id="loomad" className="container section">
      <div className="section-head">
        <h2>Meie hoolealused</h2>
        {result && <p className="muted">Leitud {result.totalCount} looma</p>}
      </div>

      <AnimalFilters filters={filters} onChange={setFilters} />

      {error && <p className="alert alert-error">{error}</p>}

      {loading && !result ? (
        <p className="muted center">Laen loomi...</p>
      ) : result && result.items.length === 0 ? (
        <div className="empty">
          <span>🐾</span>
          <p>Selliste filtritega loomi ei leitud.</p>
        </div>
      ) : (
        <div className={'animal-grid' + (loading ? ' loading' : '')}>
          {result && result.items.map((animal) => (
            <AnimalCard key={animal.id} animal={animal} onApply={setSelected} />
          ))}
        </div>
      )}

      {result && (
        <Pagination page={result.page} totalPages={result.totalPages} onChange={changePage} />
      )}

      {selected && <ApplyModal animal={selected} onClose={() => setSelected(null)} />}
    </section>
  )
}

export default HomePage
