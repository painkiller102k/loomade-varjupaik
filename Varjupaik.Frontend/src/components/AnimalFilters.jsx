import { speciesList } from '../helpers'

function AnimalFilters({ filters, onChange }) {
  function set(name, value) {
    onChange({ ...filters, [name]: value, page: 1 })
  }

  return (
    <div className="filters">
      <div className="species-chips">
        <button
          className={'chip' + (filters.species === '' ? ' active' : '')}
          onClick={() => set('species', '')}
        >
          🐾 Kõik
        </button>
        {speciesList.map((s) => (
          <button
            key={s.name}
            className={'chip' + (filters.species === s.name ? ' active' : '')}
            onClick={() => set('species', s.name)}
          >
            {s.emoji} {s.name}
          </button>
        ))}
      </div>

      <div className="filter-row">
        <label className="search-box">
          <span aria-hidden="true">🔍</span>
          <input
            type="search"
            placeholder="Otsi nime või kirjelduse järgi..."
            value={filters.search}
            onChange={(e) => set('search', e.target.value)}
          />
        </label>

        <select value={filters.status} onChange={(e) => set('status', e.target.value)} aria-label="Olek">
          <option value="">Kõik olekud</option>
          <option value="Available">Otsib kodu</option>
          <option value="Reserved">Broneeritud</option>
          <option value="Adopted">Leidis kodu</option>
        </select>

        <select value={filters.sortBy} onChange={(e) => set('sortBy', e.target.value)} aria-label="Sorteerimine">
          <option value="newest">Uuemad enne</option>
          <option value="oldest">Kauem varjupaigas</option>
          <option value="name">Nimi A–Z</option>
          <option value="age">Noorimad enne</option>
        </select>
      </div>
    </div>
  )
}

export default AnimalFilters
