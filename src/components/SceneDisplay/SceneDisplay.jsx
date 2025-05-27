import { Canvas } from '@react-three/fiber'
import { AdaptiveDpr } from '@react-three/drei'
import { useState, useEffect, useContext } from 'react'

import Scene from './Scene/Scene.jsx'
import './SceneDisplay.css'

import { SceneContext } from '@/SceneContext.jsx'
import { UIContext } from '@/UIContext.jsx'

import { XR, createXRStore } from '@react-three/xr'

const store = createXRStore()

export default function SceneDisplay() {
    const { buildingID, setBuildingID } = useContext(SceneContext)
    const { viewportSize, infoBoxVisible, isMobileEnv } = useContext(UIContext)
    const [ sceneHeight, setSceneHeight ] = useState(viewportSize.height)

    useEffect(()=>{
        if(infoBoxVisible==null)
            setSceneHeight(1000);
        if (isMobileEnv) 
            setSceneHeight(!infoBoxVisible ? viewportSize.height : (2*viewportSize.height/3))
        else
            setSceneHeight(viewportSize.height)
    },[viewportSize, infoBoxVisible])

    return (
        <div
            id="scene-display"
            className={infoBoxVisible ? "windowed" : "fullscreen"}
            style={{height: `${sceneHeight}px`}}>

            <button onClick={() => store.enterVR()}>Enter VR</button>
           
            <Canvas
                flat
                gl={{ antialias: false }}
                dpr={[0.5, 2.5]}
                camera={{ fov: 60, position: [0, 3, 0] }}> {/* 'flat' required so text white. Tonemapping issue */}
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