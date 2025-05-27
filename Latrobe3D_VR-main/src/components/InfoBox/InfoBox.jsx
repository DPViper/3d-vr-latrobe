import { useState, useContext } from 'react'
import './InfoBox.css';
import { SceneContext } from '@/SceneContext.jsx'
import { UIContext } from '@/UIContext.jsx'

import InfoTabButton from './InfoTabButton/InfoTabButton.jsx';
import ListView from '@components/ListView/ListView.jsx';
import SwapViewButton from './SwapViewButton/SwapViewButton.jsx';

export default function InfoBox() {
    const { buildingID, splatData } = useContext(SceneContext)
    const {isMobileEnv,
        infoBoxVisible,
        setInfoBoxVisible,
        showListView,
        setShowListView } = useContext(UIContext)

    const [hoverState, setHoverState] = useState('hide')

    const toggleListView = () => {
        setShowListView(!showListView);
    }

    const getBuildingTitle = () => {
        return splatData.find((splat) => splat.id === buildingID).name
    }

    const getCurrentBuilding = () => {
        return splatData.find((splat) => splat.id === buildingID)
    }

    const handleHover = (state) => {
        if (!infoBoxVisible) {
            setHoverState(state)
        }
    }

    return (
        <div className={`RoundedBox ${infoBoxVisible ? 'show' : hoverState} FlexItem_Dynamic`}
            id="InfoBox"
            onMouseEnter={() => handleHover('hover')}
            onMouseLeave={() => handleHover('hide')}>

            <InfoTabButton onClick={() => { setInfoBoxVisible(!infoBoxVisible); handleHover('hide') }} />
            <div className="FlexColumn" id="ContentContainer">
                <div id="TitleContainer">
                    <h1>{showListView ? (<>Select Building:</>) : (getBuildingTitle())}</h1>
                </div>
                {showListView ?
                    (<ListView items={splatData} />) : (
                        <>
                            <hr id="RuledLine" />
                            <table id="BuildingInfoContainer">
                                <thead></thead>
                                <tbody>
                                    <tr>
                                        <td className="left">Building Code</td>
                                        <td className="right">{getCurrentBuilding().code}</td>
                                    </tr>
                                    <tr>
                                        <td className="left">Closest Carpark</td>
                                        <td className="right">{getCurrentBuilding().closest_car_park}</td>
                                    </tr>
                                    <tr>
                                        <td className="left">Constructed/Refurbished</td>
                                        <td className="right">{getCurrentBuilding().constructed}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <hr id="RuledLine" />

                            <div id="InfoTextContainer">
                                <p>{getCurrentBuilding().description}</p>
                            </div>
                        </>)}
            </div>
            <SwapViewButton onClick={toggleListView} />
        </div>
    )
}
//


{/*<div id="SelectorContainer">
                <BuildingSelector splats={splats} setBuildingID={setBuildingID} />
            </div>*/}