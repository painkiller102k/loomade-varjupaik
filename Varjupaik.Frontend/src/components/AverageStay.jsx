import { useEffect, useState } from 'react'
import { api } from '../api'
import { speciesEmoji } from '../helpers'

// Avalehe plokk: keskmine aeg varjupaigas liigi kaupa
function AverageStay() {
  const [items, setItems] = useState(null)

  useEffect(() => {
    api.getAverageStay()
      .then(setItems)
      .catch(() => setItems([]))
  }, [])

  if (!items || items.length === 0) return null

  const max = Math.max(...items.map((i) => i.averageDays), 1)

  return (
    <section className="container section stay-section">
      <div className="section-head">
        <h2>Keskmine aeg varjupaigas</h2>
        <p className="muted">liikide kaupa, päevades</p>
      </div>

      <div className="stay-grid">
        {items.map((item) => (
          <div key={item.species} className={'stay-card' + (item.averageDays === max ? ' longest' : '')}>
            <span className="stay-species">{speciesEmoji(item.species)} {item.species}</span>
            <strong className="stay-days">{Math.round(item.averageDays)}</strong>
            <span className="muted">
              päeva · {item.animalCount === 1 ? '1 loom' : item.animalCount + ' looma'}
            </span>
            <div className="stay-bar">
              <div style={{ width: Math.round((item.averageDays / max) * 100) + '%' }} />
            </div>
          </div>
        ))}
      </div>
    </section>
  )
}

export default AverageStay
