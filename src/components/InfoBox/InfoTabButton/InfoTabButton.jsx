import './InfoTabButton.css'
import { useState, useEffect, useContext } from 'react'
import { UIContext } from '@/UIContext'
import tabOpenImg from '@images/infoTabOpen.svg'
import tabCloseImg from '@images/infoTabClose.svg'

export default function InfoTabButton({ onClick }) {
    const { infoBoxVisible } = useContext(UIContext)

    const [imgSource, setImgSource] = useState()
    const [toggle, setToggle] = useState(true)

    useEffect(() => {
        if(infoBoxVisible){
            setImgSource(tabCloseImg)
        } else {
            setImgSource(tabOpenImg)
        }
    })

    //Toggle image in box on click, propagate click upwards
    const onClickInternal = () => {
        if (toggle) {
            setImgSource(tabOpenImg)
        } else {
            setImgSource(tabCloseImg)
        }
        setToggle(!toggle)
        onClick()
    }

    return (
        <div className={`RoundedBox ${infoBoxVisible ? "opened":""}`}
          id="InfoTabButton" onClick={onClickInternal}>
            <img src={imgSource}
                alt="INFO" />
        </div>
    )
}