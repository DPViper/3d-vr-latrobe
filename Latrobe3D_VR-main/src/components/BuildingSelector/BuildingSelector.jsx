import './BuildingSelector.css'
import { useContext } from 'react'
import { SceneContext } from '@/SceneContext.jsx'

export default function BuildingSelector() {
    const { splatData, setBuildingID } = useContext(SceneContext)

    const handleChange = (event) => {
        const selectedID = parseInt(event.target.value, 10)
        setBuildingID(selectedID)
    }

    return (
        <div>
            <label id="SelectorLabel" htmlFor="building-select">Choose a building:</label>
            <select id="building-select" onChange={handleChange}>
                {splatData.map((splat) => (
                    <option key={splat.id} value={splat.id}>{splat.name}</option>
                ))}
            </select>
        </div>
    )
}