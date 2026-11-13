import { useEffect, useState } from 'react'
import { Link, Navigate, useParams } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import { ageText, dateOnly, speciesEmoji } from '../helpers'

const today = new Date().toISOString().slice(0, 10)
const emptyForm = { visitDate: today, reason: '', vet: '', notes: '' }

// Töötaja leht: looma veterinaarivisiitide ajalugu ja uue visiidi lisamine
function VetVisitsPage() {
  const { id } = useParams()
  const { isStaff } = useAuth()
  const [animal, setAnimal] = useState(null)
  const [visits, setVisits] = useState(null)
  const [form, setForm] = useState(emptyForm)
  const [errors, setErrors] = useState({})
  const [message, setMessage] = useState(null)
  const [sending, setSending] = useState(false)
  const [reload, setReload] = useState(0)

  useEffect(() => {
    if (!isStaff) return
    let ignore = false
    Promise.all([api.getAnimal(id), api.getVetVisits(id)])
      .then(([a, v]) => {
        if (!ignore) {
          setAnimal(a)
          setVisits(v)
        }
      })
      .catch((err) => {
        if (!ignore) setMessage({ type: 'error', text: err.message })
      })
    return () => {
      ignore = true
    }
  }, [id, reload, isStaff])

  if (!isStaff) {
    return <Navigate to="/login" />
  }

  function validate() {
    const e = {}
    if (!form.visitDate) e.visitDate = 'Vali kuupäev'
    if (!form.reason.trim()) e.reason = 'Sisesta visiidi põhjus'
    else if (form.reason.length > 200) e.reason = 'Põhjus on liiga pikk (max 200)'
    if (!form.vet.trim()) e.vet = 'Sisesta veterinaari nimi'
    if (form.notes.length > 1000) e.notes = 'Märkused on liiga pikad (max 1000)'
    return e
  }

  async function handleSubmit(event) {
    event.preventDefault()
    const e = validate()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    setSending(true)
    try {
      await api.addVetVisit(id, form)
      setMessage({ type: 'success', text: 'Visiit lisatud ✔' })
      setForm(emptyForm)
      setReload(reload + 1)
      setTimeout(() => setMessage(null), 3500)
    } catch (err) {
      setMessage({ type: 'error', text: err.message })
    } finally {
      setSending(false)
    }
  }

  function update(name, value) {
    setForm({ ...form, [name]: value })
  }

  return (
    <div className="container section">
      <Link to="/haldus" className="back-link">← Tagasi haldusesse</Link>

      <div className="section-head">
        <h2>{animal ? speciesEmoji(animal.species) + ' ' + animal.name : 'Visiidid'}</h2>
        {animal && <p className="muted">{animal.species} · {ageText(animal.age)}</p>}
      </div>

      {message && <p className={'alert toast alert-' + message.type}>{message.text}</p>}

      <div className="admin-layout">
        <form className="admin-form" onSubmit={handleSubmit} noValidate>
          <h2>Lisa visiit</h2>

          <div className="field">
            <label htmlFor="v-date">Kuupäev</label>
            <input id="v-date" type="date" value={form.visitDate}
              onChange={(e) => update('visitDate', e.target.value)}
              className={errors.visitDate ? 'invalid' : ''} />
            {errors.visitDate && <span className="field-error">{errors.visitDate}</span>}
          </div>

          <div className="field">
            <label htmlFor="v-reason">Põhjus</label>
            <input id="v-reason" value={form.reason} placeholder="nt Marutaudi vaktsiin"
              onChange={(e) => update('reason', e.target.value)}
              className={errors.reason ? 'invalid' : ''} />
            {errors.reason && <span className="field-error">{errors.reason}</span>}
          </div>

          <div className="field">
            <label htmlFor="v-vet">Veterinaar</label>
            <input id="v-vet" value={form.vet} placeholder="nt Dr. Tamm"
              onChange={(e) => update('vet', e.target.value)}
              className={errors.vet ? 'invalid' : ''} />
            {errors.vet && <span className="field-error">{errors.vet}</span>}
          </div>

          <div className="field">
            <label htmlFor="v-notes">Märkused</label>
            <textarea id="v-notes" rows="4" value={form.notes}
              onChange={(e) => update('notes', e.target.value)}
              className={errors.notes ? 'invalid' : ''} />
            {errors.notes && <span className="field-error">{errors.notes}</span>}
          </div>

          <button className="btn btn-primary" disabled={sending}>
            {sending ? 'Salvestan...' : 'Lisa visiit'}
          </button>
        </form>

        <div className="admin-list">
          <h2 className="list-title">Visiitide ajalugu</h2>

          {visits && visits.length === 0 && (
            <div className="empty">
              <span>🩺</span>
              <p>Sellel loomal pole veel visiite.</p>
            </div>
          )}

          <ol className="timeline">
            {visits && visits.map((v) => (
              <li key={v.id}>
                <span className="timeline-date">{dateOnly(v.visitDate)}</span>
                <div className="timeline-body">
                  <strong>{v.reason}</strong>
                  <span className="muted">{v.vet}</span>
                  {v.notes && <p>{v.notes}</p>}
                </div>
              </li>
            ))}
          </ol>
        </div>
      </div>
    </div>
  )
}

export default VetVisitsPage
