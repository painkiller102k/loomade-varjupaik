import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import AnimalCard from '../components/AnimalCard'
import AnimalFilters from '../components/AnimalFilters'
import ApplyModal from '../components/ApplyModal'
import Pagination from '../components/Pagination'
import Hero from '../components/Hero'
import HowTo from '../components/HowTo'
import AverageStay from '../components/AverageStay'
import Paw from '../components/Paw'

function HomePage() {
  const { user } = useAuth()
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
    <>
    <Hero />
    <HowTo />
    <AverageStay />
    <section id="loomad" className="container section">
      <div className="section-head">
        <div>
          <span className="eyebrow">Otsivad kodu</span>
          <h2>Meie hoolealused</h2>
        </div>
        {result && <p className="muted">Leitud {result.totalCount} looma</p>}
      </div>

      <AnimalFilters filters={filters} onChange={setFilters} />

      {error && <p className="alert alert-error">{error}</p>}

      {loading && !result ? (
        <p className="muted center">Laen loomi...</p>
      ) : result && result.items.length === 0 ? (
        <div className="empty">
          <Paw className="empty-paw" />
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

    <section className="container cta-wrap">
      <div className="cta-band">
        <div>
          <h2>Leia endale uus parim sõber</h2>
          <p>Iga loom siin ootab kedagi, kes ta koju viib. Tee esimene samm juba täna.</p>
        </div>
        <div className="cta-actions">
          <a href="#loomad" className="btn btn-accent">Vaata loomi</a>
          {!user && <Link to="/register" className="btn btn-light">Registreeru</Link>}
        </div>
      </div>
    </section>
    </>
  )
}

export default HomePage
