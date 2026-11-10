// Ühine kujundus sisselogimise ja registreerimise lehele
function AuthLayout({ title, subtitle, children }) {
  return (
    <div className="container auth-page">
      <div className="auth-card">
        <aside className="auth-side">
          <div className="auth-emojis" aria-hidden="true">
            <span>🐶</span>
            <span>🐱</span>
            <span>🐰</span>
          </div>
          <h2>Iga käpp väärib kodu</h2>
          <p>Logi sisse, et esitada adopteerimise taotlus ja leida endale uus parim sõber.</p>
        </aside>

        <div className="auth-form">
          <h1>{title}</h1>
          <p className="muted">{subtitle}</p>
          {children}
        </div>
      </div>
    </div>
  )
}

export default AuthLayout
