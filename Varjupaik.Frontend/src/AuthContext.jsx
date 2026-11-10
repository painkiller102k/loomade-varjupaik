import { createContext, useContext, useState } from 'react'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => JSON.parse(localStorage.getItem('user') || 'null'))

  function login(userData) {
    localStorage.setItem('user', JSON.stringify(userData))
    setUser(userData)
  }

  function logout() {
    localStorage.removeItem('user')
    setUser(null)
  }

  const isStaff = user && (user.role === 'Admin' || user.role === 'Staff')
  const isVisitor = user && user.role === 'Visitor'

  return (
    <AuthContext.Provider value={{ user, login, logout, isStaff, isVisitor }}>
      {children}
    </AuthContext.Provider>
  )
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  return useContext(AuthContext)
}
