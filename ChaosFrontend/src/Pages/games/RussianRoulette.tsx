import { useParams, useNavigate } from 'react-router-dom';
import '../../styles/RussianGame.css';
import { useState, useRef } from 'react';

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
  const [isRunning, setIsRunning] = useState(false);
  const [isShooting, setIsShooting] = useState(false);

  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);
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

  /* ── Al terminar ─────────────────────────────────────── */
  const handleGameFinished = (data: GameState) => {
    const currentPlayerId = getCurrentPlayerId();
    const playerWon = currentPlayerId === data.winnerId;

    if (playerWon) {
      setTimeout(() => navigate('/'), 3000);
    } else {
      setTimeout(() => {
        localStorage.removeItem('token_casino');
        navigate('/login');
      }, 3000);
    }
  };

  /* ── Play round ──────────────────────────────────────── */
  const fetchPlayRound = async (): Promise<GameState | null> => {
    try {
      const response = await fetch(
        `${Api_URL}/api/RussianRouletteControllerr/PlayRound/${lobbyId}`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            authorization: `Bearer ${token}`,
          },
        }
      );
      if (response.ok) {
        const data: GameState = await response.json();
        setGameState(data);
        if (data.wasBullet) triggerShootCursor();
        return data;
      }
    } catch (error) {
      console.error('Error playing round:', error);
    }
    return null;
  };

  /* ── Start game ──────────────────────────────────────── */
  const startGame = async () => {
    if (isRunning) return;
    setIsRunning(true);

    const firstData = await fetchPlayRound();
    if (firstData?.gameFinished) {
      setIsRunning(false);
      handleGameFinished(firstData);
      return;
    }

    intervalRef.current = setInterval(async () => {
      const data = await fetchPlayRound();
      if (data?.gameFinished) {
        clearInterval(intervalRef.current!);
        intervalRef.current = null;
        setIsRunning(false);
        handleGameFinished(data);
      }
    }, 2000);
  };

  /* ── Stop manual ─────────────────────────────────────── */
  const stopGame = () => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setIsRunning(false);
  };

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

      {/* ── Botón Start/Stop ─────────────────────────────── */}
      {!gameState?.gameFinished && (
        <button
          id="play_round"
          onClick={isRunning ? stopGame : startGame}
          className={isRunning ? 'btn--running' : ''}
        >
          {isRunning ? '⏹ Stop' : '🎯 Start Game'}
        </button>
      )}

    </main>
  );
}