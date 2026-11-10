import { useEffect, useState } from 'react'
import { photoUrl } from '../api'
import { speciesList } from '../helpers'

const empty = { name: '', species: '', age: 0, description: '', status: 'Available' }

// Vorm looma lisamiseks ja muutmiseks
function AnimalForm({ animal, onSave, onCancel }) {
  const [form, setForm] = useState(empty)
  const [photo, setPhoto] = useState(null)
  const [preview, setPreview] = useState(null)
  const [errors, setErrors] = useState({})
  const [sending, setSending] = useState(false)

  // Kui valitakse muudetav loom, täidame vormi
  useEffect(() => {
    if (animal) {
      setForm({
        name: animal.name,
        species: animal.species,
        age: animal.age,
        description: animal.description,
        status: animal.status,
      })
      setPreview(photoUrl(animal.photoUrl))
    } else {
      setForm(empty)
      setPreview(null)
    }
    setPhoto(null)
    setErrors({})
  }, [animal])

  function update(name, value) {
    setForm({ ...form, [name]: value })
  }

  function choosePhoto(file) {
    setPhoto(file || null)
    setPreview(file ? URL.createObjectURL(file) : animal ? photoUrl(animal.photoUrl) : null)
  }

  function validate() {
    const e = {}
    if (!form.name.trim()) e.name = 'Nimi on kohustuslik'
    else if (form.name.length > 50) e.name = 'Nimi on liiga pikk'
    if (!form.species.trim()) e.species = 'Liik on kohustuslik'
    const age = Number(form.age)
    if (form.age === '' || isNaN(age) || age < 0 || age > 50) e.age = 'Vanus peab olema 0–50'
    if (form.description.length > 1000) e.description = 'Kirjeldus on liiga pikk (max 1000)'
    if (photo && !photo.type.startsWith('image/')) e.photo = 'Vali pildifail'
    if (photo && photo.size > 5 * 1024 * 1024) e.photo = 'Pilt on liiga suur (max 5 MB)'
    return e
  }

  async function handleSubmit(event) {
    event.preventDefault()
    const e = validate()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    const data = new FormData()
    data.append('name', form.name.trim())
    data.append('species', form.species.trim())
    data.append('age', form.age)
    data.append('description', form.description.trim())
    data.append('status', form.status)
    if (photo) data.append('photo', photo)

    setSending(true)
    try {
      await onSave(data)
      if (!animal) {
        setForm(empty)
        choosePhoto(null)
      }
    } finally {
      setSending(false)
    }
  }

  return (
    <form className="admin-form" onSubmit={handleSubmit} noValidate>
      <h2>{animal ? 'Muuda: ' + animal.name : 'Lisa uus loom'}</h2>

      <label className="photo-picker">
        {preview ? <img src={preview} alt="" /> : <span>📷<br />Vali pilt</span>}
        <input type="file" accept="image/*" onChange={(e) => choosePhoto(e.target.files[0])} />
      </label>
      {errors.photo && <span className="field-error">{errors.photo}</span>}

      <div className="field">
        <label htmlFor="a-name">Nimi</label>
        <input id="a-name" value={form.name} onChange={(e) => update('name', e.target.value)}
          className={errors.name ? 'invalid' : ''} />
        {errors.name && <span className="field-error">{errors.name}</span>}
      </div>

      <div className="field-row">
        <div className="field">
          <label htmlFor="a-species">Liik</label>
          <input id="a-species" list="species-list" value={form.species}
            onChange={(e) => update('species', e.target.value)}
            className={errors.species ? 'invalid' : ''} />
          <datalist id="species-list">
            {speciesList.map((s) => <option key={s.name} value={s.name} />)}
          </datalist>
          {errors.species && <span className="field-error">{errors.species}</span>}
        </div>

        <div className="field">
          <label htmlFor="a-age">Vanus (a)</label>
          <input id="a-age" type="number" min="0" max="50" value={form.age}
            onChange={(e) => update('age', e.target.value)}
            className={errors.age ? 'invalid' : ''} />
          {errors.age && <span className="field-error">{errors.age}</span>}
        </div>
      </div>

      <div className="field">
        <label htmlFor="a-status">Olek</label>
        <select id="a-status" value={form.status} onChange={(e) => update('status', e.target.value)}>
          <option value="Available">Otsib kodu</option>
          <option value="Reserved">Broneeritud</option>
          <option value="Adopted">Leidis kodu</option>
        </select>
      </div>

      <div className="field">
        <label htmlFor="a-desc">Kirjeldus</label>
        <textarea id="a-desc" rows="4" value={form.description}
          onChange={(e) => update('description', e.target.value)}
          className={errors.description ? 'invalid' : ''} />
        {errors.description && <span className="field-error">{errors.description}</span>}
      </div>

      <div className="modal-actions">
        <button className="btn btn-primary" disabled={sending}>
          {sending ? 'Salvestan...' : animal ? 'Salvesta muudatused' : 'Lisa loom'}
        </button>
        {animal && <button type="button" className="btn btn-ghost" onClick={onCancel}>Tühista</button>}
      </div>
    </form>
  )
}

export default AnimalForm
