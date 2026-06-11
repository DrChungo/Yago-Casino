import React, { useRef, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './InteractiveMap.css';
import svgCoinflip from '../../assets/CasinoImages/casino_coin_flip_icon.png';
import svgSlots from '../../assets/CasinoImages/casino_tragapeiras_icon.png';
import svgRoulette from '../../assets/CasinoImages/casino_european_roulette_icon.png';
import svgRussian from '../../assets/CasinoImages/casino_russian_roulette_icon.png';
import svgHigherLower from '../../assets/CasinoImages/casino_higher_lower_icon.png';
import svgBlackjack from '../../assets/CasinoImages/casino_blackjack_icon.png';

interface InteractiveMapProps {
    onSeleccionarJuego: (nombreJuego: string) => void;
}

export const InteractiveMap: React.FC<InteractiveMapProps> = ({ onSeleccionarJuego }) => {
    const navigate = useNavigate();
    const scrollRef = useRef<HTMLDivElement>(null);
    const [showArrow, setShowArrow] = useState(true);

    const listaJuegos = [
        { id: 'coinflip', nombre: 'COINFLIP', src: svgCoinflip, ruta: '/coinflip', noBuffs: false },
        { id: 'slots', nombre: 'SLOTS ROYALE', src: svgSlots, ruta: '/yagomachine', noBuffs: true },
        { id: 'roulette', nombre: 'EUROPEAN ROULETTE', src: svgRoulette, ruta: '/european-roulette', noBuffs: true },
        { id: 'blackjack', nombre: 'BLACKJACK', src: svgBlackjack, ruta: '/blackjack', noBuffs: false },
        { id: 'russian', nombre: 'RUSSIAN ROULETTE', src: svgRussian, ruta: '/create-or-join-russian-roulette', noBuffs: true },
        { id: 'higher-lower', nombre: 'HIGHER OR LOWER', src: svgHigherLower, ruta: '/higherorlower', noBuffs: false },
    ];

    // ─────────────────────────────────────────────
    // Click handler — navigate if route exists,
    // otherwise delegate to parent via callback
    // ─────────────────────────────────────────────
    const handleGameClick = (nombre: string, ruta: string | null): void => {
        if (ruta) {
            navigate(ruta);
        } else {
            onSeleccionarJuego(nombre);
        }
    };

    // Scroll arrow visibility logic
    useEffect(() => {
        const el = scrollRef.current;
        if (!el) return;

        const checkScroll = () => {
            const atBottom = el.scrollTop + el.clientHeight >= el.scrollHeight - 8;
            const hasOverflow = el.scrollHeight > el.clientHeight;
            setShowArrow(hasOverflow && !atBottom);
        };

        checkScroll();
        el.addEventListener('scroll', checkScroll);

        const resizeObserver = new ResizeObserver(checkScroll);
        resizeObserver.observe(el);

        return () => {
            el.removeEventListener('scroll', checkScroll);
            resizeObserver.disconnect();
        };
    }, []);

    return (
        <div className="interactive-map-wrapper">
            <div className="interactive-svg-showcase" ref={scrollRef}>
                <div className="games-flexible-layout">
                    {listaJuegos.map((juego) => (
                        <div
                            key={juego.id}
                            className={`game-card-showcase item-${juego.id} ${!juego.ruta ? 'coming-soon' : ''}`}
                            onClick={() => handleGameClick(juego.nombre, juego.ruta)}
                            title={!juego.ruta ? `${juego.nombre} — Coming Soon!` : juego.nombre}
                        >
                            <div className="game-interactive-box">
                                <img
                                    src={juego.src}
                                    alt={juego.nombre}
                                    className="svg-game-asset"
                                />

                                {/* ── No Buffs Overlay Badge ───────────────────── */}
                                {juego.noBuffs && (
                                    <div className="no-buffs-overlay" title="Bar drink effects are disabled in this game">
                                        <span className="no-buffs-icon">🚫🍹</span>
                                        <span className="no-buffs-text">NO BAR BUFFS</span>
                                    </div>
                                )}
                                {/* ─────────────────────────────────────────────── */}

                            </div>
                            <span className="game-label-card">
                                {juego.nombre}
                                {/* Small badge for unfinished games */}
                                {!juego.ruta && (
                                    <span className="coming-soon-badge"> 🔒</span>
                                )}
                            </span>
                        </div>
                    ))}
                </div>
            </div>

            {/* Scroll indicator — hides when reaching the bottom */}
            <div
                className={`scroll-down-indicator ${showArrow ? 'scroll-indicator--visible' : 'scroll-indicator--hidden'}`}
                aria-hidden="true"
            >
                <div className="scroll-indicator-inner">
                    <div className="scroll-chevrons">
                        <span className="chevron">&#8595;</span>
                    </div>
                </div>
            </div>
        </div>
    );
};