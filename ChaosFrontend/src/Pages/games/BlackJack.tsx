import AnimalToBetInGame from '../../components/AnimalToBetInGame';
import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import '../../styles/BlackJack.css';
import LoadingHamster from '../../Pages/Components/LoadingHamster';
import ActiveDrinkEffects from '../Components/ActiveDrinkEffect';

import { useAudio } from '../../hooks/useAudio'; 
import musicaGame from '../../assets/Audios/casino_game_4.mp3';
import musicaIntro from '../../assets/Audios/casino_landing_soundtrack.mp3';

interface CardDto { 
  value: number;
}

const AnimalType = {
  FLY: 'FLY',
  HAMSTER: 'HAMSTER',
  CAT: 'CAT',
  DOG: 'DOG',
  SHEEP: 'SHEEP',
  COW: 'COW',
  HORSE: 'HORSE',
  CROCODILE: 'CROCODILE',
  SHARK: 'SHARK',
  WHALE: 'WHALE',
  LOVEBIRD: 'LOVEBIRD',
  TURTLE: 'TURTLE MARINE',
  T_REX: 'T REX',
  PHOENIX: 'PHOENIX',
  PTERODACTYLUS: 'PTERODACTYLUS'
} as const;

type AnimalType = typeof AnimalType[keyof typeof AnimalType];

interface Animal {
  id: string;
  name: string;
  typeAnimal: AnimalType;
  age: number;
  weight: number;
  health: string;
  height: number;
  ownerId: string;
  value: number;
  isAvailable: boolean;
  rarity: boolean;
}

interface ApiResponse {
  success: boolean;
  message: string;
  data: Animal[] | null;
}

interface BlackJackRequest {
  animalId: string;
}

interface BlackJackResponse {
  gameId: string;
  animalName: string;
  reward: number;
  message: string;
  result: number; // enum: null=en juego, 0=WIN, 1=LOSE, 2=DRAW
  isFinished: boolean; 
  playerScore: number;
  dealerScore: number;
  dealerCards: CardDto[];  
  userCards: CardDto[]; 
}

// Enum resultado (debe coincidir con tu backend)
const GameResult = {
  WIN: 0,
  LOSE: 1,
  DRAW: 2,
} as const;

type GamePhase = 'SELECT_ANIMAL' | 'PLAYING' | 'FINISHED';

function BlackJack() {
  const navigate = useNavigate();
  const { reproducir, detener, silenciar } = useAudio();
  const [musicaMutada, setMusicaMutada] = useState<boolean>(
    () => localStorage.getItem('casino_music_muted') === 'true'
  );

  const [animals, setAnimals] = useState<Animal[]>([]);
  const [selectedAnimal, setSelectedAnimal] = useState<Animal | null>(null);
  const [isLoadingAnimals, setIsLoadingAnimals] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Estado del juego
  const [gamePhase, setGamePhase] = useState<GamePhase>('SELECT_ANIMAL');
  const [gameData, setGameData] = useState<BlackJackResponse | null>(null);
  const [isActionLoading, setIsActionLoading] = useState(false);
  const [showResult, setShowResult] = useState(false);
  const [playerCards, setPlayerCards] = useState<CardDto[]>([]);
  const [lastCardAnimation, setLastCardAnimation] = useState(false);

  useEffect(() => {
    fetchUserAnimals();
  }, []);

  useEffect(() => {
      detener(musicaIntro);
      reproducir(musicaGame, { loop: true, volume: 0.5 });
      silenciar(musicaGame, localStorage.getItem('casino_music_muted') === 'true');

      return () => {
          detener(musicaGame);                                        // Para el juego
          reproducir(musicaIntro, { loop: true, volume: 0.6 });      // Reanuda lobby
          silenciar(musicaIntro, localStorage.getItem('casino_music_muted') === 'true'); // Respeta mute
      };
  }, []);

  // ─────────────────────────────────────────
  // FETCH ANIMALES
  // ─────────────────────────────────────────
  const fetchUserAnimals = async () => {
    try {
      setIsLoadingAnimals(true);
      setError(null);

      const token = localStorage.getItem('token_casino');

      if (!token) {
        setError('No estás autenticado. Redirigiendo...');
        setTimeout(() => navigate('/'), 2000);
        return;
      }

      const [response] = await Promise.all([
      fetch('https://localhost:7101/api/Animal/GetAnimalByOwnerId', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      }),
      new Promise(resolve => setTimeout(resolve, 800))
    ]);

      if (response.status === 401) {
        setError('Tu sesión ha expirado. Por favor inicia sesión nuevamente.');
        localStorage.removeItem('token_casino');
        setTimeout(() => navigate('/'), 2000);
        return;
      }

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Error ${response.status}`);
      }

      const apiResponse: ApiResponse = await response.json();

      if (!apiResponse.success || !apiResponse.data) {
        setError(apiResponse.message || 'No tienes animales disponibles');
        setAnimals([]);
        return;
      }

      const availableAnimals = apiResponse.data.filter(a => a.isAvailable);
      setAnimals(availableAnimals);

      if (availableAnimals.length === 0) {
        setError('No tienes animales disponibles para apostar. ¡Compra algunos en la tienda!');
      }

    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error al cargar los animales');
    } finally {
      setIsLoadingAnimals(false);
    }
  };

  const cardLabelCache = useRef<Map<string, string>>(new Map());

const formatCardValue = (value: number, cardIndex: number, owner: 'player' | 'dealer'): string => {
  if (value === 11 || value === 1) return 'A';

  if (value === 10) {
    // Clave única por carta: quién la tiene + su posición
    const cacheKey = `${owner}-${cardIndex}`;

    if (!cardLabelCache.current.has(cacheKey)) {
      const faceCards = ['J', 'Q', 'K'];
      const label = faceCards[Math.floor(Math.random() * faceCards.length)];
      cardLabelCache.current.set(cacheKey, label);
    }

    return cardLabelCache.current.get(cacheKey)!;
  }

  return value.toString();
};

  // ─────────────────────────────────────────
  // INICIAR PARTIDA
  // ─────────────────────────────────────────
    const startGame = async () => {
    if (!selectedAnimal) return;

    try {
      setIsActionLoading(true);
      setError(null);

      const token = localStorage.getItem('token_casino');
      if (!token) {
        setError('No estás autenticado.');
        return;
      }

      const request: BlackJackRequest = { animalId: selectedAnimal.id };

      const response = await fetch('https://localhost:7101/api/CasinoGames/blackjack/start', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Error ${response.status}`);
      }

      const result: BlackJackResponse = await response.json();
      console.log('🎮 Game response:', result);

      setGameData(result);

      if (result.userCards && result.userCards.length > 0) {
        console.log('🃏 Player cards:', result.userCards); 
        setPlayerCards(result.userCards);
      } else {
        console.warn('⚠️ No user cards received!'); 
      }

      if (result.isFinished || result.result !== null) {
        setGamePhase('FINISHED');
        setShowResult(true);
      } else {
        setGamePhase('PLAYING');
      }

    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error al iniciar el juego');
    } finally {
      setIsActionLoading(false);
    }
  };

  // ─────────────────────────────────────────
  // HIT - PEDIR CARTA
  // ─────────────────────────────────────────
    const handleHit = async () => {
    if (!gameData) return;

    try {
      setIsActionLoading(true);
      setError(null);

      const token = localStorage.getItem('token_casino');

      const response = await fetch(
        `https://localhost:7101/api/CasinoGames/blackjack/${gameData.gameId}/hit`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        }
      );

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Error ${response.status}`);
      }

      const result: BlackJackResponse = await response.json();
      console.log('Hit response:', result);
      
      setGameData(result);

      // Actualiza con todas las cartas del jugador
      if (result.userCards && result.userCards.length > 0) {
        setLastCardAnimation(true);
        setPlayerCards(result.userCards);
        
        setTimeout(() => setLastCardAnimation(false), 600);
      }

      if (result.isFinished) {
        setGamePhase('FINISHED');
        setTimeout(() => setShowResult(true), 600);
      }

    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error al pedir carta');
    } finally {
      setIsActionLoading(false);
    }
  };

  const manejarToggleMute = () => {
      const nuevoMutado = !musicaMutada;
      setMusicaMutada(nuevoMutado);

      if (nuevoMutado) {
          // Mutear — solo silenciar, no detener
          silenciar(musicaGame, true);
      } else {
          // Desmutear — si el audio murió, lo relanzamos
          reproducir(musicaGame, { loop: true, volume: 0.5 });
          silenciar(musicaGame, false);
      }

      localStorage.setItem('casino_music_muted', String(nuevoMutado));
  };

  // ─────────────────────────────────────────
  // STAND - PLANTARSE
  // ─────────────────────────────────────────
  const handleStand = async () => {
    if (!gameData) return;

    try {
      setIsActionLoading(true);
      setError(null);

      const token = localStorage.getItem('token_casino');

      const response = await fetch(
        `https://localhost:7101/api/CasinoGames/blackjack/${gameData.gameId}/stand`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        }
      );

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Error ${response.status}`);
      }

      const result: BlackJackResponse = await response.json();
      setGameData(result);
      setGamePhase('FINISHED');
      setTimeout(() => setShowResult(true), 600);

    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Error al plantarse');
    } finally {
      setIsActionLoading(false);
    }
  };

  // ─────────────────────────────────────────
  // RESET - NUEVA PARTIDA
  // ─────────────────────────────────────────
  const resetGame = () => {
    cardLabelCache.current.clear(); 
    setGameData(null);
    setGamePhase('SELECT_ANIMAL');
    setSelectedAnimal(null);
    setShowResult(false);
    setPlayerCards([]);
    setError(null);
    fetchUserAnimals();
  };

  // ─────────────────────────────────────────
  // HELPERS UI
  // ─────────────────────────────────────────
  const getResultLabel = (result: number | null) => {
    if (result === GameResult.WIN)  return { text: '🎉 YOU WIN!',  css: 'win'  };
    if (result === GameResult.LOSE) return { text: '💀 YOU LOSE!', css: 'lose' };
    if (result === GameResult.DRAW) return { text: '🤝 DRAW!',     css: 'draw' };
    return { text: '', css: '' };
  };

  const isGameActive = gamePhase === 'PLAYING';
  const isFinished   = gamePhase === 'FINISHED';

    if (isLoadingAnimals) return (
    <main className="loading-screen">
      <LoadingHamster />
    </main>
  );

  return (
    <div className="blackjack-container">

      {/* HEADER */}
      <header className="blackjack-header">
        <button className="btn-back" onClick={() => navigate('/lobby')}>
          ← Back to Lobby
        </button>
        <h1>🃏 BLACKJACK</h1>
        <button className="btn-mute" onClick={manejarToggleMute}>
          {musicaMutada ? '🔇' : '🔊'}
        </button>

      </header>

          <ActiveDrinkEffects/>

      {/* CONTENIDO: 3 COLUMNAS */}
      <div className="blackjack-content">

        {/* ── COLUMNA IZQUIERDA: Animales ── */}
        <AnimalToBetInGame
          animals={animals}
          selectedAnimal={selectedAnimal}
          onSelectAnimal={(animal) => {
            if (gamePhase === 'SELECT_ANIMAL') setSelectedAnimal(animal);
          } }
          isFlipping={isActionLoading}
          error={error}
          isForBet={false}
          isForSale={false}
          correct = {null}
        />

        {/* ── COLUMNA CENTRAL: Mesa de juego ── */}
        <div className="game-panel">

          {/* FASE: Seleccionar animal */}
          {gamePhase === 'SELECT_ANIMAL' && (
            <div className="start-section">
              {selectedAnimal ? (
                <>
                  <div className="selected-animal-info">
                    <h3>Betting: {selectedAnimal.name}</h3>
                    <p>Value: ${selectedAnimal.value.toLocaleString()}</p>
                  </div>
                  <button
                    className="btn-start"
                    onClick={startGame}
                    disabled={isActionLoading}
                  >
                    {isActionLoading ? 'Starting...' : '🃏 DEAL CARDS'}
                  </button>
                </>
              ) : (
                <div className="game-panel-placeholder">
                  <span className="placeholder-icon">🃏</span>
                  <h2></h2>
                  <p>Select an animal</p>
                </div>
              )}
            </div>
          )}

          {/* FASE: Jugando o Terminado */}
          {(isGameActive || isFinished) && gameData && (
            <div className="table-section">

              {/* Mano del Dealer */}
              <div className="hand-section dealer-section">
                <h3 className="hand-label">
                  Dealer score:
                  <span className="score-badge">
                    {isFinished ? gameData.dealerScore : '?'}
                  </span>
                </h3>
                <div className="cards-row">
                  {/* Muestra solo la primera carta mientras el juego está activo */}
                  {isGameActive && gameData.dealerCards && gameData.dealerCards.length > 0 ? (
                    <>
                      <div className="card card-visible">
                        <div className="card-inner">
                          <span className="card-value">{formatCardValue(gameData.dealerCards[0].value, 0, 'dealer')}</span>
                        </div>
                      </div>
                      <div className="card card-hidden">
                        <span>?</span>
                      </div>
                    </>
                  ) : isFinished && gameData.dealerCards ? (
                    gameData.dealerCards.map((card, index) => (
                      <div key={index} className="card card-visible">
                        <div className="card-inner">
                          <span className="card-value">{formatCardValue(card.value, index, 'dealer')}</span>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p style={{ opacity: 0.5 }}>Waiting...</p>
                  )}
                </div>
              </div>

              <div className="vs-divider">VS</div>

              {/* Mano del Jugador */}
              <div className="hand-section player-section">
                <h3 className="hand-label">
                  Your actual score:
                  <span className={`score-badge ${gameData.playerScore > 21 ? 'busted' : gameData.playerScore === 21 ? 'blackjack' : ''}`}>
                    {gameData.playerScore}
                    {gameData.playerScore > 21 && ' 💥 BUST'}
                    {gameData.playerScore === 21 && ' ⭐'}
                  </span>
                </h3>
              
                <div className="cards-row">
                  {playerCards.length > 0 ? (
                    playerCards.map((card, index) => (
                      <div 
                        key={index}
                        className={`card card-visible ${
                          index === playerCards.length - 1 && lastCardAnimation ? 'card-new' : ''
                        } ${
                          gameData.playerScore > 21 ? 'card-bust' : 
                          gameData.playerScore === 21 ? 'card-blackjack' : ''
                        }`}
                      >
                        <div className="card-inner">
                          <span className="card-value">{formatCardValue(card.value, index, 'player')}</span>
                        </div>
                      </div>
                    ))
                  ) : (
                    <p style={{ opacity: 0.5 }}>No cards yet...</p>
                  )}
                </div>
              </div>

              {/* ACCIONES */}
              {isGameActive && (
                <div className="action-buttons">
                  <button
                    className="btn-hit"
                    onClick={handleHit}
                    disabled={isActionLoading}
                  >
                    {isActionLoading ? '...' : 'HIT'}
                  </button>
                  <button
                    className="btn-stand"
                    onClick={handleStand}
                    disabled={isActionLoading}
                  >
                    {isActionLoading ? '...' : 'STAND'}
                  </button>
                </div>
              )}

            </div>
          )}
        </div>

        {/* ── COLUMNA DERECHA: Reglas ── */}
        <div className="info-panel">
          <h2>How to Play</h2>
          <div className="info-content">
            <div className="info-item">
              <span className="info-icon">🎯</span>
              <p>Select one of your animals to bet</p>
            </div>
            <div className="info-item">
              <span className="info-icon">🃏</span>
              <p>Get closer to <strong>21</strong> than the dealer without going over</p>
            </div>
            <div className="info-item">
              <span className="info-icon">💥</span>
              <p><strong>Bust</strong>: Going over 21 means you lose automatically</p>
            </div>
            <div className="info-item">
              <span className="info-icon">💰</span>
              <p><strong>Win</strong>: Beat the dealer → keep your animal + reward</p>
            </div>
            <div className="info-item">
              <span className="info-icon">💀</span>
              <p><strong>Lose</strong>: Your animal will be sacrificed</p>
            </div>
            <div className="info-item">
              <span className="info-icon">🤝</span>
              <p><strong>Tie</strong>: No one wins, your animal is safe</p>
            </div>
          </div>
        </div>
      </div>

      {/* ── MODAL DE RESULTADO ── */}
      {showResult && gameData && isFinished && (
        <div className="modal-overlay">
          <div className={`modal-content ${getResultLabel(gameData.result).css}`}>

            <h2>{getResultLabel(gameData.result).text}</h2>

            <p className="result-message">{gameData.message}</p>

            <div className="result-stats">
              <div className="stat">
                <span>Your Score</span>
                <strong>{gameData.playerScore}</strong>
              </div>
              <div className="stat">
                <span>Dealer Score</span>
                <strong>{gameData.dealerScore}</strong>
              </div>
              {gameData.result === GameResult.WIN && (
                <div className="stat reward">
                  💰 +${gameData.reward.toLocaleString()}
                </div>
              )}
            </div>

            <button className="btn-play-again" onClick={resetGame}>
              Play Again
            </button>

          </div>
        </div>
      )}

    </div>
  );
}

export default BlackJack;