import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api, photoUrl } from '../api'
import { useAuth } from '../AuthContext'
import { ageText, speciesEmoji } from '../helpers'

function ApplyModal({ animal, onClose }) {
  const { user, isVisitor } = useAuth()
  const [message, setMessage] = useState('')
  const [error, setError] = useState('')
  const [sending, setSending] = useState(false)
  const [done, setDone] = useState(false)

  // Escape sulgeb akna
  useEffect(() => {
    function onKey(e) {
      if (e.key === 'Escape') onClose()
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [onClose])

  async function handleSubmit(event) {
    event.preventDefault()

    // Kliendipoolne valideerimine
    if (message.trim().length < 10) {
      setError('Kirjuta vähemalt 10 tähemärki, miks soovid looma adopteerida.')
      return
    }

    setSending(true)
    setError('')
    try {
      await api.applyForAnimal(animal.id, message.trim())
      setDone(true)
    } catch (err) {
      setError(err.message)
    } finally {
      setSending(false)
    }
  }

  const photo = photoUrl(animal.photoUrl)

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal" role="dialog" aria-modal="true" onClick={(e) => e.stopPropagation()}>
        <button className="modal-close" onClick={onClose} aria-label="Sulge">×</button>

        <div className="modal-animal">
          <div className={'modal-avatar species-' + animal.species.toLowerCase()}>
            {photo ? <img src={photo} alt="" /> : speciesEmoji(animal.species)}
          </div>
          <div>
            <h2>{animal.name}</h2>
            <p className="muted">{animal.species} · {ageText(animal.age)}</p>
          </div>
        </div>

        {done ? (
          <div className="modal-success">
            <span>🎉</span>
            <h3>Taotlus on saadetud!</h3>
            <p>Meie töötajad vaatavad selle peagi üle ja võtavad sinuga ühendust.</p>
            <button className="btn btn-primary" onClick={onClose}>Sulge</button>
          </div>
        ) : !user ? (
          <div className="modal-info">
            <p>Taotluse esitamiseks pead olema sisse logitud.</p>
            <div className="modal-actions">
              <Link to="/login" className="btn btn-primary">Logi sisse</Link>
              <Link to="/register" className="btn btn-ghost">Registreeru</Link>
            </div>
          </div>
        ) : !isVisitor ? (
          <p className="alert alert-error">Taotlust saab esitada ainult külastaja (Visitor) konto.</p>
        ) : (
          <form onSubmit={handleSubmit} noValidate>
            <div className="field">
              <label htmlFor="message">Miks soovid {animal.name} adopteerida?</label>
              <textarea
                id="message"
                rows="5"
                maxLength="1000"
                placeholder="Räägi endast, oma kodust ja kogemusest loomadega..."
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                className={error ? 'invalid' : ''}
              />
              <span className="char-count">{message.length} / 1000</span>
            </div>
            {error && <p className="alert alert-error">{error}</p>}
            <button className="btn btn-accent btn-block" disabled={sending}>
              {sending ? 'Saadan...' : 'Saada taotlus 🐾'}
            </button>
          </form>
        )}
      </div>
    </div>
  )
}

export default ApplyModal
