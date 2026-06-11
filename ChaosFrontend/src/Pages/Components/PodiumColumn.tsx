import { createPortal } from 'react-dom';
import type { AnimalRanking, UserData } from '../../services/rankingService';
import '../../styles/Ranking.css';
import { useCallback, useEffect, useRef, useState } from 'react';

interface Props {
    animal: AnimalRanking;
    owner: UserData | null;
    place: 1 | 2 | 3;
    svgImg: string;
}

const PLACE_CONFIG = {
    1: { label: '🥇', color: '#FFD700' },
    2: { label: '🥈', color: '#C0C0C0' },
    3: { label: '🥉', color: '#CD7F32' },
};

function getImageContentRect(img: HTMLImageElement): DOMRect {
    const box = img.getBoundingClientRect();
    const natW = img.naturalWidth || 1;
    const natH = img.naturalHeight || 1;
    const natRatio = natW / natH;
    const boxRatio = box.width / box.height;

    let renderedW: number;
    let renderedH: number;

    if (natRatio > boxRatio) {
        renderedW = box.width;
        renderedH = box.width / natRatio;
    } else {
        renderedH = box.height;
        renderedW = box.height * natRatio;
    }

    const offsetX = box.left + (box.width - renderedW) / 2;
    const offsetY = box.top + (box.height - renderedH);

    return new DOMRect(offsetX, offsetY, renderedW, renderedH);
}

export default function PodiumColumn({ animal, owner, place, svgImg }: Props) {
    const [hovered, setHovered] = useState(false);
    const [pos, setPos] = useState({ x: 0, y: 0, goesUp: true, scale: 1 });
    const imgRef = useRef<HTMLImageElement>(null);
    const tooltipRef = useRef<HTMLDivElement>(null);
    const rafRef = useRef<number | null>(null);
    const config = PLACE_CONFIG[place];

    const formattedValue = animal.value.toLocaleString('es-AR', {
        style: 'currency',
        currency: 'ARS',
        maximumFractionDigits: 0,
    });

    const calculate = useCallback(() => {
        if (!imgRef.current || !tooltipRef.current) return;

        const imgRect = getImageContentRect(imgRef.current);
        const vw = window.innerWidth;
        const vh = window.innerHeight;
        const GAP = 10;
        const MARGIN = 6;

        // ── Reset scale para medir tamaño natural del tooltip ──
        tooltipRef.current.style.transform = 'translateX(-50%) scale(1)';
        tooltipRef.current.style.transformOrigin = 'top center';

        const tip = tooltipRef.current.getBoundingClientRect();

        if (tip.width === 0 || tip.height === 0) {
            rafRef.current = requestAnimationFrame(calculate);
            return;
        }

        const spaceAbove = imgRect.top - GAP;
        const spaceBelow = vh - imgRect.bottom - GAP;
        const availableSpace = Math.max(spaceAbove, spaceBelow);

        // ✅ Calcular escala necesaria para que el tooltip quepa
        // Nunca mayor a 1 (no agrandamos), nunca menor a 0.45
        const scaleByHeight = availableSpace > 0
            ? Math.min(1, availableSpace / tip.height)
            : 1;
        const scaleByWidth = vw > 0
            ? Math.min(1, (vw - MARGIN * 2) / tip.width)
            : 1;
        const scale = Math.max(0.45, Math.min(scaleByHeight, scaleByWidth));

        const scaledH = tip.height * scale;
        const scaledW = tip.width * scale;

        let y: number;
        let goesUp: boolean;

        if (spaceAbove >= scaledH) {
            y = imgRect.top - scaledH - GAP;
            goesUp = true;
        } else if (spaceBelow >= scaledH) {
            y = imgRect.bottom + GAP;
            goesUp = false;
        } else {
            // Sin espacio suficiente → centrado verticalmente
            y = (vh - scaledH) / 2;
            goesUp = true;
        }

        const centerX = imgRect.left + imgRect.width / 2;
        const halfScaledW = scaledW / 2;
        const x = Math.min(
            Math.max(centerX, halfScaledW + MARGIN),
            vw - halfScaledW - MARGIN
        );

        setPos({ x, y, goesUp, scale });
    }, []);

    useEffect(() => {
        if (!hovered) return;

        rafRef.current = requestAnimationFrame(() => {
            rafRef.current = requestAnimationFrame(calculate);
        });

        const onResize = () => {
            if (rafRef.current) cancelAnimationFrame(rafRef.current);
            rafRef.current = requestAnimationFrame(calculate);
        };

        window.addEventListener('resize', onResize);

        return () => {
            if (rafRef.current) cancelAnimationFrame(rafRef.current);
            window.removeEventListener('resize', onResize);
        };
    }, [hovered, calculate]);

    const tooltip = (
        <div
            ref={tooltipRef}
            className={`animal-tooltip ${pos.goesUp ? 'tooltip-above' : 'tooltip-below'}`}
            style={{
                position: 'fixed',
                left: pos.x,
                top: pos.y,
                // ✅ scale aplicado junto con el translateX para no pisarse
                transform: `translateX(-50%) scale(${pos.scale})`,
                transformOrigin: pos.goesUp ? 'bottom center' : 'top center',
                borderColor: config.color,
                pointerEvents: 'none',
                opacity: hovered ? 1 : 0,
                visibility: 'visible',
                transition: 'opacity 0.15s ease',
            }}
        >
            <div className="tooltip-row">
                <span className="tooltip-label">🎂 Edad</span>
                <span className="tooltip-value">{animal.age} años</span>
            </div>
            <div className="tooltip-row">
                <span className="tooltip-label">⚖️ Peso</span>
                <span className="tooltip-value">{animal.weight} kg</span>
            </div>
            <div className="tooltip-row">
                <span className="tooltip-label">📏 Altura</span>
                <span className="tooltip-value">{animal.height} cm</span>
            </div>
            <div className="tooltip-row">
                <span className="tooltip-label">❤️ Salud</span>
                <span className={`tooltip-value tooltip-health health-${animal.health.toLowerCase()}`}>
                    {animal.health.toUpperCase()}
                </span>
            </div>

            <hr className="tooltip-divider" />

            <div className="tooltip-user-section">
                <p className="place-badge">{config.label}</p>
                <p className="animal-name">{animal.name}</p>
                <p className="animal-type">{animal.typeAnimal}</p>
                <p className="animal-value1">{formattedValue}</p>
                <p className="owner-name">👤 {owner ? owner.name : '—'}</p>
            </div>
        </div>
    );

    return (
        <div className={`podium-column podium-place-${place}`}>
            <div
                className="animal-wrapper"
                onMouseEnter={() => setHovered(true)}
                onMouseLeave={() => setHovered(false)}
            >
                {svgImg ? (
                    <img
                        ref={imgRef}
                        src={svgImg}
                        alt={animal.name}
                        className="animal-svg animal-float"
                    />
                ) : (
                    <div className="animal-svg animal-float animal-placeholder">?</div>
                )}
            </div>

            {createPortal(tooltip, document.body)}
        </div>
    );
}