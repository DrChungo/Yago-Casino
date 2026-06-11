import React from 'react';
import type { AnimalRanking } from '../../services/rankingService';
import '../../styles/Ranking.css';

interface Props {
    animal: AnimalRanking;
    visible: boolean;
}

export default function AnimalTooltip({ animal, visible }: Props) {
    if (!visible) return null;

    return (
        <div className="animal-tooltip">
            <div className="tooltip-row">
                <span className="tooltip-label">🎂 Edad</span>
                <span className="tooltip-value">{animal.age} años</span>
            </div>
            <div className="tooltip-row">
                <span className="tooltip-label">⚖️ Peso</span>
                <span className="tooltip-value">{animal.weight.toLocaleString()} kg</span>
            </div>
            <div className="tooltip-row">
                <span className="tooltip-label">📏 Altura</span>
                <span className="tooltip-value">{animal.height} cm</span>
            </div>
            <div className="tooltip-row">
                <span className="tooltip-label">❤️ Salud</span>
                <span className={`tooltip-health health-${animal.health.toLowerCase()}`}>
                    {animal.health}
                </span>
            </div>
        </div>
    );
}