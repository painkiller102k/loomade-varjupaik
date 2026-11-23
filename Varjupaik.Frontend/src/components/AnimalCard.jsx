import { Link } from 'react-router-dom'
import { photoUrl } from '../api'
import { ageText, statusNames } from '../helpers'
import Paw from './Paw'

// Looma kaart: pilt ja nimi viivad looma lehele, nupp avab taotluse vormi
function AnimalCard({ animal, onApply }) {
  const photo = photoUrl(animal.photoUrl)
  const link = '/loomad/' + animal.id

  return (
    <article className="animal-card">
      <Link to={link} className="animal-photo-link" aria-label={animal.name}>
        <div className={'animal-photo species-' + animal.species.toLowerCase()}>
          {photo ? <img src={photo} alt={animal.name} loading="lazy" /> : <Paw className="paw-art" />}
          <span className={'status-badge status-' + animal.status}>{statusNames[animal.status]}</span>
        </div>
      </Link>

      <div className="animal-body">
        <div className="animal-title">
          <h3><Link to={link}>{animal.name}</Link></h3>
          <span className="age-chip">{ageText(animal.age)}</span>
        </div>
        <p className="animal-meta">{animal.species}</p>
        <p className="animal-description">{animal.description}</p>

        <div className="animal-actions">
          <Link to={link} className="more-link">Loe lähemalt →</Link>
          {onApply && (
            animal.status === 'Adopted' ? (
              <span className="found-home">Leidis kodu</span>
            ) : (
              <button className="btn btn-accent btn-small" onClick={() => onApply(animal)}>
                Taha adopteerida
              </button>
            )
          )}
        </div>
      </div>
    </article>
  )
}

export default AnimalCard
