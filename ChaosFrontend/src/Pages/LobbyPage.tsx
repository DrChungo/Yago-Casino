import React, { useState, useEffect } from 'react';
import { useNavigate, useOutletContext } from 'react-router-dom';
import { LobbyHeader } from '../components/Lobby/LobbyHeader';
import { InteractiveMap } from '../components/Lobby/InteractiveMap';

// ─────────────────────────────────────────────
// Helper: JWT Decoder
// ─────────────────────────────────────────────
const parseJwt = (token: string) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
};

// ─────────────────────────────────────────────
// Helper: Animal Health Mapper
// ─────────────────────────────────────────────
const mapHealth = (healthVal: any) => {
    switch (healthVal) {
        case 1: case 'EXCELENT': case 'Excellent': return { label: 'Excellent', color: '#10b981' };
        case 2: case 'HEALTHY': case 'Healthy': return { label: 'Healthy', color: '#10b981' };
        case 3: case 'RECOVERING': case 'Recovering': return { label: 'Recovering', color: '#3b82f6' };
        case 4: case 'SICK': case 'Sick': return { label: 'Sick', color: '#f59e0b' };
        case 5: case 'CRITICAL': case 'Critical': return { label: 'Critical', color: '#ef4444' };
        case 0: case 'YAGUETE': case 'Yaguete': return { label: 'Yaguete', color: '#8b5cf6' };
        default: return { label: String(healthVal), color: '#6b7280' };
    }
};

// ─────────────────────────────────────────────
// Component
// ─────────────────────────────────────────────
const LobbyPage: React.FC = () => {
    const navigate = useNavigate();
    const {
        logoCasino,
        musicaMutada: isMuted,
        handleToggleMute: onToggleMute,
        realizarLogout: onLogout,
    } = useOutletContext<any>();
    const [isGameSelectorOpen, setIsGameSelectorOpen] = useState(false);
    const [userWallet, setUserWallet] = useState(0);
    const [userAnimals, setUserAnimals] = useState<any[]>([]);
    const [userName, setUserName] = useState('Player');
    const [cargandoDatos, setCargandoDatos] = useState(false);

    const totalAnimalValue = userAnimals.reduce((sum, animal) => sum + (animal.value ?? 0), 0);

    useEffect(() => {
        fetchDashboardData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    // ─────────────────────────────────────────
    // Data Fetcher
    // ─────────────────────────────────────────
    const fetchDashboardData = async () => {
        const token = localStorage.getItem('token_casino');
        if (!token) return;

        const payload = parseJwt(token);
        if (!payload) return;

        const userId =
            payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
            payload.nameid ||
            payload.sub;

        const name =
            payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
            payload.unique_name ||
            payload.name ||
            'Player';

        setUserName(name);
        if (!userId) return;

        setCargandoDatos(true);
        try {
            // Wallet
            const resUsers = await fetch('https://localhost:7101/api/User/AllUsers', {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (resUsers.ok) {
                const data = await resUsers.json();
                const matchedUser = data.users?.find((u: any) => u.id === userId);
                if (matchedUser) setUserWallet(matchedUser.wallet);
            }

            // Animals
            const resAnimals = await fetch('https://localhost:7101/api/Animal/All', {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (resAnimals.ok) {
                const data = await resAnimals.json();
                const list = data.animalList || data;
                if (Array.isArray(list)) {
                    setUserAnimals(list.filter((a: any) => a.ownerId === userId));
                }
            }
        } catch (err) {
            console.error('Error fetching dashboard data:', err);
        } finally {
            setCargandoDatos(false);
        }
    };

    // ─────────────────────────────────────────
    // Game Selection Handler
    // ─────────────────────────────────────────
    const manejarSeleccionJuego = (nombreJuego: string): void => {
        switch (nombreJuego) {
            case 'SLOTS ROYALE':
                // Navigation handled by <Link> inside InteractiveMap
                break;
            case 'SHOP':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente!`);
                break;
            case 'RUSSIAN ROULETTE':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente!`);
                break;
            case 'HIGHER OR LOWER':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente!`);
                break;
            case 'BLACKJACK':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente!`);
                break;
            default:
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente!`);
        }
    };

    // ─────────────────────────────────────────
    // Render
    // ─────────────────────────────────────────
    return (
        <div className="menu-container-royale">

            {/* Header */}
    <LobbyHeader
     logoCasino={logoCasino}
     onLogout={onLogout}
     isMuted={isMuted}
     onToggleMute={onToggleMute}
     onNavigateToPen={() => navigate('/farm')}
     onNavigateToShop={() => navigate('/shop')}
     onNavigateToBar={() => navigate('/bar')}
     onNavigateToMusicRoom={() => navigate('/music-room')}
/>
            {/* Three-column dashboard */}
            <div className="lobby-dashboard-layout">

                {/* LEFT — My Corral */}
                <aside className="lobby-panel-left">
                    <h3 className="panel-title">🦁 MY CORRAL</h3>
                    <div className="animals-list-wrapper">
                        {cargandoDatos ? (
                            <p className="loading-text">Loading corral...</p>
                        ) : userAnimals.length === 0 ? (
                            <p className="no-animals-text">
                                Your corral is empty. Buy pets in the Shop!
                            </p>
                        ) : (
                            userAnimals.map((animal) => (
                                <div key={animal.id} className="animal-card-mini">
                                    <div className="animal-card-header">
                                        <span className="animal-name">
                                            {animal.name || 'Unnamed Pet'}
                                        </span>
                                        <span
                                            className="animal-health-badge"
                                            style={{
                                                backgroundColor: mapHealth(animal.health).color,
                                            }}
                                        >
                                            {mapHealth(animal.health).label}
                                        </span>
                                    </div>
                                    <div className="animal-card-body">
                                        <span>Type: <strong>{animal.typeAnimal}</strong></span>
                                        <span>Age: <strong>{animal.age} y/o</strong></span>
                                        <span>
                                            Est. Value:{' '}
                                            <strong className="gold-text">${animal.value}</strong>
                                        </span>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </aside>

                {/* CENTER — Welcome Billboard */}
                <main className="lobby-panel-center">
                    <div className="casino-welcome-billboard">
                        <p className="las-vegas-subtitle">
                            WELCOME, {userName.toUpperCase()}! TRY YOUR LUCK AT THE ROYAL TABLES
                        </p>
                        <div className="las-vegas-chips-deco" />
                    </div>
                </main>

                {/* RIGHT — Player Wallet */}
                <aside className="lobby-panel-right">
                    <h3 className="panel-title">💰 PLAYER WALLET</h3>
                    <div className="wallet-card">
                        <div className="wallet-chip-icon">🪙</div>
                        <span className="wallet-label">WALLET BALANCE</span>
                        {cargandoDatos ? (
                            <span className="wallet-amount loading">...</span>
                        ) : (
                            <span className="wallet-amount">
                                ${userWallet.toLocaleString()}
                            </span>
                        )}
                    </div>
                    <div className="wallet-card">
                        <div className="wallet-chip-icon">🤑</div>
                        <span className="wallet-label">TOTAL ANIMAL VALUE</span>
                        {cargandoDatos ? (
                            <span className="wallet-amount loading">...</span>
                        ) : (
                                <span className="wallet-amount">
                                    ${totalAnimalValue.toLocaleString()}
                            </span>
                        )}
                    </div>
                </aside>
            </div>

            {/* Game Selector Overlay */}
            {isGameSelectorOpen && (
                <div className="games-overlay-modal animate-fade-in">
                    <div className="games-modal-content">
                        <h3 className="games-modal-title">🎰 SELECT A CASINO GAME</h3>
                        <InteractiveMap onSeleccionarJuego={manejarSeleccionJuego} />
                    </div>
                </div>
            )}

            {/* Bottom Bar */}
            <div className="lobby-bottom-bar">
                <button
                    className={`btn-play-games ${isGameSelectorOpen ? 'active' : ''}`}
                    onClick={() => setIsGameSelectorOpen(prev => !prev)}
                >
                    {isGameSelectorOpen ? '❌ CLOSE SELECTION' : 'PLAY GAMES'}
                </button>
            </div>

        </div>
    );
};

export default LobbyPage;