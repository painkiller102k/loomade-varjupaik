import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api } from '../api'
import { useAuth } from '../AuthContext'
import AuthLayout from '../components/AuthLayout'

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

function RegisterPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ name: '', email: '', password: '', password2: '' })
  const [errors, setErrors] = useState({})
  const [serverError, setServerError] = useState('')
  const [sending, setSending] = useState(false)

  // Kliendipoolne valideerimine (API kontrollib sama asja uuesti)
  function validate() {
    const e = {}
    if (!form.name.trim()) e.name = 'Sisesta oma nimi'
    if (!emailRegex.test(form.email)) e.email = 'Email ei ole korrektne'
    if (form.password.length < 6) e.password = 'Parool peab olema vähemalt 6 märki'
    if (form.password !== form.password2) e.password2 = 'Paroolid ei kattu'
    return e
  }

  function update(name, value) {
    setForm({ ...form, [name]: value })
  }

  async function handleSubmit(event) {
    event.preventDefault()
    const e = validate()
    setErrors(e)
    if (Object.keys(e).length > 0) return

    setSending(true)
    setServerError('')
    try {
      const user = await api.register({ name: form.name, email: form.email, password: form.password })
      login(user)
      navigate('/')
    } catch (err) {
      setServerError(err.message)
    } finally {
      setSending(false)
    }
  }

  const fields = [
    { name: 'name', label: 'Nimi', type: 'text' },
    { name: 'email', label: 'Email', type: 'email' },
    { name: 'password', label: 'Parool', type: 'password' },
    { name: 'password2', label: 'Parool uuesti', type: 'password' },
  ]

  return (
    <AuthLayout title="Loo konto" subtitle="Registreeru, et esitada adopteerimise taotlus">
      <form onSubmit={handleSubmit} noValidate>
        {serverError && <p className="alert alert-error">{serverError}</p>}

        {fields.map((f) => (
          <div className="field" key={f.name}>
            <label htmlFor={f.name}>{f.label}</label>
            <input
              id={f.name}
              type={f.type}
              value={form[f.name]}
              onChange={(e) => update(f.name, e.target.value)}
              className={errors[f.name] ? 'invalid' : ''}
            />
            {errors[f.name] && <span className="field-error">{errors[f.name]}</span>}
          </div>
        ))}

        <button className="btn btn-accent btn-block" disabled={sending}>
          {sending ? 'Salvestan...' : 'Registreeru'}
        </button>
      </form>

      <p className="auth-switch">
        Konto juba olemas? <Link to="/login">Logi sisse</Link>
      </p>
    </AuthLayout>
  )
}

export default RegisterPage
