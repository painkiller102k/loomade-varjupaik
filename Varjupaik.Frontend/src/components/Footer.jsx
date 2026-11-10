function Footer() {
  return (
    <footer className="footer">
      <div className="container footer-inner">
        <p>🐾 Käpakodu – loomade varjupaik</p>
        <p className="muted">Iga käpp väärib kodu. © {new Date().getFullYear()}</p>
      </div>
    </footer>
  )
}

export default Footer
