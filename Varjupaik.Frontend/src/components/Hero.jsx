import { useEffect, useState } from 'react'
import { api } from '../api'

// Avalehe ülemine osa koos statistikaga
function Hero() {
  const [stats, setStats] = useState(null)

  useEffect(() => {
    // Küsime ainult totalCount väärtusi (pageSize=1)
    Promise.all([
      api.getAnimals({ pageSize: 1 }),
      api.getAnimals({ status: 'Available', pageSize: 1 }),
      api.getAnimals({ status: 'Adopted', pageSize: 1 }),
    ])
      .then(([all, available, adopted]) =>
        setStats({ all: all.totalCount, available: available.totalCount, adopted: adopted.totalCount }))
      .catch(() => setStats(null))
  }, [])

  return (
    <section className="hero">
      <div className="container hero-inner">
        <div className="hero-text">
          <span className="hero-badge">🏡 Käpakodu varjupaik</span>
          <h1>
            Iga käpp väärib <span className="highlight">armastavat kodu</span>
          </h1>
          <p>
            Meie juures ootavad koerad, kassid, küülikud ja linnud oma uut peret.
            Sirvi loomi, leia oma lemmik ja esita adopteerimise taotlus vaid mõne klikiga.
          </p>
          <div className="hero-actions">
            <a href="#loomad" className="btn btn-accent">Vaata loomi 🐾</a>
            <a href="#kuidas" className="btn btn-ghost">Kuidas adopteerida?</a>
          </div>

          {stats && (
            <div className="stats">
              <div><strong>{stats.all}</strong><span>looma varjupaigas</span></div>
              <div><strong>{stats.available}</strong><span>otsivad kodu</span></div>
              <div><strong>{stats.adopted}</strong><span>leidsid kodu</span></div>
            </div>
          )}
        </div>

        <div className="hero-art" aria-hidden="true">
          <div className="blob" />
          <span className="bubble b1">🐶</span>
          <span className="bubble b2">🐱</span>
          <span className="bubble b3">🐰</span>
          <span className="bubble b4">🦜</span>
          <span className="paw p1">🐾</span>
          <span className="paw p2">🐾</span>
          <div className="hero-heart">❤️</div>
        </div>
      </div>
    </section>
  )
}

export default Hero
