import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './AuthContext'
import Navbar from './components/Navbar'
import Footer from './components/Footer'
import ScrollToTop from './components/ScrollToTop'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import AdminPage from './pages/AdminPage'
import VetVisitsPage from './pages/VetVisitsPage'
import ApplicationsPage from './pages/ApplicationsPage'
import MyApplicationsPage from './pages/MyApplicationsPage'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <ScrollToTop />
        <Navbar />
        <main>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/haldus" element={<AdminPage />} />
            <Route path="/haldus/:id/visiidid" element={<VetVisitsPage />} />
            <Route path="/taotlused" element={<ApplicationsPage />} />
            <Route path="/minu-taotlused" element={<MyApplicationsPage />} />
          </Routes>
        </main>
        <Footer />
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
