import React from 'react';
import './StartButton.css';

interface StartButtonProps {
    onStart: () => void;
    onHover: () => void;
}

export const StartButton: React.FC<StartButtonProps> = ({ onStart, onHover }) => {
    return (
        <div className="centered-container">
            <button
                className="boton-start-royale"
                onClick={onStart}
                onMouseEnter={onHover}
            >
                START
            </button>
        </div>
    );
};