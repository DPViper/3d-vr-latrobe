import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './main.css'
import App from './App.jsx'
import { SceneContextProvider } from '@/SceneContext.jsx'
import { UIContextProvider } from '@/UIContext.jsx'

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <UIContextProvider>
            <SceneContextProvider>
                <App />
            </SceneContextProvider>
        </UIContextProvider>
    </StrictMode>
)