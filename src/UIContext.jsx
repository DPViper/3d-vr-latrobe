import { createContext, useState, useEffect } from 'react';

export const UIContext = createContext();

export function UIContextProvider({ children }) {
    const [viewportSize, setViewportSize] = useState({
        height: window.innerHeight,
        width: window.innerWidth
    })

    const [isMobileEnv, setIsMobileEnv] = useState(window.innerWidth <= 700)
    const [infoBoxVisible, setInfoBoxVisible] = useState(false)
    const [showListView, setShowListView] = useState(false);

    useEffect(() => {
        const handleWindowResize = () => {
            setViewportSize({
                height: window.innerHeight,
                width: window.innerWidth
            })
            setIsMobileEnv(window.innerWidth <= 550);
        }

        window.addEventListener('resize', handleWindowResize);

        return () => {window.removeEventListener('resize', handleWindowResize);}
    }, [])


    return (
        <UIContext.Provider value={{
            viewportSize,
            isMobileEnv,
            infoBoxVisible,
            setInfoBoxVisible,
            showListView,
            setShowListView
        }}>
            {children}
        </UIContext.Provider>
    );
}