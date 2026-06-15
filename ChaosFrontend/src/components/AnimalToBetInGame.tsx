// src/components/AnimalToBetInGame.tsx
// Este componente muestra los animales disponibles para apostar en el juego de Coin Flip, permitiendo al usuario seleccionar uno y mostrando su rareza y valor.
//
//EJEMPLO DE USO:
// <AnimalToBetInGame
//   animals={animals} // Lista de animales obtenida de la API
//   selectedAnimal={selectedAnimal} // El animal actualmente seleccionado por el usuario
//   onSelectAnimal={handleSelectAnimal} // Función para manejar la selección de un animal
//   isFlipping={isFlipping} // Estado que indica si la moneda está volando
//   error={error} // Mensaje de error a mostrar si no hay animales disponibles
// />

import { useState, useEffect } from 'react';
import { getAnimalImageUrl } from '../services/animalImageService';

import './AnimalToBetInGame.css';

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
  TURTLEMARINE: 'TURTLE MARINE',
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
interface AnimalToBetInGameProps {
  animals: Animal[];
  selectedAnimal: Animal | null;
  onSelectAnimal: (animal: Animal) => void;
  isFlipping: boolean;
  error: string | null;
  isForBet: boolean;
  isForSale: boolean;
  correct: string | null;
}

const AnimalToBetInGame = ({
  animals,
  onSelectAnimal,
  selectedAnimal,
  isFlipping,
  error,
  isForBet,
  isForSale,
  correct,
}: AnimalToBetInGameProps) => {

  const [animalConfigs, setAnimalConfigs] = useState<Map<string, any>>(new Map());

  useEffect(() => {
    const fetchConfigs = async () => {
      const token = localStorage.getItem('token_casino');
      if (!token) return;
      try {
        const res = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/AnimalValueConfig/images`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        if (res.ok) {
          const data = await res.json();
          const configMap = new Map<string, any>();
          (data || []).forEach((h: any) => {
            const tipo = h.animalType || h.typeAnimal;
            if (tipo) configMap.set(tipo.toLowerCase(), h);
          });
          setAnimalConfigs(configMap);
        }
      } catch (e) {
        console.error("Error loading animal configs in AnimalToBetInGame:", e);
      }
    };
    fetchConfigs();
  }, []);

  const getRarityColor = (rarity: boolean): string => {
    return rarity ? '#EF4444' : '#9CA3AF';
  };

  const getAnimalImage = (animal: Animal): string => {
    const isMecha = animal.rarity === true;
    const typeKey = (animal.typeAnimal || '').toLowerCase();

    // Try resolving using backend database configurations (wwwroot path)
    const config = animalConfigs.get(typeKey);
    if (config) {
      const svgBd = isMecha ? config.imageUrlMecha : config.imageUrlNormal;
      const resolved = getAnimalImageUrl(svgBd);
      if (resolved) return resolved;
    }

    return '';
  };

  return (
    <div className="selection-panel">


      {isForBet ? (
        <h2>Select Your Animal to Bet</h2>
      ) : isForSale ? (
        <h2>Select Animal to Sell</h2>
      ) : (
        <h2>Select Animal to Buy</h2>
      )}



      {error && <div className="error-message">{error}</div>}
      {correct && <div className="correct-message">{correct}</div>}

      {(!animals || animals.length === 0) && (
        <div className="no-animals">
          <p>You don't have any animals available!</p>
        </div>
      )}

      <div className="animals-grid">
        {animals.map((animal) => (
          <div
            key={animal.id}
            className={`animal-card ${selectedAnimal?.id === animal.id ? 'selected' : ''} ${animal.rarity ? 'mecha' : ''}`}
            onClick={() => !isFlipping && onSelectAnimal(animal)}
            style={{ borderColor: getRarityColor(animal.rarity) }}
          >
            <div className="animal-placeholder">
              <img
                src={getAnimalImage(animal)}
                alt={animal.name}
                className="animal-svg"
              />
            </div>
            <h3>{animal.name}</h3>
            <p className="animal-type" style={{ color: getRarityColor(animal.rarity), marginTop: '4px', textTransform: 'capitalize' }}>
              {animal.typeAnimal.toLowerCase()}
            </p>
            <p className="animal-value">${animal.value.toLocaleString()}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default AnimalToBetInGame;