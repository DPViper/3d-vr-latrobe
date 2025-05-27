import { useContext } from 'react'
import { SceneContext } from '@/SceneContext.jsx'
import SceneDisplay from '@/components/SceneDisplay/SceneDisplay.jsx'
import UIContainer from '@/components/UIContainer/UIContainer.jsx'

export default function App() {
    const { isLoading, splatData } = useContext(SceneContext);

    if (isLoading) {
        return (<p>Loading Splat Data...</p>);
    }

    if (!splatData) {
        return (<p>Data Error, Please Try Again...</p>);
    }

    return (
        <>
            <UIContainer />
            <SceneDisplay />
        </>
    )
}