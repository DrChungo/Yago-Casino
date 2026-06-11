import { useState, useRef } from 'react';
import SpinWheel from './SpinWheel';

interface AnimalResponse {
    id: string;
    name: string;
}

interface WheelResult {
    animal: AnimalResponse | null;
}

interface Props {
    spinCount: number;
    animals: AnimalResponse[];
    onAllDone: (results: WheelResult[]) => void;
}

// Genera segmentos con el animal ganador + relleno de vacíos
// El ganador siempre existe en la rueda en una posición aleatoria
const buildSegments = (winner: AnimalResponse | null) => {
    const TOTAL = 10;
    const segments: { label: string; animal: AnimalResponse | null }[] = [];

    // Llenamos todo de vacío primero
    for (let i = 0; i < TOTAL; i++) {
        segments.push({ label: '💨 Vacío', animal: null });
    }

    // Si hay ganador, lo ponemos en una posición aleatoria
    if (winner) {
        const winnerPos = Math.floor(Math.random() * TOTAL);
        segments[winnerPos] = { label: `🐾 ${winner.name}`, animal: winner };
        return { segments, winnerIndex: winnerPos };
    }

    // Si no hay ganador (tirada vacía), apuntamos a cualquier vacío
    const emptyIndex = Math.floor(Math.random() * TOTAL);
    return { segments, winnerIndex: emptyIndex };
};

// ─── Una sola ruleta ────────────────────────────────────────────────────────
interface SingleWheelProps {
    index: number;
    winner: AnimalResponse | null;
    isActive: boolean;       // solo gira si es la activa
    onDone: (result: WheelResult) => void;
}

const SingleAnimalWheel = ({ index, winner, isActive, onDone }: SingleWheelProps) => {
    const wheelRef = useRef<{ spin: (index: number) => void }>(null);
    const [spun, setSpun] = useState(false);
    const [result, setResult] = useState<WheelResult | null>(null);

    const { segments, winnerIndex } = buildSegments(winner);

    const handleSpin = () => {
        if (spun || !isActive) return;
        setSpun(true);
        wheelRef.current?.spin(winnerIndex);
    };

    const handleSpinEnd = () => {
        const r = { animal: segments[winnerIndex].animal };
        setResult(r);
        onDone(r);
    };

    return (
        <div className="single-wheel-container">
            <h4 className="wheel-number">Tirada #{index + 1}</h4>

            <SpinWheel
                ref={wheelRef}
                segments={segments}
                size={260}
                onSpinEnd={handleSpinEnd}
            />

            {/* Botón solo visible si es la ruleta activa y no giró aún */}
            {isActive && !spun && (
                <button className="spin-btn small" onClick={handleSpin}>
                    🎰 Girar
                </button>
            )}

            {/* Resultado */}
            {result && (
                <div className={`wheel-result ${result.animal ? 'win' : 'empty'}`}>
                    {result.animal ? `✅ ${result.animal.name}` : '❌ Vacío'}
                </div>
            )}

            {/* Esperando turno */}
            {!isActive && !spun && (
                <p className="waiting-label">⏳ Esperando...</p>
            )}
        </div>
    );
};

// ─── Controlador de ruletas secuenciales ────────────────────────────────────
const AnimalWheels = ({ spinCount, animals, onAllDone }: Props) => {
    const [currentIndex, setCurrentIndex] = useState(0); // cuál ruleta está activa
    const [results, setResults] = useState<WheelResult[]>([]);

    const handleWheelDone = (result: WheelResult) => {
        const newResults = [...results, result];
        setResults(newResults);

        if (newResults.length === spinCount) {
            // Todas terminaron
            onAllDone(newResults);
        } else {
            // Avanza a la siguiente ruleta
            setCurrentIndex((prev) => prev + 1);
        }
    };

    return (
        <div className="animal-wheels-sequential">
            {/* Solo mostramos la ruleta activa */}
            {currentIndex < spinCount && (
                <SingleAnimalWheel
                    key={currentIndex}           // 👈 key cambia → remonta el componente limpio
                    index={currentIndex}
                    winner={animals[currentIndex] ?? null}
                    isActive={true}
                    onDone={handleWheelDone}
                />
            )}

            {/* Mini historial de resultados anteriores */}
            {results.length > 0 && (
                <div className="results-history">
                    {results.map((r, i) => (
                        <span
                            key={i}
                            className={`history-chip ${r.animal ? 'win' : 'empty'}`}
                        >
                            {r.animal ? `🐾 ${r.animal.name}` : '❌ Vacío'}
                        </span>
                    ))}
                </div>
            )}
        </div>
    );
};

export default AnimalWheels;