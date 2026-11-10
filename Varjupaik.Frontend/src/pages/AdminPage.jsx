import { useEffect, useState } from 'react'
import { Navigate } from 'react-router-dom'
import { api, photoUrl } from '../api'
import { useAuth } from '../AuthContext'
import { ageText, speciesEmoji, statusNames } from '../helpers'
import AnimalForm from '../components/AnimalForm'
import Pagination from '../components/Pagination'

function AdminPage() {
  const { isStaff } = useAuth()
  const [query, setQuery] = useState({ search: '', sortBy: 'newest', page: 1, pageSize: 8 })
  const [result, setResult] = useState(null)
  const [editing, setEditing] = useState(null)
  const [message, setMessage] = useState(null)
  const [reload, setReload] = useState(0)

  useEffect(() => {
    if (!isStaff) return
    const timer = setTimeout(() => {
      api.getAnimals(query)
        .then(setResult)
        .catch((err) => setMessage({ type: 'error', text: err.message }))
    }, 250)
    return () => clearTimeout(timer)
  }, [query, reload, isStaff])

  // Ainult Admin ja Staff
  if (!isStaff) {
    return <Navigate to="/login" />
  }

  function showMessage(type, text) {
    setMessage({ type, text })
    setTimeout(() => setMessage(null), 3500)
  }

  async function handleSave(formData) {
    try {
      if (editing) {
        await api.updateAnimal(editing.id, formData)
        showMessage('success', 'Muudatused salvestatud ✔')
        setEditing(null)
      } else {
        await api.createAnimal(formData)
        showMessage('success', 'Uus loom lisatud 🐾')
      }
      setReload(reload + 1)
    } catch (err) {
      showMessage('error', err.message)
    }
  }

  async function handleDelete(animal) {
    if (!window.confirm('Kas kustutada ' + animal.name + '?')) return
    try {
      await api.deleteAnimal(animal.id)
      showMessage('success', animal.name + ' kustutatud')
      if (editing && editing.id === animal.id) setEditing(null)
      setReload(reload + 1)
    } catch (err) {
      showMessage('error', err.message)
    }
  }

  return (
    <div className="container section">
      <div className="section-head">
        <h2>Loomade haldus</h2>
        {result && <p className="muted">Kokku {result.totalCount} looma</p>}
      </div>

      {message && <p className={'alert toast alert-' + message.type}>{message.text}</p>}

      <div className="admin-layout">
        <AnimalForm key={editing ? editing.id : 'new'} animal={editing} onSave={handleSave} onCancel={() => setEditing(null)} />

        <div className="admin-list">
          <div className="filter-row">
            <label className="search-box">
              <span aria-hidden="true">🔍</span>
              <input
                type="search"
                placeholder="Otsi looma..."
                value={query.search}
                onChange={(e) => setQuery({ ...query, search: e.target.value, page: 1 })}
              />
            </label>
            <select value={query.sortBy} onChange={(e) => setQuery({ ...query, sortBy: e.target.value, page: 1 })}>
              <option value="newest">Uuemad enne</option>
              <option value="oldest">Vanemad enne</option>
              <option value="name">Nimi A–Z</option>
              <option value="age">Vanus</option>
            </select>
          </div>

          {result && result.items.map((animal) => (
            <div key={animal.id} className={'admin-row' + (editing && editing.id === animal.id ? ' selected' : '')}>
              <div className={'admin-thumb species-' + animal.species.toLowerCase()}>
                {animal.photoUrl ? <img src={photoUrl(animal.photoUrl)} alt="" /> : speciesEmoji(animal.species)}
              </div>
              <div className="admin-info">
                <strong>{animal.name}</strong>
                <span className="muted">{animal.species} · {ageText(animal.age)}</span>
              </div>
              <span className={'status-pill status-' + animal.status}>{statusNames[animal.status]}</span>
              <div className="admin-actions">
                <button className="btn btn-ghost btn-small" onClick={() => setEditing(animal)}>Muuda</button>
                <button className="btn btn-danger btn-small" onClick={() => handleDelete(animal)}>Kustuta</button>
              </div>
            </div>
          ))}

          {result && result.items.length === 0 && <p className="muted center">Loomi ei leitud.</p>}

          {result && (
            <Pagination page={result.page} totalPages={result.totalPages}
              onChange={(page) => setQuery({ ...query, page })} />
          )}
        </div>
      </div>
    </div>
  )
}

export default AdminPage
