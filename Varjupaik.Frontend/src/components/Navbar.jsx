import { Link, NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../AuthContext'
import Logo from './Logo'

function Navbar() {
  const { user, logout, isStaff, isVisitor } = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate('/')
  }

  return (
    <header className="navbar">
      <div className="container navbar-inner">
        <Link to="/" className="brand">
          <Logo />
          <span>Käpakodu</span>
        </Link>

        <nav className="nav-links">
          <NavLink to="/" end>Loomad</NavLink>
          {isStaff && <NavLink to="/taotlused">Taotlused</NavLink>}
          {isStaff && <NavLink to="/haldus">Haldus</NavLink>}
          {isVisitor && <NavLink to="/minu-taotlused">Minu taotlused</NavLink>}

          {user ? (
            <>
              <span className="user-chip" title={user.email}>
                {user.name} <small>{user.role}</small>
              </span>
              <button className="btn btn-ghost" onClick={handleLogout}>Logi välja</button>
            </>
          ) : (
            <>
              <NavLink to="/login">Logi sisse</NavLink>
              <Link to="/register" className="btn btn-accent">Registreeru</Link>
            </>
          )}
        </nav>
      </div>
    </header>
  )
}

export default Navbar
