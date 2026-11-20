import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api'
import { dateOnly, speciesEmoji } from '../helpers'

function daysText(daysLeft) {
  if (daysLeft < 0) return 'hilinenud ' + -daysLeft + ' p'
  if (daysLeft === 0) return 'täna'
  if (daysLeft === 1) return 'homme'
  return daysLeft + ' päeva pärast'
}

// Töötaja lehe plokk: lähenevad vaktsineerimised esile tõstetud
function UpcomingVaccinations() {
  const [days, setDays] = useState(7)
  const [items, setItems] = useState(null)

  useEffect(() => {
    let ignore = false
    api.getUpcomingVaccinations(days)
      .then((data) => {
        if (!ignore) setItems(data)
      })
      .catch(() => {
        if (!ignore) setItems([])
      })
    return () => {
      ignore = true
    }
  }, [days])

  return (
    <section className="vaccine-box">
      <div className="vaccine-head">
        <h3>💉 Lähenevad vaktsineerimised</h3>
        <select value={days} onChange={(e) => setDays(Number(e.target.value))} aria-label="Päevi ette">
          <option value="7">7 päeva</option>
          <option value="14">14 päeva</option>
          <option value="30">30 päeva</option>
        </select>
      </div>

      {items && items.length === 0 && (
        <p className="muted">Järgmise {days} päeva jooksul vaktsineerimisi pole.</p>
      )}

      <ul className="vaccine-list">
        {items && items.map((v) => (
          <li key={v.id}>
            <Link to={'/haldus/' + v.animalId + '/visiidid'}>
              {speciesEmoji(v.animalSpecies)} <strong>{v.animalName}</strong>
            </Link>
            <span>{v.name}</span>
            <span className="muted">{dateOnly(v.nextDueDate)}</span>
            <span className={'due-pill ' + (v.daysLeft < 0 ? 'due-late' : v.daysLeft <= 3 ? 'due-soon' : 'due-ok')}>
              {daysText(v.daysLeft)}
            </span>
          </li>
        ))}
      </ul>
    </section>
  )
}

export default UpcomingVaccinations
