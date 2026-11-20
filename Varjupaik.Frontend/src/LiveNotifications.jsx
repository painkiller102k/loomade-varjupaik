import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { API_URL } from './api'
import { useAuth } from './AuthContext'

// Töötaja avab API-ga püsiühenduse (WebSocket) ja saab teated kohe:
//  - ApplicationReceived: uus adopteerimise taotlus
//  - Notification: teavitus (nt meeldetuletus vaktsineerimisest taustateenuselt)
// Token läheb query-parameetris access_token, sest brauseri WebSocket ei saa päiseid saata.
function LiveNotifications() {
  const { user, isStaff } = useAuth()
  const [toast, setToast] = useState(null)
  const token = user ? user.token : null

  useEffect(() => {
    if (!isStaff || !token) return

    let socket = null
    let retryTimer = null
    let closedByUs = false

    function connect() {
      const url = API_URL.replace(/^http/, 'ws') + '/ws/applications?access_token=' + encodeURIComponent(token)
      socket = new WebSocket(url)

      socket.onmessage = (event) => {
        let message
        try {
          message = JSON.parse(event.data)
        } catch {
          return
        }

        if (message.type === 'ApplicationReceived') {
          setToast({
            icon: '🔔',
            title: 'Uus taotlus!',
            text: message.data.applicantName + ' soovib adopteerida: ' + message.data.animalName,
            link: '/taotlused',
            linkText: 'Vaata taotlusi →',
          })
          // Taotluste leht kuulab seda sündmust ja laeb nimekirja uuesti
          window.dispatchEvent(new CustomEvent('application-received', { detail: message.data }))
        }

        if (message.type === 'Notification') {
          setToast({ icon: '💉', title: message.data.title, text: message.data.message, link: null })
          // Kell menüüs kuulab seda sündmust
          window.dispatchEvent(new CustomEvent('notification-received', { detail: message.data }))
        }
      }

      // Kui ühendus katkeb (nt API restart), proovime 3 sekundi pärast uuesti
      socket.onclose = () => {
        if (!closedByUs) {
          retryTimer = setTimeout(connect, 3000)
        }
      }
    }

    connect()

    return () => {
      closedByUs = true
      clearTimeout(retryTimer)
      if (socket) socket.close()
    }
  }, [isStaff, token])

  // Teade kaob ise 8 sekundi pärast
  useEffect(() => {
    if (!toast) return
    const timer = setTimeout(() => setToast(null), 8000)
    return () => clearTimeout(timer)
  }, [toast])

  if (!toast) return null

  return (
    <div className="live-toast" role="status">
      <span className="live-icon">{toast.icon}</span>
      <div>
        <strong>{toast.title}</strong>
        <p>{toast.text}</p>
        {toast.link && <Link to={toast.link} onClick={() => setToast(null)}>{toast.linkText}</Link>}
      </div>
      <button className="live-close" onClick={() => setToast(null)} aria-label="Sulge">×</button>
    </div>
  )
}

export default LiveNotifications
