import React from 'react';
import './ListItem.css'

const arrowStyle = {
  fontSize: '32px',
};

export default function ListItem({ label, onClick }) {
    const onClickInternal = () => {
        //Handle internal click events
        onClick()
    }

  return (
    <div className="ListItem" onClick={onClickInternal}>
      <span id="Label">{label}</span>
      <span id="Label" style={arrowStyle}>&#8250;</span>
    </div>
  );
}