import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { api, photoUrl } from '../api'
import { ageText, dateOnly, speciesColor, statusNames } from '../helpers'
import AnimalCard from '../components/AnimalCard'
import ApplyModal from '../components/ApplyModal'
import Paw from '../components/Paw'

// Ühe looma leht: suur pilt, andmed, taotluse nupp ja sarnased loomad
function AnimalPage() {
  const { id } = useParams()
  const [animal, setAnimal] = useState(null)
  const [days, setDays] = useState(0)
  const [similar, setSimilar] = useState([])
  const [error, setError] = useState('')
  const [applying, setApplying] = useState(false)

  useEffect(() => {
    let ignore = false
    api.getAnimal(id)
      .then((data) => {
        if (ignore) return
        setAnimal(data)
        setError('')
        setDays(Math.max(0, Math.floor((Date.now() - new Date(data.createdAt)) / 86400000)))
        return api.getAnimals({ species: data.species, pageSize: 4 })
      })
      .then((data) => {
        if (!ignore && data) setSimilar(data.items.filter((a) => String(a.id) !== String(id)).slice(0, 3))
      })
      .catch((err) => {
        if (!ignore) setError(err.message)
      })
    return () => {
      ignore = true
    }
  }, [id])

  if (error) {
    return (
      <div className="container section">
        <div className="empty">
          <Paw className="empty-paw" />
          <p>{error}</p>
          <Link to="/" className="btn btn-primary">Tagasi loomade juurde</Link>
        </div>
      </div>
    )
  }

  if (!animal) {
    return <p className="container section muted center">Laen looma andmeid...</p>
  }

  const photo = photoUrl(animal.photoUrl)

  return (
    <div className="container section animal-page">
      <Link to="/" className="back-link">← Kõik loomad</Link>

      <div className="animal-detail">
        <div className={'detail-photo species-' + animal.species.toLowerCase()}>
          {photo ? <img src={photo} alt={animal.name} /> : <Paw className="detail-paw" />}
          <span className={'status-badge status-' + animal.status}>{statusNames[animal.status]}</span>
        </div>

        <div className="detail-info">
          <span className="eyebrow">
            <i className="stay-dot" style={{ background: speciesColor(animal.species) }} />
            {animal.species}
          </span>
          <h1>{animal.name}</h1>

          <div className="fact-row">
            <div className="fact"><span>Vanus</span><strong>{ageText(animal.age)}</strong></div>
            <div className="fact"><span>Olek</span><strong>{statusNames[animal.status]}</strong></div>
            <div className="fact"><span>Varjupaigas alates</span><strong>{dateOnly(animal.createdAt)}</strong></div>
          </div>

          <h3>Looma lugu</h3>
          <p className="detail-description">{animal.description || 'Kirjeldust pole lisatud.'}</p>

          {animal.status !== 'Adopted' && (
            <p className="muted">Ootab uut kodu juba {days} {days === 1 ? 'päev' : 'päeva'}.</p>
          )}

          <div className="detail-actions">
            {animal.status === 'Adopted' ? (
              <span className="found-home big">Leidis kodu</span>
            ) : (
              <button className="btn btn-accent" onClick={() => setApplying(true)}>Taha adopteerida</button>
            )}
            <Link to="/" className="btn btn-ghost">Vaata teisi loomi</Link>
          </div>
        </div>
      </div>

      {similar.length > 0 && (
        <div className="similar">
          <div className="section-head">
            <div>
              <span className="eyebrow">Sinu jaoks</span>
              <h2>Sarnased loomad</h2>
            </div>
          </div>
          <div className="animal-grid">
            {similar.map((a) => <AnimalCard key={a.id} animal={a} />)}
          </div>
        </div>
      )}

      {applying && <ApplyModal animal={animal} onClose={() => setApplying(false)} />}
    </div>
  )
}

export default AnimalPage
