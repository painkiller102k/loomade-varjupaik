// Kõik päringud API-le käivad läbi selle faili
export const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5156'

// Pildi täielik aadress (API tagastab /uploads/...)
export function photoUrl(path) {
  if (!path) return null
  return path.startsWith('http') ? path : API_URL + path
}

function getToken() {
  const user = JSON.parse(localStorage.getItem('user') || 'null')
  return user ? user.token : null
}

// Teeb veateate API vastusest (ProblemDetails, validation errors või { message })
async function errorMessage(response) {
  if (response.status === 401) return 'Palun logi uuesti sisse.'
  if (response.status === 403) return 'Sul pole selleks õigust.'
  try {
    const data = await response.json()
    if (data.message) return data.message
    if (data.errors) return Object.values(data.errors).flat().join(' ')
    if (data.detail) return data.detail
    if (data.title) return data.title
  } catch {
    // vastus ei olnud JSON
  }
  return 'Midagi läks valesti (' + response.status + ')'
}

async function request(path, options = {}) {
  const headers = { ...(options.headers || {}) }
  const token = getToken()
  if (token) headers.Authorization = 'Bearer ' + token

  let response
  try {
    response = await fetch(API_URL + path, { ...options, headers })
  } catch {
    // API ei ole käivitatud või aadress on vale
    throw new Error('Serveriga ei saa ühendust. Kontrolli, et API töötab (' + API_URL + ').')
  }
  if (!response.ok) {
    throw new Error(await errorMessage(response))
  }
  if (response.status === 204) return null
  return response.json()
}

function json(method, body) {
  return {
    method,
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  }
}

// Eemaldab tühjad parameetrid, et URL oleks puhas
function query(params) {
  const clean = {}
  for (const key in params) {
    if (params[key] !== '' && params[key] !== null && params[key] !== undefined) {
      clean[key] = params[key]
    }
  }
  return new URLSearchParams(clean).toString()
}

export const api = {
  getAnimals: (params) => request('/api/animals?' + query(params)),
  getAnimal: (id) => request('/api/animals/' + id),
  createAnimal: (formData) => request('/api/animals', { method: 'POST', body: formData }),
  updateAnimal: (id, formData) => request('/api/animals/' + id, { method: 'PUT', body: formData }),
  deleteAnimal: (id) => request('/api/animals/' + id, { method: 'DELETE' }),

  login: (data) => request('/api/auth/login', json('POST', data)),
  register: (data) => request('/api/auth/register', json('POST', data)),

  getApplications: (status) => request('/api/applications?' + query({ status })),
  getMyApplications: () => request('/api/applications/my'),
  changeApplicationStatus: (id, status) =>
    request('/api/applications/' + id + '/status', json('PATCH', { status })),

  applyForAnimal: (animalId, message) =>
    request('/api/animals/' + animalId + '/applications', json('POST', { message })),
}
