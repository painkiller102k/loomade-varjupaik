const year = new Date().getFullYear()

function Footer() {
  return (
    <footer className="footer">
      <div className="container footer-inner">
        <p>🐾 Käpakodu – loomade varjupaik</p>
        <p className="muted">Iga käpp väärib kodu. © {year}</p>
      </div>
    </footer>
  )
}

export default Footer
