import { useEffect, useState } from 'react'
import { Link, Navigate, useParams } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import { ageText, dateOnly, speciesEmoji } from '../helpers'

const today = new Date().toISOString().slice(0, 10)
const emptyVisit = { visitDate: today, reason: '', vet: '', notes: '' }
const emptyVaccine = { name: '', vaccinationDate: today, nextDueDate: '' }

// Töötaja leht: looma veterinaarivisiitide ajalugu ja vaktsineerimised
function VetVisitsPage() {
  const { id } = useParams()
  const { isStaff } = useAuth()
  const [animal, setAnimal] = useState(null)
  const [visits, setVisits] = useState(null)
  const [vaccinations, setVaccinations] = useState(null)
  const [form, setForm] = useState(emptyVisit)
  const [vaccineForm, setVaccineForm] = useState(emptyVaccine)
  const [errors, setErrors] = useState({})
  const [vaccineErrors, setVaccineErrors] = useState({})
  const [message, setMessage] = useState(null)
  const [sending, setSending] = useState(false)
  const [reload, setReload] = useState(0)

  useEffect(() => {
    if (!isStaff) return
    let ignore = false
    Promise.all([api.getAnimal(id), api.getVetVisits(id), api.getVaccinations(id)])
      .then(([a, v, vac]) => {
        if (!ignore) {
          setAnimal(a)
          setVisits(v)
          setVaccinations(vac)
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

  function showMessage(type, text) {
    setMessage({ type, text })
    setTimeout(() => setMessage(null), 3500)
  }

  function validateVisit() {
    const e = {}
    if (!form.visitDate) e.visitDate = 'Vali kuupäev'
    if (!form.reason.trim()) e.reason = 'Sisesta visiidi põhjus'
    else if (form.reason.length > 200) e.reason = 'Põhjus on liiga pikk (max 200)'
    if (!form.vet.trim()) e.vet = 'Sisesta veterinaari nimi'
    if (form.notes.length > 1000) e.notes = 'Märkused on liiga pikad (max 1000)'
    return e
  }

  function validateVaccine() {
    const e = {}
    if (!vaccineForm.name.trim()) e.name = 'Sisesta vaktsiini nimi'
    else if (vaccineForm.name.length > 100) e.name = 'Nimi on liiga pikk (max 100)'
    if (!vaccineForm.vaccinationDate) e.vaccinationDate = 'Vali kuupäev'
    if (vaccineForm.nextDueDate && vaccineForm.nextDueDate <= vaccineForm.vaccinationDate) {
      e.nextDueDate = 'Järgmine tähtaeg peab olema hilisem'
    }
    return e
  }

  async function handleVisit(event) {
    event.preventDefault()
    const e = validateVisit()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    setSending(true)
    try {
      await api.addVetVisit(id, form)
      showMessage('success', 'Visiit lisatud ✔')
      setForm(emptyVisit)
      setReload(reload + 1)
    } catch (err) {
      showMessage('error', err.message)
    } finally {
      setSending(false)
    }
  }

  async function handleVaccine(event) {
    event.preventDefault()
    const e = validateVaccine()
    setVaccineErrors(e)
    if (Object.keys(e).length > 0) return

    setSending(true)
    try {
      await api.addVaccination(id, {
        name: vaccineForm.name,
        vaccinationDate: vaccineForm.vaccinationDate,
        nextDueDate: vaccineForm.nextDueDate || null,
      })
      showMessage('success', 'Vaktsineerimine lisatud ✔')
      setVaccineForm(emptyVaccine)
      setReload(reload + 1)
    } catch (err) {
      showMessage('error', err.message)
    } finally {
      setSending(false)
    }
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
        <div className="forms-stack">
          <form className="admin-form" onSubmit={handleVisit} noValidate>
            <h2>Lisa visiit</h2>

            <div className="field">
              <label htmlFor="v-date">Kuupäev</label>
              <input id="v-date" type="date" value={form.visitDate}
                onChange={(e) => setForm({ ...form, visitDate: e.target.value })}
                className={errors.visitDate ? 'invalid' : ''} />
              {errors.visitDate && <span className="field-error">{errors.visitDate}</span>}
            </div>

            <div className="field">
              <label htmlFor="v-reason">Põhjus</label>
              <input id="v-reason" value={form.reason} placeholder="nt Hammaste kontroll"
                onChange={(e) => setForm({ ...form, reason: e.target.value })}
                className={errors.reason ? 'invalid' : ''} />
              {errors.reason && <span className="field-error">{errors.reason}</span>}
            </div>

            <div className="field">
              <label htmlFor="v-vet">Veterinaar</label>
              <input id="v-vet" value={form.vet} placeholder="nt Dr. Tamm"
                onChange={(e) => setForm({ ...form, vet: e.target.value })}
                className={errors.vet ? 'invalid' : ''} />
              {errors.vet && <span className="field-error">{errors.vet}</span>}
            </div>

            <div className="field">
              <label htmlFor="v-notes">Märkused</label>
              <textarea id="v-notes" rows="3" value={form.notes}
                onChange={(e) => setForm({ ...form, notes: e.target.value })}
                className={errors.notes ? 'invalid' : ''} />
              {errors.notes && <span className="field-error">{errors.notes}</span>}
            </div>

            <button className="btn btn-primary" disabled={sending}>Lisa visiit</button>
          </form>

          <form className="admin-form" onSubmit={handleVaccine} noValidate>
            <h2>Lisa vaktsineerimine</h2>

            <div className="field">
              <label htmlFor="c-name">Vaktsiin</label>
              <input id="c-name" value={vaccineForm.name} placeholder="nt Marutaudi"
                onChange={(e) => setVaccineForm({ ...vaccineForm, name: e.target.value })}
                className={vaccineErrors.name ? 'invalid' : ''} />
              {vaccineErrors.name && <span className="field-error">{vaccineErrors.name}</span>}
            </div>

            <div className="field-row-even">
              <div className="field">
                <label htmlFor="c-date">Vaktsineeriti</label>
                <input id="c-date" type="date" value={vaccineForm.vaccinationDate}
                  onChange={(e) => setVaccineForm({ ...vaccineForm, vaccinationDate: e.target.value })}
                  className={vaccineErrors.vaccinationDate ? 'invalid' : ''} />
                {vaccineErrors.vaccinationDate && <span className="field-error">{vaccineErrors.vaccinationDate}</span>}
              </div>

              <div className="field">
                <label htmlFor="c-next">Järgmine kord</label>
                <input id="c-next" type="date" value={vaccineForm.nextDueDate}
                  onChange={(e) => setVaccineForm({ ...vaccineForm, nextDueDate: e.target.value })}
                  className={vaccineErrors.nextDueDate ? 'invalid' : ''} />
                {vaccineErrors.nextDueDate && <span className="field-error">{vaccineErrors.nextDueDate}</span>}
              </div>
            </div>

            <button className="btn btn-primary" disabled={sending}>Lisa vaktsineerimine</button>
          </form>
        </div>

        <div className="forms-stack">
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

          <div className="admin-list">
            <h2 className="list-title">💉 Vaktsineerimised</h2>

            {vaccinations && vaccinations.length === 0 && (
              <p className="muted">Sellel loomal pole veel vaktsineerimisi.</p>
            )}

            <ul className="vaccine-history">
              {vaccinations && vaccinations.map((v) => (
                <li key={v.id}>
                  <strong>{v.name}</strong>
                  <span className="muted">tehtud {dateOnly(v.vaccinationDate)}</span>
                  {v.nextDueDate && <span className="due-pill due-ok">järgmine {dateOnly(v.nextDueDate)}</span>}
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  )
}

export default VetVisitsPage
