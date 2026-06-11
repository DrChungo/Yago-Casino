import React, { useState, useEffect } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAudio } from './hooks/useAudio';
import { useAuth } from './hooks/useAuth';
 
import fondoCasino from './assets/CasinoImages/casino_landing.png';
import fondoMenu from './assets/CasinoImages/casino_menu.png';
import fondoBar from './assets/CasinoImages/casino_bar_background.png';
import musicaIntro from './assets/Audios/casino_landing_soundtrack.mp3';
import sonidoHover from './assets/Audios/casino_coin_sound.m4a';
import logoCasino from './assets/CasinoImages/casino_logo.png';
 
import './App.css';
 
const App: React.FC = () => {
    const location = useLocation();
    const navigate = useNavigate();
   
    const [haComenzado, setHaComenzado] = useState(false);
    const [userName, setUserName] = useState('');
    const [cargandoDatos, setCargandoDatos] = useState(false);
    const [userWallet, setUserWallet] = useState(0);
    const [userAnimals, setUserAnimals] = useState<any[]>([]);
 
   
    const parseJwt = (token: string) => {
        try {
            return JSON.parse(atob(token.split('.')[1]));
        } catch {
            return null;
        }
    };
 
 
    const [musicaMutada, setMusicaMutada] = useState<boolean>(() => {
        return localStorage.getItem('casino_music_muted') === 'true';
    });
 
    const { reproducir, detenerTodo, silenciar } = useAudio();
 
    const {
        email, setEmail,
        password, setPassword,
        error, cargando, estaLogueado,
        realizarLogin, realizarLogout,
    } = useAuth(
        () => {
            navigate('/lobby');
        },
        () => {
            navigate('/');
            detenerTodo();
        }
    );
 
    // ─────────────────────────────────────────
    // Dashboard Data Fetcher
    // ─────────────────────────────────────────
    const Api_Url = import.meta.env.VITE_BASE_URL;
 
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
            const resUsers = await fetch(`${Api_Url}/api/User/AllUsers`, {
                headers: { Authorization: `Bearer ${token}` },
            });
           
            if (resUsers.ok) {
                const data = await resUsers.json();
                const matchedUser = data.users?.find((u: any) => u.id === userId);
                if (matchedUser) setUserWallet(matchedUser.wallet);
            }
 
            // Animals
            const resAnimals = await fetch(`${Api_Url}/api/Animal/All`, {
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
 
    // Auto-fetch + play music on login
    useEffect(() => {
        if (estaLogueado && location.pathname === '/lobby') {
            reproducir(musicaIntro, { loop: true, volume: 0.6 });
            silenciar(musicaIntro, musicaMutada);
        }
    }, [estaLogueado]);
 
    // ─────────────────────────────────────────
    // Handlers
    // ─────────────────────────────────────────
 
    /** Mute / unmute background music */
    const manejarToggleMute = (): void => {
        const nuevoMutado = !musicaMutada;
        setMusicaMutada(nuevoMutado);
 
        detenerTodo();                                          // Mata cualquier audio residual
        reproducir(musicaIntro, { loop: true, volume: 0.6 }); // Reinicia solo el lobby
        silenciar(musicaIntro, nuevoMutado);                   // Aplica el mute correcto
 
        localStorage.setItem('casino_music_muted', String(nuevoMutado));
    };
 
    /** Hover sound on the Start button */
    const manejarHoverBotonStart = (): void => {
        reproducir(sonidoHover, { volume: 0.4 });
    };
 
    /** Start screen → login screen transition */
    const manejarInicioCasino = (): void => {
        detenerTodo();
         navigate('/login');
    };
 
    /**
     * Game selection handler — preserved from File 1.
     * Extend the switch-case below as new game routes are added.
     */
    const manejarSeleccionJuego = (nombreJuego: string): void => {
        detenerTodo();
        switch (nombreJuego) {
            case 'SLOTS ROYALE':
                // Navigation handled by <Link> inside InteractiveMap
                break;
            case 'SHOP':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente podrás jugar aquí!`);
                break;
            case 'RUSSIAN ROULETTE':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente podrás jugar aquí!`);
                break;
            case 'HIGHER OR LOWER':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente podrás jugar aquí!`);
                break;
            case 'BLACKJACK':
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente podrás jugar aquí!`);
                break;
            default:
                alert(`Clickeaste en: ${nombreJuego}. ¡Próximamente podrás jugar aquí!`);
        }
    };
 
    const handleToggleMute = () => {
        const next = !musicaMutada;
        setMusicaMutada(next);
        silenciar(musicaIntro, next);
        localStorage.setItem('casino_music_muted', String(next));
    };
 
    const fondoActual = location.pathname === '/lobby' ? fondoMenu
        : location.pathname === '/bar' ? fondoBar
            : fondoCasino;
    const isOverlayActive = location.pathname === '/login' || location.pathname === '/register';
 
    return (
        <div
            className="app-main-layout"
            style={{ backgroundImage: `url(${fondoActual})` }}
        >
            <div className={`dark-overlay ${isOverlayActive ? 'active' : ''}`} />
 
            <div className="content-layer">
                <Outlet
                    context={{
                        logoCasino,
                        email,
                        setEmail,
                        password,
                        setPassword,
                        error,
                        cargando,
                        estaLogueado,
                        realizarLogin,
                        realizarLogout,
                        musicaMutada,
                        handleToggleMute,
                        handleStartPressed: manejarInicioCasino,
                        handleHoverStart: manejarHoverBotonStart,
 
                    }}
                />
            </div>
        </div>
    );
};
 
export default App;