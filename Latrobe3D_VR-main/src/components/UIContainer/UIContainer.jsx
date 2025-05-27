import './UIContainer.css'
import RibbonHeader from '../RibbonHeader/RibbonHeader.jsx'
import InfoBox from '../InfoBox/InfoBox.jsx'
import MapTabButton from '../MapTabButton/MapTabButton.jsx'

export default function UIContainer() {
    const openMapPopup = () => {
        console.log("Open Map Popup")
        // Not Yet Implemented
    }

    return(
        <div id="UIContainer" className="FlexColumn">
            <RibbonHeader />
            <MapTabButton onClick={openMapPopup}/>
            <InfoBox/>
        </div>
    )
}