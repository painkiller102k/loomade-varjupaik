import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { API_URL } from './api'
import { useAuth } from './AuthContext'

// Töötaja avab API-ga püsiühenduse (WebSocket) ja saab uue taotluse teate kohe.
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
          setToast(message.data)
          // Taotluste leht kuulab seda sündmust ja laeb nimekirja uuesti
          window.dispatchEvent(new CustomEvent('application-received', { detail: message.data }))
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
      <span className="live-icon">🔔</span>
      <div>
        <strong>Uus taotlus!</strong>
        <p>{toast.applicantName} soovib adopteerida: {toast.animalName}</p>
        <Link to="/taotlused" onClick={() => setToast(null)}>Vaata taotlusi →</Link>
      </div>
      <button className="live-close" onClick={() => setToast(null)} aria-label="Sulge">×</button>
    </div>
  )
}

export default LiveNotifications
