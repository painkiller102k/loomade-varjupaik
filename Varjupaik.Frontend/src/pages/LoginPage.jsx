import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import AuthLayout from '../components/AuthLayout'

function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ email: '', password: '' })
  const [errors, setErrors] = useState({})
  const [serverError, setServerError] = useState('')
  const [sending, setSending] = useState(false)

  function validate() {
    const e = {}
    if (!form.email.trim()) e.email = 'Sisesta email'
    if (!form.password) e.password = 'Sisesta parool'
    return e
  }

  async function handleSubmit(event) {
    event.preventDefault()
    const e = validate()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    setSending(true)
    setServerError('')
    try {
      const user = await api.login(form)
      login(user)
      navigate(user.role === 'Admin' || user.role === 'Staff' ? '/haldus' : '/')
    } catch (err) {
      setServerError(err.message)
    } finally {
      setSending(false)
    }
  }

  return (
    <AuthLayout title="Tere tulemast tagasi!" subtitle="Logi oma kontoga sisse">
      <form onSubmit={handleSubmit} noValidate>
        {serverError && <p className="alert alert-error">{serverError}</p>}

        <div className="field">
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            className={errors.email ? 'invalid' : ''}
          />
          {errors.email && <span className="field-error">{errors.email}</span>}
        </div>

        <div className="field">
          <label htmlFor="password">Parool</label>
          <input
            id="password"
            type="password"
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            className={errors.password ? 'invalid' : ''}
          />
          {errors.password && <span className="field-error">{errors.password}</span>}
        </div>

        <button className="btn btn-primary btn-block" disabled={sending}>
          {sending ? 'Login...' : 'Logi sisse'}
        </button>
      </form>

      <p className="auth-switch">
        Pole veel kontot? <Link to="/register">Registreeru</Link>
      </p>
    </AuthLayout>
  )
}

export default LoginPage
