/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState, useRef, useCallback, useEffect } from 'react';
import SpinWheel from './SpinWheel';
import DifficultySelector from './DifficultySelector';
import './AnimalRoulette.css';

// ─── Interfaces ───────────────────────────────────────────────────────────────
interface AnimalResponse {
    id: string;
    name: string;
}

interface SpinResult {
    success: boolean;
    message: string;
    candidates: AnimalResponse[];
    winner: AnimalResponse | null;
}

interface RouletteApiResponse {
    success: boolean;
    message: string;
    data: SpinResult[] | null;
    totalSpins: number;
}

interface Props {
    onFinished?: () => void;
    userBalance: number;
}

interface Segment {
    label: string;
    animal: AnimalResponse | null;
    isWinner: boolean;
}

interface MainSegment {
    label: string;
    value: number;
}

// ─── Constantes ───────────────────────────────────────────────────────────────
const shuffle = (arr: number[]) => {
    const a = [...arr];
    for (let i = a.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [a[i], a[j]] = [a[j], a[i]];
    }
    return a;
};

const DIFFICULTY_SEGMENTS: Record<string, MainSegment[]> = {
    facil: shuffle([0, 0, 1, 1, 2, 1, 2]).map(v => ({ label: `${v}`, value: v })),
    medio: shuffle([0, 0, 1, 2, 2, 3, 1, 2, 3, 1]).map(v => ({ label: `${v}`, value: v })),
    dificil: shuffle([0, 0, 1, 2, 3, 4, 2, 3, 4, 1]).map(v => ({ label: `${v}`, value: v })),
};

type Difficulty = keyof typeof DIFFICULTY_SEGMENTS;

const DIFFICULTY_PRICE: Record<Difficulty, { label: string; price: number }> = {
    facil: { label: 'Fácil', price: 500000 },
    medio: { label: 'Medio', price: 750000 },
    dificil: { label: 'Difícil', price: 1000000 },
};

type Phase = 'select' | 'spinning' | 'result' | 'done';

// ─── Tamaño de la ruleta ──────────────────────────────────────────────────────
const getWheelSize = () => {
    const vw = window.innerWidth;
    if (vw >= 1400) return 300;
    if (vw >= 1100) return 260;
    if (vw >= 850) return 230;
    if (vw >= 600) return 280;
    return 220;
};

const useWheelSize = () => {
    const [size, setSize] = useState<number>(() => getWheelSize());

    useEffect(() => {
        const handleResize = () => setSize(getWheelSize());
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    return size;
};

// ─── Helper ───────────────────────────────────────────────────────────────────
const buildSegmentsFromCandidates = (
    candidates: AnimalResponse[],
    winner: AnimalResponse | null
): Segment[] => {
    const badLuckSegment: Segment = {
        label: '😢 Sin suerte',
        animal: null,
        isWinner: winner === null,
    };

    if (!candidates || candidates.length === 0) {
        return [
            badLuckSegment,
            ...Array(4).fill(null).map(() => ({
                label: '❓',
                animal: null,
                isWinner: false,
            })),
        ];
    }

    const candidateSegs: Segment[] = candidates.map((c) => ({
        label: c?.name ?? 'Desconocido',
        animal: c ?? null,
        isWinner: winner != null && c?.id === winner?.id,
    }));

    return [badLuckSegment, ...candidateSegs];
};

// ─── DoneScreen ───────────────────────────────────────────────────────────────
const DoneScreen = ({
    results,
    onReset,
}: {
    results: SpinResult[];
    onReset: () => void;
}) => {
    const winners = results.filter((r) => r?.winner != null);
    return (
        <div className="done-section">
            <h3 className="done-title">
                {winners.length > 0 ? '🎉 ¡Felicidades!' : '😢 ¡Mala suerte!'}
            </h3>
            {winners.length > 0 ? (
                <div className="animals-summary">
                    <h4>Animales obtenidos:</h4>
                    <ul>
                        {winners.map((r, i) => (
                            <li key={i} className="animal-item">
                                🐾 {r.winner?.name ?? 'Animal desconocido'}
                            </li>
                        ))}
                    </ul>
                </div>
            ) : (
                <p className="done-message">
                    No obtuviste animales esta vez. ¡Intenta de nuevo!
                </p>
            )}
            <button className="spin-btn primary" onClick={onReset}>
                🔄 Jugar de nuevo
            </button>
        </div>
    );
};

// ─── AnimalSpinWheel ──────────────────────────────────────────────────────────
const AnimalSpinWheel = ({
    spinIndex,
    totalSpins,
    spinData,
    onDone,
}: {
    spinIndex: number;
    totalSpins: number;
    spinData: SpinResult | null;
    onDone: () => void;
}) => {
    const wheelSize = useWheelSize();
    const wheelRef = useRef<{ spin: (i: number) => void }>(null);
    const [spun, setSpun] = useState(false);
    const [result, setResult] = useState<'win' | 'lose' | null>(null);
    const [winner, setWinner] = useState<AnimalResponse | null>(null);

    const candidates = spinData?.candidates ?? [];
    const spinWinner = spinData?.winner ?? null;
    const segments = buildSegmentsFromCandidates(candidates, spinWinner);
    const winnerIndex = segments.findIndex((s) => s.isWinner);

    const handleSpin = () => {
        if (spun) return;
        setSpun(true);
        const idx = winnerIndex >= 0 ? winnerIndex : 0;
        wheelRef.current?.spin(idx);
    };

    const handleSpinEnd = () => {
        if (spinWinner != null) {
            setResult('win');
            setWinner(spinWinner);
        } else {
            setResult('lose');
        }
        setTimeout(() => onDone(), 2500);
    };

    return (
        <div className="animal-spin-screen">
            <div className="spin-counter">
                <span className="spin-counter-current">{spinIndex + 1}</span>
                <span className="spin-counter-sep"> / </span>
                <span className="spin-counter-total">{totalSpins}</span>
                <span className="spin-counter-label"> tiradas</span>
            </div>

            <h4 className="wheel-number">Tirada #{spinIndex + 1}</h4>

            <SpinWheel
                key={`spin-wheel-${spinIndex}-${wheelSize}`}
                ref={wheelRef}
                segments={segments}
                size={wheelSize}
                onSpinEnd={handleSpinEnd}
            />

            {!spun && (
                <button className="spin-btn primary" onClick={handleSpin}>
                    🎰 Girar tirada #{spinIndex + 1}
                </button>
            )}

            {result === 'win' && winner && (
                <div className="wheel-result-big win">
                    🎉 ¡<strong>{winner.name}</strong> es tuyo!
                </div>
            )}
            {result === 'lose' && (
                <div className="wheel-result-big empty">
                    😢 ¡Mala suerte en esta tirada!
                </div>
            )}
        </div>
    );
};

// ─── AnimalRoulette (principal) ───────────────────────────────────────────────
const AnimalRoulette = ({ onFinished, userBalance }: Props) => {
    const wheelSize = useWheelSize();
    const [phase, setPhase] = useState<Phase>('select');
    const [difficulty, setDifficulty] = useState<Difficulty>('facil');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [spinResults, setSpinResults] = useState<SpinResult[]>([]);
    const [totalSpins, setTotalSpins] = useState(0);
    const [currentSpin, setCurrentSpin] = useState(0);

    const canSpin = userBalance >= DIFFICULTY_PRICE[difficulty].price;

    const mainWheelRef = useRef<{ spin: (i: number) => void }>(null);
    const totalSpinsRef = useRef(0);
    const currentSpinRef = useRef(0);
    const spinResultsRef = useRef<SpinResult[]>([]);
    const mainSpinEndCallbackRef = useRef<() => void>(() => { });

    const segments = DIFFICULTY_SEGMENTS[difficulty];

    // ─── API ──────────────────────────────────────────────────────────────────
    const callDifficultyAPI = async (): Promise<RouletteApiResponse | null> => {
        setLoading(true);
        setError(null);
        try {
            const token = localStorage.getItem('token_casino');
            if (!token) { setError('No estás autenticado.'); return null; }
            const res = await fetch(
                `${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/AnimalRoullete?Dificultad=${difficulty}`,
                {
                    method: 'POST',
                    headers: {
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                }
            );
            return await res.json();
        } catch {
            setError('Error de conexión con el servidor');
            return null;
        } finally {
            setLoading(false);
        }
    };

    // ─── Spin principal ───────────────────────────────────────────────────────
    const handleMainSpin = async () => {
        if (loading) return;

        const data = await callDifficultyAPI();
        if (!data) return;

        let spins = 0;
        let results: SpinResult[] = [];

        if (data.totalSpins && Array.isArray(data.data)) {
            spins = data.totalSpins;
            results = data.data;
        } else if (Array.isArray(data.data) && data.data.length > 0) {
            spins = data.data.length;
            results = (data.data as any[]).map((a) => ({
                success: true,
                message: `¡Has obtenido a ${a.name}!`,
                candidates: [{ id: a.id, name: a.name }],
                winner: { id: a.id, name: a.name },
            }));
        } else {
            spins = 1;
            results = [{
                success: false,
                message: data.message ?? 'Sin suerte',
                candidates: [],
                winner: null,
            }];
        }

        totalSpinsRef.current = spins;
        currentSpinRef.current = 0;
        spinResultsRef.current = results;

        setTotalSpins(spins);
        setCurrentSpin(0);
        setSpinResults(results);

        mainSpinEndCallbackRef.current = () => {
            currentSpinRef.current = 0;
            setCurrentSpin(0);
            setPhase('result');
        };

        const targetIdx = segments.findIndex((s) => s.value === spins);
        const safeIdx = targetIdx >= 0 ? targetIdx : 0;

        setPhase('spinning');
        setTimeout(() => mainWheelRef.current?.spin(safeIdx), 300);
    };

    const handleMainSpinEnd = useCallback(() => {
        mainSpinEndCallbackRef.current();
    }, []);

    // ─── Tirada individual terminada ──────────────────────────────────────────
    const handleAnimalSpinDone = () => {
        const next = currentSpinRef.current + 1;
        const total = totalSpinsRef.current;

        if (next >= total) {
            setPhase('done');
            onFinished?.();
        } else {
            currentSpinRef.current = next;
            setCurrentSpin(next);
        }
    };

    // ─── Reset ────────────────────────────────────────────────────────────────
    const reset = () => {
        totalSpinsRef.current = 0;
        currentSpinRef.current = 0;
        spinResultsRef.current = [];
        mainSpinEndCallbackRef.current = () => { };
        setPhase('select');
        setError(null);
        setSpinResults([]);
        setTotalSpins(0);
        setCurrentSpin(0);
    };

    // ─── Render ───────────────────────────────────────────────────────────────
    return (
        <div className="roulette-wrapper">

            {/* ✅ FIX: título dentro del wrapper */}
            <h2 className="roulette-title">🎰 Ruleta de Animales</h2>

            {(phase === 'select' || phase === 'spinning') && (
                <div className="main-wheel-section">
                    <DifficultySelector
                        selected={difficulty}
                        onChange={(d: string) => setDifficulty(d as Difficulty)}
                        disabled={phase === 'spinning' || loading}
                    />

                    <div className="wheel-wrapper">
                        <SpinWheel
                            key={`main-wheel-${wheelSize}`}
                            ref={mainWheelRef}
                            segments={segments}
                            size={wheelSize}
                            onSpinEnd={handleMainSpinEnd}
                        />
                    </div>

                    {phase === 'select' && (
                        <>
                            {/* Aviso saldo insuficiente */}
                            {!canSpin && (
                                <p className="error-msg">
                                    💸 Saldo insuficiente. Necesitas{' '}
                                    <strong>
                                        ${DIFFICULTY_PRICE[difficulty].price.toLocaleString('es-ES')}
                                    </strong>{' '}
                                    para girar en dificultad{' '}
                                    <strong>{DIFFICULTY_PRICE[difficulty].label}</strong>.
                                    <br />
                                    Tu saldo actual:{' '}
                                    <strong>${userBalance.toLocaleString('es-ES')}</strong>
                                </p>
                            )}

                            {/* ✅ FIX: sin style inline — el CSS maneja todo */}
                            <button
                                className="spin-btn primary"
                                onClick={handleMainSpin}
                                disabled={loading || !canSpin}
                            >
                                {loading
                                    ? '⏳ Cargando...'
                                    : `🎰 ¡Girar Ruleta! — ${DIFFICULTY_PRICE[difficulty].label}: $${DIFFICULTY_PRICE[difficulty].price.toLocaleString('es-ES')}`
                                }
                            </button>

                            {/* User wallet balance display under the spin button */}
                            <div className="roulette-balance-display" style={{
                                marginTop: '12px',
                                fontSize: '18px',
                                fontWeight: 'bold',
                                color: '#ffd700',
                                backgroundColor: 'rgba(0, 0, 0, 0.4)',
                                padding: '8px 20px',
                                borderRadius: '20px',
                                border: '1px solid rgba(255, 215, 0, 0.3)',
                                display: 'inline-block',
                                textAlign: 'center',
                                backdropFilter: 'blur(4px)'
                            }}>
                                🪙 Tu Saldo: ${userBalance.toLocaleString('es-ES')}
                            </div>
                        </>
                    )}

                    {phase === 'spinning' && (
                        <p className="spinning-label">✨ Girando... ¡buena suerte!</p>
                    )}

                    {error && <p className="error-msg">⚠️ {error}</p>}
                </div>
            )}

            {phase === 'result' && (
                <AnimalSpinWheel
                    key={currentSpin}
                    spinIndex={currentSpin}
                    totalSpins={totalSpins}
                    spinData={spinResultsRef.current[currentSpin] ?? null}
                    onDone={handleAnimalSpinDone}
                />
            )}

            {phase === 'done' && (
                <DoneScreen
                    results={spinResultsRef.current}
                    onReset={reset}
                />
            )}
        </div>
    );
};

export default AnimalRoulette;