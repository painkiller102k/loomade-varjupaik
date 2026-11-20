import { useEffect, useState } from 'react'
import { api } from '../api'
import { dateText } from '../helpers'

const SEEN_KEY = 'lastSeenNotification'

// Töötaja menüü kell: teavituste nimekiri (nt meeldetuletused vaktsineerimistest)
function NotificationsBell() {
  const [items, setItems] = useState([])
  const [open, setOpen] = useState(false)
  const [seen, setSeen] = useState(() => Number(localStorage.getItem(SEEN_KEY) || 0))

  useEffect(() => {
    api.getNotifications()
      .then(setItems)
      .catch(() => setItems([]))
  }, [])

  // Uus teavitus tuli WebSocketi kaudu (vt LiveNotifications)
  useEffect(() => {
    function onNew(event) {
      setItems((prev) => [event.detail, ...prev.filter((n) => n.id !== event.detail.id)].slice(0, 20))
    }
    window.addEventListener('notification-received', onNew)
    return () => window.removeEventListener('notification-received', onNew)
  }, [])

  const unread = items.filter((n) => n.id > seen).length

  function toggle() {
    const next = !open
    setOpen(next)
    // Kella avamisel loeme kõik teavitused loetuks, aga uued jäävad nimekirjas esile tõstetud
    if (next && items.length > 0) {
      const maxId = Math.max(...items.map((n) => n.id))
      localStorage.setItem(SEEN_KEY, String(maxId))
      setTimeout(() => setSeen(maxId), 1500)
    }
  }

  return (
    <div className="bell-wrap">
      <button className="bell-btn" onClick={toggle} aria-label="Teavitused">
        🔔
        {unread > 0 && <span className="bell-badge">{unread}</span>}
      </button>

      {open && (
        <>
          <div className="bell-backdrop" onClick={() => setOpen(false)} />
          <div className="bell-panel">
            <h4>Teavitused</h4>
            {items.length === 0 && <p className="muted">Teavitusi pole.</p>}
            <ul>
              {items.map((n) => (
                <li key={n.id} className={n.id > seen ? 'is-new' : ''}>
                  <strong>{n.title}</strong>
                  <p>{n.message}</p>
                  <span className="muted">{dateText(n.createdAt)}</span>
                </li>
              ))}
            </ul>
          </div>
        </>
      )}
    </div>
  )
}

export default NotificationsBell
