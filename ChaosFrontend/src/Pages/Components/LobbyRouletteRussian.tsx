import { useEffect, useState, useRef } from "react";
import '../../styles/RussianRoulette.css'
import BackButton from "./BackButton";
import { useNavigate, useLocation } from "react-router-dom"; // ← añadir useLocation
import LoadingHamster from "./LoadingHamster";
import copiedSvg from "../../assets/Copy.svg"

interface Lobby {
    idLobby: string;
    masterOfLobby: string;
    nameOfMaster: string;
    lobbyCode: string;
    players: string[];
    nameOfPlayers: string[];
}

export default function LobbyRouletteRussian() {
    const [lobby, setLobby] = useState<Lobby | null>(null);
    const Api_URL = import.meta.env.VITE_BASE_URL;
    const [copied, setCopied] = useState(false);
    const navigate = useNavigate();
    const location = useLocation(); 
    const token = localStorage.getItem('token_casino');
    const [loading, setLoading] = useState(true);
    const isStarting = useRef(false);

    const decodeToken = (token: string) => {
        const base64Payload = token.split('.')[1];
        const payload = atob(base64Payload);
        return JSON.parse(payload);
    }

    const decoded = decodeToken(token);
    const currentUserName = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    const isMaster = currentUserName === lobby?.nameOfMaster;

    const handleLeaveLobby = () => {
        if (isStarting.current || !lobby?.idLobby) return;
        
        fetch(`${Api_URL}/api/Lobby/leave?lobbyId=${lobby.idLobby}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'authorization': `Bearer ${token}`
            },
            keepalive: true
        }).catch(err => console.error("Error leaving lobby:", err));
    };

    // ── Fetch Start Game ──────────────────────────────────────
    const fetchStartGame = async () => {
        try {
            isStarting.current = true;
            const response = await fetch(`${Api_URL}/api/RussianRouletteControllerr/Start/${lobby.idLobby}`, {
                method: 'POST',
                headers: {
                    'content-Type': 'application/json',
                    'authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                console.log(data);
                navigate(`/russian-roulette/game/${lobby.idLobby}`);
            } else {
                isStarting.current = false;
            }
        } catch (error) {
            console.error("Error starting game:", error);
            isStarting.current = false;
        }
    }

    // ── Kick Player ───────────────────────────────────────────
    const kickPlayer = async (playerId: string, playerName: string) => {
        if (playerName === lobby?.nameOfMaster) return;

        try {
            const response = await fetch(`${Api_URL}/api/Lobby/Kick/Player`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    idLobby: lobby.idLobby,
                    idPlayer: playerId
                })
            });

            if (response.ok) {
                const data = await response.json();
                console.log(data);
                if (playerId == currentUserName) {
                    isStarting.current = true;
                    navigate("/lobby");
                }
            }
        } catch (error) {
            console.error("Error kicking player:", error);
        }
    }

    // ── Delete Lobby ──────────────────────────────────────────
    const fetchDeleteLobby = async () => {
        try {
            isStarting.current = true;
            const response = await fetch(`${Api_URL}/api/Lobby/Remove`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'authorization': `Bearer ${token}`
                },
                body: JSON.stringify(lobby.idLobby)
            });
            if (response.ok) { navigate("/lobby"); }
            else { isStarting.current = false; }
        } catch (error) {
            console.error("Error deleting lobby:", error);
            isStarting.current = false;
            navigate("/create-or-join-russian-roulette");
        }
    }

    // ── Create Lobby ──────────────────────────────────────────
    const fetchCreateLobby = async () => {
        try {
            const response = await fetch(`${Api_URL}/api/Lobby/create`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setLobby(data);
                setLoading(false);
            }
        } catch (error) {
            console.error("Error creating lobby:", error);
        }
    }

    // ── Efecto inicial: crear o cargar lobby ──────────────────
    useEffect(() => {
        // Si venimos de JoinToLobby, el lobby ya viene en el state
        const lobbyFromJoin = location.state?.lobby as Lobby | undefined;

        if (lobbyFromJoin) {
            // El usuario se unió: usamos el lobby recibido directamente
            setLobby(lobbyFromJoin);
            setLoading(false);
        } else {
            // El usuario está creando: creamos el lobby normalmente
            fetchCreateLobby();
        }
    }, []);

    // ── Polling cada 3 segundos ───────────────────────────────
    useEffect(() => {
        console.log(lobby +"texto de prueba para ver que el lobby es el mismo")
        if (!lobby?.idLobby) return;

        const interval = setInterval(async () => {
            const response = await fetch(`${Api_URL}/api/Lobby/select/${lobby.idLobby}`, {
                headers: { 
                    
                    'authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const data = await response.json();

                setLobby(data);
                console.log("data del servidor"+ data.id)
                console.log("Lobby"+lobby.idLobby)

                if (data.status === "InProgress") {
                    isStarting.current = true;
                    navigate(`/russian-roulette/game/${lobby.idLobby}`);
                    return;
                }

                const isPlayerInLobby = data.nameOfPlayers?.includes(currentUserName);
                if (!isPlayerInLobby) {
                    isStarting.current = true;
                    navigate('/lobby');
                }
            }
        }, 3000);

        return () => clearInterval(interval);
    }, [lobby?.idLobby]);

    // ── Cleanup: leave lobby when navigating away or closing page ──────────────────
    useEffect(() => {
        if (!lobby?.idLobby) return;

        const handleUnload = () => {
            handleLeaveLobby();
        };

        window.addEventListener("beforeunload", handleUnload);
        window.addEventListener("pagehide", handleUnload);

        return () => {
            handleLeaveLobby();
            window.removeEventListener("beforeunload", handleUnload);
            window.removeEventListener("pagehide", handleUnload);
        };
    }, [lobby?.idLobby]);

    // ── Copy code ─────────────────────────────────────────────
    const handleCopy = () => {
        navigator.clipboard.writeText(lobby?.lobbyCode);
        setCopied(true);
        setTimeout(() => setCopied(false), 3000);
    };

    if (loading) return <main id="lobby"><LoadingHamster /></main>;

    return (
        <>
            <BackButton />
            <main id="lobby">
                <table id="dashboard_players">
                    <thead>
                        <tr>
                            {isMaster
                                ? (
                                    <th id="delete" onClick={() => fetchDeleteLobby()}>
                                        » Master: {lobby?.nameOfMaster}
                                    </th>
                                ) : (
                                    <th>
                                        » Master: {lobby?.nameOfMaster}
                                    </th>
                                )
                            }
                            <th
                                id="code_copy"
                                style={{ cursor: "pointer" }}
                                className={copied ? "copied" : ""}
                                onClick={handleCopy}
                            >
                                Code: {lobby?.lobbyCode}
                                <img src={copiedSvg} alt="Copy" />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {lobby?.nameOfPlayers
                            .reduce((rows: { name: string; id: string }[][], player, index) => {
                                if (index % 2 === 0) {
                                    rows.push([{ name: player, id: lobby.players[index] }]);
                                } else {
                                    rows[rows.length - 1].push({ name: player, id: lobby.players[index] });
                                }
                                return rows;
                            }, [])
                            .map((pair, rowIndex) => (
                                <tr key={rowIndex}>
                                    <td
                                        className={
                                            pair[0]?.name === lobby?.nameOfMaster
                                                ? "player-cell master-cell"
                                                : isMaster
                                                    ? "player-cell kickable-cell"
                                                    : "player-cell"
                                        }
                                        onClick={() =>
                                            isMaster && pair[0] && kickPlayer(pair[0].id, pair[0].name)
                                        }
                                    >
                                        {pair[0]?.name ?? ""}
                                    </td>
                                    <td
                                        className={
                                            pair[1]?.name === lobby?.nameOfMaster
                                                ? "player-cell master-cell"
                                                : isMaster
                                                    ? "player-cell kickable-cell"
                                                    : "player-cell"
                                        }
                                        onClick={() =>
                                            isMaster && pair[1] && kickPlayer(pair[1].id, pair[1].name)
                                        }
                                    >
                                        {pair[1]?.name ?? ""}
                                    </td>
                                </tr>
                            ))}
                    </tbody>
                </table>

                {isMaster && (
                    <button id="start_button" onClick={fetchStartGame}>
                        Start Game
                    </button>
                )}
            </main>
        </>
    );
}