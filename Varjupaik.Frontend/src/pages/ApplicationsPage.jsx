import { useEffect, useState } from 'react'
import { Navigate } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import { applicationStatusNames, dateText, speciesEmoji, statusNames } from '../helpers'

const filters = [
  { value: '', label: 'Kõik' },
  { value: 'New', label: 'Ootel' },
  { value: 'Approved', label: 'Kinnitatud' },
  { value: 'Rejected', label: 'Tagasi lükatud' },
]

// Töötaja leht: kõik adopteerimise taotlused
function ApplicationsPage() {
  const { isStaff } = useAuth()
  const [status, setStatus] = useState('New')
  const [applications, setApplications] = useState(null)
  const [message, setMessage] = useState(null)
  const [reload, setReload] = useState(0)

  useEffect(() => {
    if (!isStaff) return
    let ignore = false
    api.getApplications(status)
      .then((data) => {
        if (!ignore) setApplications(data)
      })
      .catch((err) => {
        if (!ignore) setMessage({ type: 'error', text: err.message })
      })
    return () => {
      ignore = true
    }
  }, [status, reload, isStaff])

  // Uus taotlus tuli WebSocketi kaudu: laeme nimekirja uuesti
  useEffect(() => {
    function onReceived() {
      setReload((r) => r + 1)
    }
    window.addEventListener('application-received', onReceived)
    return () => window.removeEventListener('application-received', onReceived)
  }, [])

  if (!isStaff) {
    return <Navigate to="/login" />
  }

  function showMessage(type, text) {
    setMessage({ type, text })
    setTimeout(() => setMessage(null), 3500)
  }

  async function handleChange(application, newStatus) {
    const text = newStatus === 'Approved'
      ? 'Kas kinnitada taotlus? ' + application.animalName + ' märgitakse adopteerituks.'
      : 'Kas lükata taotlus tagasi?'
    if (!window.confirm(text)) return

    try {
      await api.changeApplicationStatus(application.id, newStatus)
      showMessage('success', newStatus === 'Approved' ? 'Taotlus kinnitatud ✔' : 'Taotlus tagasi lükatud')
      setReload(reload + 1)
    } catch (err) {
      showMessage('error', err.message)
    }
  }

  return (
    <div className="container section">
      <div className="section-head">
        <h2>Adopteerimise taotlused</h2>
        {applications && <p className="muted">Leitud {applications.length} taotlust</p>}
      </div>

      {message && <p className={'alert toast alert-' + message.type}>{message.text}</p>}

      <div className="species-chips">
        {filters.map((f) => (
          <button
            key={f.value}
            className={'chip' + (status === f.value ? ' active' : '')}
            onClick={() => setStatus(f.value)}
          >
            {f.label}
          </button>
        ))}
      </div>

      {applications && applications.length === 0 && (
        <div className="empty">
          <span>📭</span>
          <p>Selles olekus taotlusi pole.</p>
        </div>
      )}

      <div className="application-list">
        {applications && applications.map((a) => (
          <article key={a.id} className="application-card">
            <div className="application-top">
              <div>
                <h3>{speciesEmoji(a.animalSpecies)} {a.animalName}</h3>
                <p className="muted">Looma olek: {statusNames[a.animalStatus]}</p>
              </div>
              <span className={'app-status app-' + a.status}>{applicationStatusNames[a.status]}</span>
            </div>

            <p className="application-message">“{a.message}”</p>

            <div className="application-bottom">
              <div className="muted">
                <strong>{a.applicantName}</strong> · {a.applicantEmail}
                <br />
                {dateText(a.createdAt)}
              </div>

              {a.status === 'New' && (
                <div className="admin-actions">
                  <button className="btn btn-primary btn-small" onClick={() => handleChange(a, 'Approved')}>
                    Kinnita
                  </button>
                  <button className="btn btn-danger btn-small" onClick={() => handleChange(a, 'Rejected')}>
                    Lükka tagasi
                  </button>
                </div>
              )}
            </div>
          </article>
        ))}
      </div>
    </div>
  )
}

export default ApplicationsPage
