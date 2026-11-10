import { useEffect } from 'react'
import { useLocation } from 'react-router-dom'

// Uuele lehele minnes keritakse lehe algusesse
function ScrollToTop() {
  const { pathname } = useLocation()

  useEffect(() => {
    window.scrollTo(0, 0)
  }, [pathname])

  return null
}

export default ScrollToTop
