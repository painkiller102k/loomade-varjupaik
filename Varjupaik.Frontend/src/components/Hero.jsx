import { useEffect, useState } from 'react'
import { api } from '../api'
import Dachshund from './Dachshund'
import Logo from './Logo'

// Avalehe ülemine osa: tekst, statistika ja hubane stseen takskoeraga
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
    <section className="home-hero">
      <div className="container hero-grid">
        <div className="hero-text">
          <span className="hero-pill"><Logo /> Loomade varjupaik</span>
          <h1>
            Iga käpp väärib <span className="highlight">armastavat kodu</span>
          </h1>
          <p>
            Meie juures ootavad koerad, kassid, küülikud ja linnud oma uut peret.
            Sirvi loomi, leia oma lemmik ja esita adopteerimise taotlus vaid mõne klikiga.
          </p>
          <div className="hero-actions">
            <a href="#loomad" className="btn btn-accent">Vaata loomi</a>
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

        <div className="hero-scene">
          <svg viewBox="0 0 420 500" role="img" aria-label="Takskoer piilub künka tagant ja vehib käpaga">
            <defs>
              <clipPath id="arch">
                <path d="M0 500 V210 A210 210 0 0 1 420 210 V500 Z" />
              </clipPath>
              <linearGradient id="sky" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0" stopColor="#ffe0b0" />
                <stop offset="1" stopColor="#fff4e2" />
              </linearGradient>
            </defs>

            <g clipPath="url(#arch)">
              <rect width="420" height="500" fill="url(#sky)" />
              <circle cx="300" cy="150" r="98" fill="#ffc978" opacity="0.28" />
              <circle cx="300" cy="150" r="62" fill="#ffc978" opacity="0.8" />
              <path d="M58 132 h72 a19 19 0 0 0 0-38 a27 27 0 0 0-50-6 a21 21 0 0 0-22 44z" fill="#fff" opacity="0.85" />
              <path d="M240 78 h46 a13 13 0 0 0 0-26 a18 18 0 0 0-33-4 a14 14 0 0 0-13 30z" fill="#fff" opacity="0.7" />
              <path d="M0 362 C90 306 190 346 270 324 C340 304 380 322 420 312 V500 H0Z" fill="#a9d6bd" />
              <Dachshund x="58" y="206" width="310" height="245" />
              <path d="M0 432 C100 382 210 424 300 398 C350 384 390 398 420 392 V500 H0Z" fill="#1f5f4a" />
              <g fill="#2f8a68" opacity="0.9">
                <circle cx="70" cy="462" r="7" />
                <circle cx="96" cy="452" r="5" />
                <circle cx="336" cy="450" r="7" />
                <circle cx="360" cy="466" r="5" />
              </g>
            </g>
            <path d="M5 500 V210 A205 205 0 0 1 415 210 V500" fill="none" stroke="#fff" strokeWidth="10" />
          </svg>
        </div>
      </div>

      <svg className="hero-wave" viewBox="0 0 1440 80" preserveAspectRatio="none" aria-hidden="true">
        <path d="M0 42 C240 92 480 2 720 44 C960 86 1200 10 1440 52 V80 H0Z" fill="#ffffff" />
      </svg>
    </section>
  )
}

export default Hero
