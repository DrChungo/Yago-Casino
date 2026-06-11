import AnimalToBetInGame from '../../components/AnimalToBetInGame';
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../../styles/HigherOrLower.css';
import ActiveDrinkEffects from '../Components/ActiveDrinkEffect';
import { useAudio } from '../../hooks/useAudio'; 
import LoadingHamster from '../../Pages/Components/LoadingHamster';

import musicaGame from '../../assets/Audios/casino_game_3.mp3';
import musicaIntro from '../../assets/Audios/casino_landing_soundtrack.mp3';

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

const Suit = {
  HEARTS: 'HEARTS',
  DIAMONDS: 'DIAMONDS',
  CLUBS: 'CLUBS',
  SPADES: 'SPADES'
} as const;

type Suit = typeof Suit[keyof typeof Suit];

const Rank = {
  TWO: 2,
  THREE: 3,
  FOUR: 4,
  FIVE: 5,
  SIX: 6,
  SEVEN: 7,
  EIGHT: 8,
  NINE: 9,
  TEN: 10,
  JACK: 11,
  QUEEN: 12,
  KING: 13,
  ACE: 14
} as const;

type Rank = typeof Rank[keyof typeof Rank];

const HigherOrLowerChoice = {
  HIGHER: 'HIGHER',
  LOWER: 'LOWER'
} as const;

type HigherOrLowerChoice = typeof HigherOrLowerChoice[keyof typeof HigherOrLowerChoice];

interface Card {
  suit: Suit;
  rank: Rank;
  displayName: string;
}

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

interface HigherLowerRequest {
  animalId: string;
}

interface HigherLowerPlayRequest {
  gameId: string;
  choice: HigherOrLowerChoice;
}

interface HigherLowerResponse {
  gameId: string;
  animalId: string;
  animalName: string;
  animalValue: number;
  reward: number;
  message: string;
  currentCard: Card | null;
  gameEnded: boolean;
}

const HigherOrLower = () => {
  const navigate = useNavigate();
  const { reproducir, detener, silenciar } = useAudio();
  const [musicaMutada, setMusicaMutada] = useState<boolean>(
    () => localStorage.getItem('casino_music_muted') === 'true'
  );
  
  // Estados de animales
  const [animals, setAnimals] = useState<Animal[]>([]);
  const [selectedAnimal, setSelectedAnimal] = useState<Animal | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoadingAnimals, setIsLoadingAnimals] = useState(true);
  const [isFlipping, setIsFlipping] = useState(false);

  // Estados del juego
  const [gameState, setGameState] = useState<HigherLowerResponse | null>(null);
  const [isGameActive, setIsGameActive] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const [showCardReveal, setShowCardReveal] = useState(false);
  const [roundsPlayed, setRoundsPlayed] = useState(0);

  //Estado del modal
  const [isModalOpen, setIsModalOpen] = useState(false);

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

  useEffect(() => {
  if (gameState) {
    console.log('🎮 GameState actualizado:', gameState);
    console.log('🃏 Current Card:', gameState.currentCard);
    console.log('🔢 Rank:', gameState.currentCard?.rank);
    console.log('♠️ Suit:', gameState.currentCard?.suit);
  }
  }, [gameState]);

  const getShortName = (displayName) => {
    const rank = displayName.split(' ')[0]; 
    
    const shortNames = {
      "Jack": "J",
      "Queen": "Q",
      "King": "K",
      "Ace": "A"
    };
    
    return shortNames[rank] || rank; // Si no está en el objeto, devuelve el número
  };

  // En tu componente de carta
  const CardComponent = ({ card }) => {
    return (
      <div className="card">
        <span className="card-value">
          {getShortName(card.displayName)}
        </span>
        <span className="card-suit">{card.suit}</span>
      </div>
    );
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

  // Función para obtener los animales del usuario
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
        throw new Error(errorData?.message || `Error ${response.status}: No se pudieron cargar los animales`);
      }

      const apiResponse: ApiResponse = await response.json();

      console.log('API Response:', apiResponse); // Debug

      if (!apiResponse.success || !apiResponse.data) {
        setError(apiResponse.message || 'No tienes animales disponibles');
        setAnimals([]);
        return;
      }
      
      // Filtrar solo animales disponibles para apostar
      const availableAnimals = apiResponse.data.filter(animal => animal.isAvailable);
      
      setAnimals(availableAnimals);

      if (availableAnimals.length === 0) {
        setError('No tienes animales disponibles para apostar. ¡Compra algunos en la tienda!');
      }

    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Error al cargar los animales';
      setError(errorMessage);
      console.error('Error fetching animals:', err);
    } finally {
      setIsLoadingAnimals(false);
    }
  };


  // Función para iniciar el juego
  const startGame = async () => {
    if (!selectedAnimal) {
      setError('Debes seleccionar un animal para jugar');
      return;
    }

    try {
      setIsProcessing(true);
      setError(null);

      const token = localStorage.getItem('token_casino');

      if (!token) {
        setError('No estás autenticado');
        navigate('/login');
        return;
      }

      const request: HigherLowerRequest = {
        animalId: selectedAnimal.id
      };

      const response = await fetch('https://localhost:7101/api/CasinoGames/higherlower/start', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      });

      if (response.status === 401) {
        setError('Tu sesión ha expirado');
        localStorage.removeItem('token_casino');
        setTimeout(() => navigate('/login'), 2000);
        return;
      }

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || 'Error al iniciar el juego');
      }

      const result: HigherLowerResponse = await response.json();
      
      setGameState(result);
      setIsGameActive(true);
      setRoundsPlayed(0);
      setIsModalOpen(true); // Abrir modal

    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError(errorMessage);
    } finally {
      setIsProcessing(false);
    }
  };

  // Función para hacer una elección (Higher o Lower)
  const makeChoice = async (choice: HigherOrLowerChoice) => {
    if (!gameState || gameState.gameEnded) return;

    try {
      setIsProcessing(true);
      setShowCardReveal(true);
      setError(null);

      const token = localStorage.getItem('token_casino');

      const request: HigherLowerPlayRequest = {
        gameId: gameState.gameId,
        choice: choice
      };

      await new Promise(resolve => setTimeout(resolve, 1500));

      const response = await fetch('https://localhost:7101/api/CasinoGames/higherlower/play', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || 'Error al jugar');
      }

      const result: HigherLowerResponse = await response.json();
      
      setGameState(result);
      setShowCardReveal(false);
      setRoundsPlayed(prev => prev + 1);

      if (result.gameEnded) {
        setIsGameActive(false);
        if (result.message.toLowerCase().includes('lose') || result.message.toLowerCase().includes('dead')) {
          setTimeout(() => fetchUserAnimals(), 2000);
        }
      }

    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError(errorMessage);
      setShowCardReveal(false);
    } finally {
      setIsProcessing(false);
    }
  };

  const cashOut = async () => {
    if (!gameState || gameState.gameEnded) return;

    try {
      setIsProcessing(true);
      setError(null);

      const token = localStorage.getItem('token_casino');

      const response = await fetch(`https://localhost:7101/api/CasinoGames/higherlower/${gameState.gameId}/cashout`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || 'Error al retirar');
      }

      const result: HigherLowerResponse = await response.json();
      
      setGameState(result);
      setIsGameActive(false);

    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError(errorMessage);
    } finally {
      setIsProcessing(false);
    }
  };

  const resetGame = () => {
    setGameState(null);
    setIsGameActive(false);
    setSelectedAnimal(null);
    setError(null);
    setShowCardReveal(false);
    setRoundsPlayed(0);
    setIsModalOpen(false);
  };

  const getRankDisplay = (rank: Rank | string, displayName?: string): string => {
    if (displayName) {
      const rankPart = displayName.split(' ')[0]; 
      
      const shortNames: Record<string, string> = {
        'Jack': 'J',
        'Queen': 'Q',
        'King': 'K',
        'Ace': 'A'
      };
      
      return shortNames[rankPart] || rankPart; // Devuelve "J", "Q", "K", "A" o el número
    }
    
    // Fallback: Si solo tenemos el rank como string
    if (typeof rank === 'string') {
      const stringToDisplay: Record<string, string> = {
        'Two': '2', 'Three': '3', 'Four': '4', 'Five': '5', 
        'Six': '6', 'Seven': '7', 'Eight': '8', 'Nine': '9', 
        'Ten': '10', 'Jack': 'J', 'Queen': 'Q', 'King': 'K', 'Ace': 'A'
      };
      return stringToDisplay[rank] || rank;
    }
    
    // Fallback: Si tenemos el rank como número
    const displays: Record<number, string> = {
      2: '2', 3: '3', 4: '4', 5: '5', 6: '6', 7: '7', 8: '8', 9: '9', 10: '10',
      11: 'J', 12: 'Q', 13: 'K', 14: 'A'
    };
    return displays[rank] || '?';
  };

  if (isLoadingAnimals) return (
      <main className="loading-screen">
        <LoadingHamster />
      </main>
  );

  return (
    <div className="higherorlower-container">
      <div className="higherorlower-header">

        <button className="btn-back" onClick={() => navigate(-1)}>← Back to Lobby</button>
        
        <h1>🎴 Higher or Lower</h1>

        <button className="btn-mute" onClick={manejarToggleMute}>
          {musicaMutada ? '🔇' : '🔊'}
        </button>

      </div>

          <ActiveDrinkEffects corner={ true } />

      <div className="higherorlower-content">
        {/* Panel izquierdo: Selección de animal */}
        <div className="selection-section">
          {isLoadingAnimals ? (
            <div className="loading-container">
              <div className="spinner"></div>
              <p>Cargando tus animales...</p>
            </div>
          ) : (
            <AnimalToBetInGame
              animals={animals}
              selectedAnimal={selectedAnimal}
              onSelectAnimal={setSelectedAnimal}
              isFlipping={isFlipping}
              error={error}
              isForBet={false}
              isForSale={false}
              correct = {null}
            />
          )}
        </div>

        {/* Panel central: Vista de inicio */}
        <div className="game-section">
          <div className="start-game-view">
            <div className="game-title-card">
              <h2>🎴 Higher or Lower</h2>
              <p>Guess if the next card will be higher or lower!</p>
            </div>

            {selectedAnimal && (
              <div className="selected-animal-card">
                <h3>Selected Animal</h3>
                <div className="animal-info">
                  <p className="animal-name">{selectedAnimal.name}</p>
                  <p className="animal-type">{selectedAnimal.typeAnimal}</p>
                  <p className="animal-value">💰 ${selectedAnimal.value.toLocaleString()}</p>
                </div>
              </div>
            )}

            <button
              className="btn-start-game"
              onClick={startGame}
              disabled={!selectedAnimal || isProcessing}
            >
              {isProcessing ? 'Starting...' : 'Start Game'}
            </button>
          </div>
        </div>

        {/* Panel derecho: Instrucciones */}
        <div className="info-section">
          <div className="info-card">
            <h3>How to Play</h3>
            <ul className="instructions-list">
              <li>

                <span>🎯 Select an animal to bet</span>
              </li>
              <li>
                <span>❓ Guess if the next card will be HIGHER or LOWER</span>
              </li>
              <li>
                <span>💸 Each correct guess multiplies your reward by 1.5x</span>
              </li>
              <li>
                <span>💰 Cash out anytime to keep your winnings</span>
              </li>
              <li>
                <span>💀 Wrong guess = lose your animal</span>
              </li>
            </ul>
          </div>

          <div className="info-card tips">
            <h3>Tips</h3>
            <ul className="tips-list">
              <li>If cards are equal, you win automatically!</li>
              <li>The deck has 52 cards</li>
              <li>Aces are the highest (14)</li>
              <li>Cash out before it's too late!</li>
            </ul>
          </div>
        </div>
      </div>

      {/* MODAL DEL JUEGO */}
      {isModalOpen && gameState && (

        <div className="game-modal-overlay" onClick={(e) => {
          if (e.target === e.currentTarget && gameState.gameEnded) {
            resetGame();
          }
        }}>
          <div className="game-modal">

            <div className="modal-content">
              {/* Información del juego */}
              <div className="game-info-bar">
                <div className="info-item">
                  <span className="label">Animal:</span>
                  <span className="value">{gameState.animalName}</span>
                </div>
                <div className="info-item highlight">
                  <span className="label">Reward:</span>
                  <span className="value">💰 ${gameState.reward.toLocaleString()}</span>
                </div>
                <div className="info-item">
                  <span className="label">Rounds:</span>
                  <span className="value">{roundsPlayed}</span>
                </div>
              </div>

              {/* Cartas minimalistas */}
              <div className="cards-container">
                {gameState.currentCard && (
                  <div className="card-wrapper">
                    <p className="card-label">Current Card</p>
                    <div className="minimal-card current">
                      {getRankDisplay(gameState.currentCard.rank)}
                    </div>
                  </div>
                )}

                <div className="vs-separator">VS</div>

                <div className="card-wrapper">
                  <p className="card-label">Next Card</p>
                  <div
                    className={`minimal-card next ${isProcessing ? 'loading' : ''} ${showCardReveal ? 'revealing' : ''}`}
                  >
                    <span className="next-card-content">
                      {isProcessing ? '🤔' : '?'}
                    </span>
                  </div>
                </div>

              </div>

              {/* Mensaje del juego */}
              {gameState.message && (
                <div className={`game-message ${gameState.gameEnded ? 'game-ended' : ''}`}>
                  <p>{gameState.message}</p>
                </div>
              )}

              {/* Botones de acción */}
              {!gameState.gameEnded ? (
                <div className="action-buttons">
                  <button
                    className="btn-choice btn-higher"
                    onClick={() => makeChoice('HIGHER')}
                    disabled={isProcessing}
                  >
                    <span className="icon">⬆️</span>
                    <span>HIGHER</span>
                  </button>
                  <button
                    className="btn-choice btn-lower"
                    onClick={() => makeChoice('LOWER')}
                    disabled={isProcessing}
                  >
                    <span className="icon">⬇️</span>
                    <span>LOWER</span>
                  </button>
                  <button
                    className="btn-cashout"
                    onClick={cashOut}
                    disabled={isProcessing || roundsPlayed === 0}
                  >
                    💰 Cash Out
                  </button>
                </div>
              ) : (
                <button className="btn-play-again" onClick={resetGame}>
                  Play Again
                </button>
              )}

              {/* Estadísticas */}
              <div className="game-stats-modal">
                <div className="stat-row">
                  <span>Initial Bet:</span>
                  <span className="stat-value">${gameState.animalValue.toLocaleString()}</span>
                </div>
                <div className="stat-row">
                  <span>Current Multiplier:</span>
                  <span className="stat-value">
                    {gameState.reward > 0 ? `${(gameState.reward / gameState.animalValue).toFixed(2)}x` : '1x'}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default HigherOrLower;