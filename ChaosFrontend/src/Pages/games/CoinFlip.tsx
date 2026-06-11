import AnimalToBetInGame from '../../components/AnimalToBetInGame';
import { useState, useEffect} from 'react';
import { useNavigate } from 'react-router-dom';
import ActiveDrinkEffects from '../Components/ActiveDrinkEffect';
import '../../styles/CoinFlip.css';
import { useAudio } from '../../hooks/useAudio'; 
import LoadingHamster from '../../Pages/Components/LoadingHamster';

import musicaGame from '../../assets/Audios/casino_game_1.mp3';
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

interface CoinFlipRequest {
  animalId: string;
  headOrTail: boolean;
}

interface CoinFlipResponse {
  animalName: string;
  possibility: number;
  reward: number;
  won: boolean;
  message: string;
}

const CoinFlip = () => {
  const navigate = useNavigate();
  const { reproducir, detener, silenciar} = useAudio();
  const [musicaMutada, setMusicaMutada] = useState<boolean>(
    () => localStorage.getItem('casino_music_muted') === 'true'
  );

  const [animals, setAnimals] = useState<Animal[]>([]);
  const [selectedAnimal, setSelectedAnimal] = useState<Animal | null>(null);
  const [selectedSide, setSelectedSide] = useState<boolean | null>(null);
  const [isFlipping, setIsFlipping] = useState(false);
  const [gameResult, setGameResult] = useState<CoinFlipResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [showResult, setShowResult] = useState(false);
  const [isLoadingAnimals, setIsLoadingAnimals] = useState(true);
  const [resultSide, setResultSide] = useState<boolean | null>(null);

  // ARGAR ANIMALES AL MONTAR EL COMPONENTE
  useEffect(() => {
    fetchUserAnimals();
  }, []);


  // Manejo de música al entrar/salir del juego
  useEffect(() => {
      detener(musicaIntro);
      reproducir(musicaGame, { loop: true, volume: 0.5 });
      silenciar(musicaGame, localStorage.getItem('casino_music_muted') === 'true');

      return () => {
          detener(musicaGame);                                        
          reproducir(musicaIntro, { loop: true, volume: 0.6 });
          silenciar(musicaIntro, localStorage.getItem('casino_music_muted') === 'true');
      };
  }, []);

  // UNCIÓN PARA OBTENER LOS ANIMALES DEL USUARIO
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
  
  // FUNCIÓN PARA JUGAR AL COIN FLIP
  const playCoinFlip = async () => {

    //si se ha iniciado una partida no de puede jugar otra hasta que termine la animación y se muestre el resultado
    isFlipping && setError('Ya estás jugando una partida. Espera el resultado antes de jugar otra.');
    setIsFlipping(true);

    if (!selectedAnimal || selectedSide === null) {
      setError('Debes seleccionar un animal y un lado de la moneda');
      return;
    }

    try {
            
      setError(null);
      setShowResult(false);

      const token = localStorage.getItem('token_casino');

      if (!token) {
        setError('No estás autenticado. Por favor inicia sesión nuevamente.');
        navigate('/login');
        return;
      }

      // Hacer la solicitud al backend
      const request: CoinFlipRequest = {
        animalId: selectedAnimal.id,
        headOrTail: selectedSide
      };

      const response = await fetch('https://localhost:7101/api/CasinoGames/CoinGame', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(request)
      });

      // Manejo errores
      if (response.status === 401) {
        setError('Tu sesión ha expirado. Por favor inicia sesión nuevamente.');
        localStorage.removeItem('token_casino');
        setTimeout(() => navigate('/login'), 2000);
        return;
      }

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Error ${response.status}`);
      }

      const result: CoinFlipResponse = await response.json();

      const finalSide = result.won ? selectedSide : !selectedSide;

      setResultSide(finalSide);

      setIsFlipping(true);

      // Mostrar resultado después de la animación
      setTimeout(() => {
        setIsFlipping(false);
        setGameResult(result);  
        setShowResult(true);   
        fetchUserAnimals(); // Recargar animales después de mostrar resultado 
      }, 2000);

    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido';
      setError(errorMessage);
      setIsFlipping(false);
    }
  };

  const resetGame = () => {
    setGameResult(null);
    setShowResult(false);
    setSelectedAnimal(null);
    setSelectedSide(null);
    setError(null);
    setResultSide(null);
  };

    const calculateProbability = (animalValue: number) => {
    const max = 3000000;
    animalValue = Math.min(animalValue, max);

    let probability;

    if (animalValue <= 100000) {
        probability = 70 - (animalValue * 10.0 / 100000);
    } else {
        const remaining = animalValue - 100000;
        const range = max - 100000;
        probability = 60 - (remaining * 40.0 / range);
    }

    probability = Math.max(20, Math.min(70, probability));

    return Math.round(probability);
    };

      if (isLoadingAnimals) return (
        <main className="loading-screen">
          <LoadingHamster />
        </main>
      );

  return (
    <div className="coinflip-container">
      <header className="coinflip-header">
        <button className="btn-back" onClick={() => navigate('/lobby')}>
          ← Back to Lobby
        </button>
        <h1>🪙 COIN FLIP</h1>
        <button className="btn-mute" onClick={manejarToggleMute}>
          {musicaMutada ? '🔇' : '🔊'}
        </button>

      </header>

          <ActiveDrinkEffects/>

      <div className="coinflip-content">

        {/* Panel izquierdo: Selección de animal */}
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

        {/* Panel central: Moneda y controles */}
        <div className="game-panel">
          {selectedAnimal && (
            <div className="selected-animal-info">
              <h3>Betting: {selectedAnimal.name}</h3>
              <p>Value: ${selectedAnimal.value.toLocaleString()}</p>
            </div>
          )}

          {/* Selección de lado */}
          <div className="coin-selection">
            <h3>Choose Your Side</h3>
            <div className="side-buttons">
              <button
                className={`btn-side ${selectedSide === true ? 'selected' : ''}`}
                onClick={() => !isFlipping && setSelectedSide(true)}
                disabled={isFlipping}
              >
                <span className="coin-face">🦁</span>
                <span>HEAD</span>
              </button>
              <button
                className={`btn-side ${selectedSide === false ? 'selected' : ''}`}
                onClick={() => !isFlipping && setSelectedSide(false)}
                disabled={isFlipping}
              >
                <span className="coin-face">🐁</span>
                <span>TAIL</span>
              </button>
            </div>
          </div>

          {/* Moneda animada */}
          
          <div className="coin-display">    
            <div 
              className="coin"
              style={{
                transform: isFlipping
                  ? `rotateY(${resultSide === true ? 1800 : 1980}deg)`
                  : resultSide === true
                    ? 'rotateY(0deg)'
                    : resultSide === false
                      ? 'rotateY(180deg)'
                      : 'rotateY(0deg)'
              }}
            >
              <div className="coin-side heads">🦁</div>
              <div className="coin-side tails">🐁</div>
            </div>
          </div>

          {/* Botón de jugar */}
          <button
            className="btn-flip"
            onClick={playCoinFlip}
            disabled={!selectedAnimal || selectedSide === null || isFlipping}
          >
            {isFlipping ? 'FLIPPING...' : 'FLIP THE COIN'}
          </button>

          {/* Resultado */}   
          {showResult && gameResult && (
            <div className="modal-overlay">
              <div className={`modal-content ${gameResult.won ? 'win' : 'lose'}`}>

                <h2>
                  {gameResult.won ? '🎉 YOU WON!' : '💀 YOU LOST!'}
                </h2>

                <p className="result-message">{gameResult.message}</p>

                <div className="result-stats">
                  <div className="stat">
                    <span>Win Probability:</span>
                    <strong>{gameResult.possibility}%</strong>
                  </div>

                  {gameResult.won && (
                    <div className="stat reward">
                      💰 ${gameResult.reward.toLocaleString()}
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

        {/* Panel derecho: Información */}
        <div className="info-panel">
          <h2>How to Play</h2>
          <div className="info-content">
            <div className="info-item">
              <span className="info-icon">🎯</span>
              <p>Select one of your animals to bet</p>
            </div>
            <div className="info-item">
              <span className="info-icon">🪙</span>
              <p>Choose HEAD or TAIL</p>
            </div>
            <div className="info-item">
              <span className="info-icon">🎲</span>
              <p>Win probability: 20-70% (based on animal value)</p>
            </div>
            <div className="info-item">
              <span className="info-icon">💰</span>
              <p>Win: Keep your animal + get reward</p>
            </div>
            <div className="info-item">
              <span className="info-icon">💀</span>
              <p>Lose: Your animal will be sacrificed</p>
            </div>
          </div>

          {selectedAnimal && (
            <div className="probability-indicator">
              <h3>Current Probability</h3>
              <div className="probability-bar">
                <div 
                  className="probability-fill"
                  style={{ 
                    width: `${calculateProbability(selectedAnimal.value)}%` 
                  }}
                />
              </div>
              <p className="probability-text">
                ~{calculateProbability(selectedAnimal.value)}% chance to win
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default CoinFlip;