import { useEffect, useState } from 'react'
import { Link, Navigate } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import { applicationStatusNames, dateText, speciesEmoji } from '../helpers'

const info = {
  New: 'Meie töötajad vaatavad taotluse peagi üle.',
  Approved: 'Palju õnne! Võtame sinuga peagi ühendust.',
  Rejected: 'Kahjuks seekord ei õnnestunud, aga meil on veel palju loomi.',
}

// Külastaja leht: enda esitatud taotlused
function MyApplicationsPage() {
  const { isVisitor } = useAuth()
  const [applications, setApplications] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!isVisitor) return
    api.getMyApplications()
      .then(setApplications)
      .catch((err) => setError(err.message))
  }, [isVisitor])

  if (!isVisitor) {
    return <Navigate to="/login" />
  }

  return (
    <div className="container section">
      <div className="section-head">
        <h2>Minu taotlused</h2>
      </div>

      {error && <p className="alert alert-error">{error}</p>}

      {applications && applications.length === 0 && (
        <div className="empty">
          <span>🐾</span>
          <p>Sa pole veel ühtegi taotlust esitanud.</p>
          <Link to="/" className="btn btn-accent">Vaata loomi</Link>
        </div>
      )}

      <div className="application-list">
        {applications && applications.map((a) => (
          <article key={a.id} className="application-card">
            <div className="application-top">
              <div>
                <h3>{speciesEmoji(a.animalSpecies)} {a.animalName}</h3>
                <p className="muted">Esitatud {dateText(a.createdAt)}</p>
              </div>
              <span className={'app-status app-' + a.status}>{applicationStatusNames[a.status]}</span>
            </div>
            <p className="application-message">“{a.message}”</p>
            <p className="muted">{info[a.status]}</p>
          </article>
        ))}
      </div>
    </div>
  )
}

export default MyApplicationsPage
