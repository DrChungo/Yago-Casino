import { useParams, useNavigate } from 'react-router-dom';
import '../../styles/RussianGame.css';
import { useState, useRef, useEffect } from 'react';

interface GameState {
  roundNumber: number;
  playerId: string;
  playerName: string;
  wasBullet: boolean;
  isBot: boolean;
  message: string;
  gameFinished: boolean;
  winnerId: string;
  winnername: string;
  prizePool: number;
}

export default function RussianRoulette() {
  const [gameState, setGameState] = useState<GameState | null>(null);
  const [isShooting, setIsShooting] = useState(false);

  const { lobbyId } = useParams();
  const navigate = useNavigate();

  const token = localStorage.getItem('token_casino');
  const Api_URL = import.meta.env.VITE_BASE_URL;

  /* ── ID del jugador actual desde el JWT ──────────────── */
  const getCurrentPlayerId = (): string | null => {
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.sub ?? payload.id ?? payload.userId ?? null;
    } catch {
      return null;
    }
  };

  /* ── Cursor disparo ──────────────────────────────────── */
  const triggerShootCursor = () => {
    setIsShooting(true);
    setTimeout(() => setIsShooting(false), 600);
  };

  const animationFinishedRef = useRef(false);

  /* ── Efecto para reproducir toda la partida automáticamente ──────── */
  useEffect(() => {
    let mounted = true;

    const runGameAnimation = async () => {
      try {
        const [statusRes, historyRes] = await Promise.all([
          fetch(`${Api_URL}/api/RussianRouletteControllerr/Status/${lobbyId}`),
          fetch(`${Api_URL}/api/RussianRouletteControllerr/history/${lobbyId}`)
        ]);

        if (!statusRes.ok || !historyRes.ok) {
          console.error("Failed to fetch status or history", statusRes.status, historyRes.status);
          return;
        }

        const statusData = await statusRes.json();
        const historyData = await historyRes.json();

        const playersMap = new Map();
        statusData.players.forEach((p: any) => playersMap.set(p.playerId, p.name));

        for (let i = 0; i < historyData.length; i++) {
          if (!mounted) return;
          const round = historyData[i];
          const playerName = playersMap.get(round.playerId) || "Unknown";

          setGameState({
            roundNumber: round.roundNumber,
            playerId: round.playerId,
            playerName: playerName,
            wasBullet: round.wasBullet,
            isBot: false,
            message: round.wasBullet ? `💀 BANG! ${playerName} was shot!` : `🔫 Click! ${playerName} survived this round!`,
            gameFinished: false,
            winnerId: "",
            winnername: "",
            prizePool: 0
          });

          if (round.wasBullet) triggerShootCursor();

          await new Promise(r => setTimeout(r, 2000));
        }

        if (!mounted) return;

        // Final de la partida
        setGameState({
          roundNumber: historyData.length,
          playerId: "",
          playerName: "",
          wasBullet: false,
          isBot: false,
          message: statusData.winnerId ? `🏆 ${statusData.winnerName} wins ${statusData.currentPrizePool}!` : `🤖 No human survivors! Bot wins.`,
          gameFinished: true,
          winnerId: statusData.winnerId,
          winnername: statusData.winnerName,
          prizePool: statusData.currentPrizePool
        });

        animationFinishedRef.current = true;

        const currentPlayerId = getCurrentPlayerId();
        const playerWon = currentPlayerId && currentPlayerId === statusData.winnerId;

        if (playerWon) {
          setTimeout(() => navigate('/'), 3000);
        } else {
          setTimeout(() => {
            localStorage.removeItem('token_casino');
            navigate('/login');
          }, 3000);
        }

      } catch (error) {
        console.error("Error running animation:", error);
      }
    };

    runGameAnimation();

    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (!animationFinishedRef.current) {
        e.preventDefault();
        e.returnValue = "If you leave now, you will lose everything! Are you sure?";
        return e.returnValue;
      }
    };

    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => {
      mounted = false;
      window.removeEventListener("beforeunload", handleBeforeUnload);

      // Si desmonta el componente antes de terminar la animación, es un abandono.
      if (!animationFinishedRef.current) {
        fetch(`${Api_URL}/api/RussianRouletteControllerr/Abandon/${lobbyId}`, {
          method: 'POST',
          headers: { authorization: `Bearer ${token}` }
        }).catch(e => console.error("Abandon fetch failed", e));
      }
    };
  }, [lobbyId, token, Api_URL, navigate]);

  /* ── Clases dinámicas ────────────────────────────────── */
  const gameClass = [
    gameState?.gameFinished
      ? 'state--winner'
      : gameState?.wasBullet
        ? 'state--danger'
        : gameState
          ? 'state--safe'
          : '',
    isShooting ? 'is-shooting' : '',
  ]
    .filter(Boolean)
    .join(' ');

  const msgClass = gameState?.gameFinished
    ? 'game-message game-message--winner'
    : gameState?.wasBullet
      ? 'game-message game-message--danger'
      : 'game-message game-message--safe';

  const currentPlayerId = getCurrentPlayerId();
  const playerWon =
    gameState?.gameFinished && currentPlayerId === gameState.winnerId;
  const playerLost =
    gameState?.gameFinished && currentPlayerId !== gameState.winnerId;

  return (
    <main id="game" className={gameClass}>

      {/* ── Panel central ────────────────────────────────── */}
      <section className="game_state">

        <span className="roulette-crown">👑</span>
        <h2>Russian Roulette</h2>
        <div className="roulette-divider" />

        <p>
          <span className="label">Round</span>
          <span className="value">{gameState?.roundNumber ?? '—'}</span>
        </p>

        <p>
          <span className="label">Player</span>
          <span className="value">{gameState?.playerName ?? '—'}</span>
        </p>

        {gameState?.message && (
          <p className={msgClass}>{gameState.message}</p>
        )}

        {gameState?.gameFinished && (
          <>
            <p>
              <span className="label">🏆 Winner</span>
              <span className="value">{gameState.winnername}</span>
            </p>
            <p className="game-prize">
              Prize Pool: ${gameState.prizePool.toLocaleString()}
            </p>
          </>
        )}

        {playerWon && (
          <p className="game-message game-message--winner winner-msg">
            🎉 You won! Returning to home in 3s...
          </p>
        )}

        {playerLost && (
          <p className="game-message game-message--danger loser-msg">
            💀 You lost! Logging out in 3s...
          </p>
        )}

      </section>

      {/* ── Aviso de abandono ─────────────────────────────── */}
      {!gameState?.gameFinished && (
        <div className="game-message game-message--danger">
          ⚠️ DO NOT LEAVE! If you leave the game now, your account will be deactivated and you will lose your wallet!
        </div>
      )}

    </main>
  );
}