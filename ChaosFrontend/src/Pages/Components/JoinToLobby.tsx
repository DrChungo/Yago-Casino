import { useEffect, useState } from 'react';
import '../../styles/JoinToLobby.css';
import BackButton from './BackButton';
import LoadingHamster from './LoadingHamster';
import { useNavigate } from 'react-router-dom';
import pistols from '/Icons-Lobby/pistols-lobby.svg'

interface LobbyProperties {
    idLobby: string;
    lobbyCode: string;
    masterOfLobby: string;
    nameOfMaster: string;
    nameOfPlayers: string[];
    players: string[];
}

export default function JoinToLobby() {
    const Api_Url = import.meta.env.VITE_BASE_URL;
    const token = localStorage.getItem('token_casino');
    const [lobbies, setLobbies] = useState<LobbyProperties[] | null>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const navigate = useNavigate();

    const fetchAllLobbies = async () => {
        try {
            const response = await fetch(`${Api_Url}/api/Lobby/AllLobbies`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                setLobbies(data);
                setLoading(false);
            }
        } catch (error) {
            console.error('error fetching the lobbys', error);
        }
    };

   const fetchJoinLobby = async (lobbyCode: string) => {
    try {
        const response = await fetch(`${Api_Url}/api/Lobby/join`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'authorization': `Bearer ${token}`
            },
            body: JSON.stringify(lobbyCode)
        });

        if (response.ok) {
            const data = await response.json();
            console.log(data);
            
            navigate('/lobby-russian-roulette', { state: { lobby: data } });
        }
    } catch (error) {
        console.error("can't join to the lobby", error);
    }
};

    useEffect(() => {
        fetchAllLobbies();
    }, []);

    if (loading) return <main id="search"><LoadingHamster /></main>;

    return (
        <main id="search">
            <BackButton />

            {/* ── Título ── */}
            <h2 id="search-title">Available Lobbies</h2>

            {/* ── Grid de lobbies ── */}
            <div className="lobbies-grid">

                {lobbies?.length === 0 && (
                    <p className="no-lobbies">
                        No lobbies available — be the first to create one 🃏
                    </p>
                )}

                {lobbies?.map((lobby, index) => (
                    <div key={index} className="content-lobby">

                        {/* Imagen decorativa */}
                        <div className="lobby-bg-image">
                            <img src={pistols} alt="" aria-hidden="true" />
                        </div>

                        {/* Contenido */}
                        <div className="lobby-content">
                            <h3 className="lobby-master">{lobby?.nameOfMaster}</h3>
                            <hr className="lobby-divider" />
                            <p className="lobby-code">Code: <span>{lobby?.lobbyCode}</span></p>
                            <p className="lobby-players">Players: <span>{lobby?.players?.length ?? 0}</span></p>
                            <button
                                className="join-game"
                                onClick={() => fetchJoinLobby(lobby?.lobbyCode)}
                            >
                                Join
                            </button>
                        </div>

                    </div>
                ))}
            </div>
        </main>
    );
}