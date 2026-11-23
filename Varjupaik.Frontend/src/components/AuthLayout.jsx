import Dachshund from './Dachshund'
import Logo from './Logo'
import Paw from './Paw'
import { CheckIcon } from './Icons'

// Ühine kujundus sisselogimise ja registreerimise lehele.
// Takskoer piilub vormi akna tagant ja vehib käpaga.
function AuthLayout({ title, subtitle, children }) {
  return (
    <div className="auth-page">
      <div className="container auth-shell">
        <aside className="auth-panel">
          <Paw className="panel-paw panel-paw-1" />
          <Paw className="panel-paw panel-paw-2" />
          <div className="panel-brand">
            <Logo />
            <span>Loomade varjupaik</span>
          </div>
          <h2>Iga käpp väärib teist võimalust</h2>
          <p>Tee konto ja aita mõnel loomal leida endale armastav kodu.</p>
          <ul>
            <li><CheckIcon /> Sirvi kõiki loomi ja filtreeri neid</li>
            <li><CheckIcon /> Esita adopteerimise taotlus mõne klikiga</li>
            <li><CheckIcon /> Jälgi oma taotluse olekut</li>
          </ul>
        </aside>

        <div className="auth-form-wrap">
          <div className="auth-box">
            <Dachshund className="auth-dog" />
            <h1>{title}</h1>
            <p className="muted auth-sub">{subtitle}</p>
            {children}
          </div>
        </div>
      </div>
    </div>
  )
}

export default AuthLayout
