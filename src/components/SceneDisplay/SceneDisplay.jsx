import { Canvas } from '@react-three/fiber'
import { AdaptiveDpr } from '@react-three/drei'
import { useState, useEffect, useContext } from 'react'
import { XR, createXRStore } from '@react-three/xr'

import Scene from './Scene/Scene.jsx'
import './SceneDisplay.css'

import { SceneContext } from '@/SceneContext.jsx'
import { UIContext } from '@/UIContext.jsx'

const store = createXRStore()

export default function SceneDisplay() {
    const { buildingID, setBuildingID } = useContext(SceneContext)
    const { viewportSize, infoBoxVisible, isMobileEnv } = useContext(UIContext)
    const [sceneHeight, setSceneHeight] = useState(viewportSize.height)
    const [isVRSupported, setIsVRSupported] = useState(false)

    useEffect(() => {
        // Check if VR is supported
        if (navigator.xr) {
            navigator.xr.isSessionSupported('immersive-vr')
                .then(supported => setIsVRSupported(supported))
                .catch(() => setIsVRSupported(false))
        }
    }, [])

    useEffect(() => {
        if (infoBoxVisible == null) {
            setSceneHeight(1000)
        } else if (isMobileEnv) {
            setSceneHeight(!infoBoxVisible ? viewportSize.height : (2 * viewportSize.height / 3))
        } else {
            setSceneHeight(viewportSize.height)
        }
    }, [viewportSize, infoBoxVisible, isMobileEnv])

    return (
        <div
            id="scene-display"
            className={infoBoxVisible ? "windowed" : "fullscreen"}
            style={{ height: `${sceneHeight}px` }}>
            
            {isVRSupported && (
                <button 
                    className="vr-button"
                    onClick={() => store.enterVR()}
                    title="Enter VR Mode">
                    Enter VR
                </button>
            )}
           
            <Canvas
                flat
                gl={{ antialias: false }}
                dpr={[0.5, 1]}
                camera={{ fov: 60, position: [0, 3, 0] }}>
                <XR store={store}>
                    <AdaptiveDpr pixelated />
                    <Scene
                        targetBuildingID={buildingID}
                        setTargetBuildingID={setBuildingID} />
                </XR>
            </Canvas>
        </div>
    )
}