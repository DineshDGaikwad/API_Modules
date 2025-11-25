import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { DoctorsPage } from './pages/DoctorsPage.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <DoctorsPage/>
    <App />
  </StrictMode>,
)
