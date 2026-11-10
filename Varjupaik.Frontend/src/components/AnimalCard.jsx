import { photoUrl } from '../api'
import { ageText, speciesEmoji, statusNames } from '../helpers'

function AnimalCard({ animal, onApply }) {
  const photo = photoUrl(animal.photoUrl)

  return (
    <article className="animal-card">
      <div className={'animal-photo species-' + animal.species.toLowerCase()}>
        {photo ? (
          <img src={photo} alt={animal.name} loading="lazy" />
        ) : (
          <span className="animal-emoji" aria-hidden="true">{speciesEmoji(animal.species)}</span>
        )}
        <span className={'status-badge status-' + animal.status}>{statusNames[animal.status]}</span>
      </div>

      <div className="animal-body">
        <h3>{animal.name}</h3>
        <p className="animal-meta">
          {speciesEmoji(animal.species)} {animal.species} · {ageText(animal.age)}
        </p>
        <p className="animal-description">{animal.description}</p>

        {onApply && (
          animal.status === 'Adopted' ? (
            <button className="btn btn-ghost" disabled>Leidis kodu ❤️</button>
          ) : (
            <button className="btn btn-accent" onClick={() => onApply(animal)}>
              Taha adopteerida
            </button>
          )
        )}
      </div>
    </article>
  )
}

export default AnimalCard
