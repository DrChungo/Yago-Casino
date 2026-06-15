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
import flySvg from '../assets/AnimalsSprites/SVG/Fly.svg';
import hamsterSvg from '../assets/AnimalsSprites/SVG/Hamster.svg';
import catSvg from '../assets/AnimalsSprites/SVG/Cat.svg';
import dogSvg from '../assets/AnimalsSprites/SVG/Dog.svg';
import sheepSvg from '../assets/AnimalsSprites/SVG/Sheep.svg';
import cowSvg from '../assets/AnimalsSprites/SVG/Cow.svg';
import horseSvg from '../assets/AnimalsSprites/SVG/Horse.svg';
import crocodileSvg from '../assets/AnimalsSprites/SVG/Crocodile.svg';
import sharkSvg from '../assets/AnimalsSprites/SVG/Shark.svg';
import whaleSvg from '../assets/AnimalsSprites/SVG/Whale.svg';
import mechaFlySvg from '../assets/AnimalsSprites/SVG/MechaFly.svg';
import mechaHamsterSvg from '../assets/AnimalsSprites/SVG/MechaHamster.svg';
import mechaCatSvg from '../assets/AnimalsSprites/SVG/MechaCat.svg';
import mechaDogSvg from '../assets/AnimalsSprites/SVG/MechaDog.svg';
import mechaSheepSvg from '../assets/AnimalsSprites/SVG/MechaSheep.svg';
import mechaCowSvg from '../assets/AnimalsSprites/SVG/MechaCow.svg';
import mechaHorseSvg from '../assets/AnimalsSprites/SVG/MechaHorse.svg';
import mechaCrocodileSvg from '../assets/AnimalsSprites/SVG/MechaCrocodile.svg';
import mechaSharkSvg from '../assets/AnimalsSprites/SVG/MechaShark.svg';
import mechaWhaleSvg from '../assets/AnimalsSprites/SVG/MechaWhale.svg';

import  lovebirdSvg from '../assets/AnimalsSprites/SVG/Lovebird.svg';
import  mechaLovebirdSvg from '../assets/AnimalsSprites/SVG/MechaLovebird.svg';

import turtleSvg from '../assets/AnimalsSprites/SVG/TurtleMarine.svg';
import mechaTurtleSvg from '../assets/AnimalsSprites/SVG/MechaTurtleMarine.svg';

import tRex from '../assets/AnimalsSprites/SVG/TRex.svg';
import mechaTRex from '../assets/AnimalsSprites/SVG/MechaTRexx.svg';

import phoenix from '../assets/AnimalsSprites/SVG/Phoenix.svg';
import mechaPhoenix from '../assets/AnimalsSprites/SVG/PhoenixMecha.svg';

import pterodactylus from '../assets/AnimalsSprites/SVG/Pterodactylus.svg';
import mechaPterodactylus from '../assets/AnimalsSprites/SVG/MechaPterodactylus.svg';

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

  const getRarityName = (rarity: boolean): string => {
    return rarity ? 'MK' : 'Normal';
  };

    const getAnimalImage = (animal: Animal): string => {
        const isMecha = animal.rarity === true;
        const typeKey = (animal.typeAnimal || '').toLowerCase();

        // 1. Try resolving using backend database configurations first
        const config = animalConfigs.get(typeKey);
        if (config) {
            const svgBd = isMecha ? config.imageUrlMecha : config.imageUrlNormal;
            const resolved = getAnimalImageUrl(svgBd);
            if (resolved) return resolved;
        }

        // 2. Fallback to hardcoded imported SVGs
        const normalImages: Record<string, string> = {
            [AnimalType.FLY.toLowerCase()]: flySvg,
            [AnimalType.HAMSTER.toLowerCase()]: hamsterSvg,
            [AnimalType.CAT.toLowerCase()]: catSvg,
            [AnimalType.DOG.toLowerCase()]: dogSvg,
            [AnimalType.SHEEP.toLowerCase()]: sheepSvg,
            [AnimalType.COW.toLowerCase()]: cowSvg,
            [AnimalType.HORSE.toLowerCase()]: horseSvg,
            [AnimalType.CROCODILE.toLowerCase()]: crocodileSvg,
            [AnimalType.SHARK.toLowerCase()]: sharkSvg,
            [AnimalType.WHALE.toLowerCase()]: whaleSvg,
            [AnimalType.LOVEBIRD.toLowerCase()]: lovebirdSvg,
            [AnimalType.TURTLEMARINE.toLowerCase()]: turtleSvg,
            [AnimalType.T_REX.toLowerCase()]: tRex,
            [AnimalType.PHOENIX.toLowerCase()]: phoenix,
            [AnimalType.PTERODACTYLUS.toLowerCase()]: pterodactylus,
        };

        const mechaImages: Record<string, string> = {
            [AnimalType.FLY.toLowerCase()]: mechaFlySvg,
            [AnimalType.HAMSTER.toLowerCase()]: mechaHamsterSvg,
            [AnimalType.CAT.toLowerCase()]: mechaCatSvg,
            [AnimalType.DOG.toLowerCase()]: mechaDogSvg,
            [AnimalType.SHEEP.toLowerCase()]: mechaSheepSvg,
            [AnimalType.COW.toLowerCase()]: mechaCowSvg,  
            [AnimalType.HORSE.toLowerCase()]: mechaHorseSvg,
            [AnimalType.CROCODILE.toLowerCase()]: mechaCrocodileSvg,
            [AnimalType.SHARK.toLowerCase()]: mechaSharkSvg,
            [AnimalType.WHALE.toLowerCase()]: mechaWhaleSvg,
            [AnimalType.LOVEBIRD.toLowerCase()]: mechaLovebirdSvg,
            [AnimalType.TURTLEMARINE.toLowerCase()]: mechaTurtleSvg,
            [AnimalType.T_REX.toLowerCase()]: mechaTRex,
            [AnimalType.PHOENIX.toLowerCase()]: mechaPhoenix,
            [AnimalType.PTERODACTYLUS.toLowerCase()]: mechaPterodactylus
        };
    
        const resolvedMap = isMecha ? mechaImages : normalImages;
        return resolvedMap[typeKey] || resolvedMap[typeKey.replace(/\s+/g, '')] || '';
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
            <p className="animal-rarity" style={{ color: getRarityColor(animal.rarity) }}>
              {getRarityName(animal.rarity)}
            </p>
                    <p className="animal-value">${animal.value.toLocaleString()}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default AnimalToBetInGame;