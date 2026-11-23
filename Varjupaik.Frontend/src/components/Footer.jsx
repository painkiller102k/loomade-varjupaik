import { Link } from 'react-router-dom'
import Logo from './Logo'

const year = new Date().getFullYear()

function Footer() {
  return (
    <footer className="footer">
      <div className="container footer-grid">
        <div>
          <div className="footer-brand">
            <Logo />
            <span>Loomade varjupaik</span>
          </div>
          <p className="footer-text">
            Aitame hüljatud loomadel leida armastav kodu. Iga käpp väärib teist võimalust.
          </p>
        </div>

        <div>
          <h4>Lehed</h4>
          <ul>
            <li><Link to="/">Loomad</Link></li>
            <li><Link to="/login">Logi sisse</Link></li>
            <li><Link to="/register">Registreeru</Link></li>
          </ul>
        </div>
      </div>

      <div className="container footer-bottom">
        <span>© {year} Loomade varjupaik</span>
      </div>
    </footer>
  )
}

export default Footer
