import './RibbonHeader.css'
import logo from '@images/latrobe_cisco_icon.png';

export default function RibbonHeader() {
    return (
        <div className="RoundedBox FlexItem_Fixed" id="RibbonHeader">
            <a href="https://www.latrobe.edu.au/research/centres/health/aiot">
                <img src={logo} alt="Latrobe Cisco Centre" />
            </a>
            <h1>Latrobe 3D Campus Viewer</h1>
        </div>
    )
}