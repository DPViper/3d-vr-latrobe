import './MapTabButton.css'
import tabOpenImg from '@images/mapTabOpen.svg'

export default function MapTabButton({ onClick }) {

    const onClickInternal = () => {
        onClick()
    }

    return (
        <div className="RoundedBox FlexRow FlexItem_Fixed" id="MapTabButton" onClick={onClickInternal}>
            <h1>View Map</h1>
            <img src={tabOpenImg} alt="MAP" />
        </div>
    )
}