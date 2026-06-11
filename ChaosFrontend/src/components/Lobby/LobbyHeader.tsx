import React, { useState, useRef, useEffect } from 'react';
import './LobbyHeader.css';

interface LobbyHeaderProps {
    logoCasino: string;
    onLogout: () => void;
    isMuted: boolean;
    onToggleMute: () => void;
    onNavigateToShop?: () => void;
    onNavigateToPen?: () => void;
    onNavigateToBar?: () => void;
    onNavigateToMusicRoom?: () => void;
}

export const LobbyHeader: React.FC<LobbyHeaderProps> = ({
    logoCasino,
    onLogout,
    isMuted,
    onToggleMute,
    onNavigateToShop,
    onNavigateToPen,
    onNavigateToBar,
    onNavigateToMusicRoom
}) => {
    const [menuOpen, setMenuOpen] = useState(false);
    const menuRef = useRef<HTMLDivElement>(null);

    // Close menu when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
                setMenuOpen(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleNavClick = (callback?: () => void) => {
        setMenuOpen(false);
        callback?.();
    };

    return (
        <header className="menu-header">
            <nav className="header-nav">
                <img src={logoCasino} className="logo-menu" alt="Logo Casino" />

                {/* Desktop nav buttons */}
                <div className="nav-desktop-buttons">
                    {onNavigateToShop && (
                        <button className="nav-item-btn" onClick={onNavigateToShop}>
                            PETS SHOP
                        </button>
                    )}
                    {onNavigateToPen && (
                        <button className="nav-item-btn" onClick={onNavigateToPen}>
                            HABITATS
                        </button>
                    )}
                    {onNavigateToBar && (
                        <button className="nav-item-btn" onClick={onNavigateToBar}>
                            BAR
                        </button>
                    )}
                    {onNavigateToMusicRoom && (
                        <button className="nav-item-btn" onClick={onNavigateToMusicRoom}>
                            MUSIC ROOM
                        </button>
                    )}
                </div>

                {/* Hamburger button — mobile only */}
                <div className="hamburger-wrapper" ref={menuRef}>
                    <button
                        className="nav-item-btn hamburger-btn"
                        onClick={() => setMenuOpen(prev => !prev)}
                        aria-label="Toggle navigation menu"
                        aria-expanded={menuOpen}
                    >
                        <span className={`hamburger-icon ${menuOpen ? 'open' : ''}`}>
                            <span />
                            <span />
                            <span />
                        </span>
                    </button>

                    {/* Dropdown menu */}
                    {menuOpen && (
                        <div className="mobile-dropdown">
                            {onNavigateToShop && (
                                <button
                                    className="nav-item-btn mobile-nav-btn"
                                    onClick={() => handleNavClick(onNavigateToShop)}
                                >
                                    PETS SHOP
                                </button>
                            )}
                            {onNavigateToPen && (
                                <button
                                    className="nav-item-btn mobile-nav-btn"
                                    onClick={() => handleNavClick(onNavigateToPen)}
                                >
                                    HABITATS
                                </button>
                            )}
                            {onNavigateToBar && (
                                <button
                                    className="nav-item-btn mobile-nav-btn"
                                    onClick={() => handleNavClick(onNavigateToBar)}
                                >
                                    BAR
                                </button>
                            )}
                            {onNavigateToMusicRoom && (
                                <button
                                    className="nav-item-btn mobile-nav-btn"
                                    onClick={() => handleNavClick(onNavigateToMusicRoom)}
                                >
                                    MUSIC ROOM
                                </button>
                            )}
                        </div>
                    )}
                </div>
            </nav>

            <div className="header-utils">
                <button
                    className={`boton-mute ${isMuted ? 'muted' : ''}`}
                    onClick={onToggleMute}
                    title={isMuted ? "Unmute Music" : "Mute Music"}
                    aria-label="Toggle music mute"
                >
                    {isMuted ? '🔇 MUTED' : '🔊 MUSIC'}
                </button>
                <button className="boton-logout" onClick={onLogout}>LOGOUT</button>
            </div>
        </header>
    );
};