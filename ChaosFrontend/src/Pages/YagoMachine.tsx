import "../styles/YagoMachine.css";
import { useState, useEffect } from "react";
import AnimalsUser from "../components/AnimalsUser";
import { useSlotMachine } from "./Components/useSlotMachine";
import MachineConfig from "./Components/MachineConfig";
import BackButton from "./Components/BackButton";
import ActiveDrinkEffects from "./Components/ActiveDrinkEffect";

import ErrorPopup from "./Components/ErrorPopup";
import { useAudio } from '../hooks/useAudio';
import musicaGame from '../assets/Audios/casino_game_4.mp3';
import musicaIntro from '../assets/Audios/casino_landing_soundtrack.mp3';

interface SlotSymbol {
    id: string;
    symbolName: string;
    symbolCode: string;
    rarity: string;
    baseValue: number;
    isActive: boolean;
    createdAt: string;
    slotGameConfigId: string;
}

interface SlotGameConfig {
    id: string;
    machineName: string;
    multiplier: number;
    numberOfReels: number;
    numberOfRows: number;
    paylines: number;
}

interface GameResult {
    tragaperras: string[][][];
    jackpots: any[];
    totalReward: number;
}

export default function YagoMachine() {
    const [slotGameConf, setSlotGameConf] = useState<SlotGameConfig | null>(null);
    const [symbols, setSymbols] = useState<SlotSymbol[] | null>(null);
    const [animalSelected, setAnimalSelected] = useState<any | null>(null);
    const [gameResult, setGameResult] = useState<GameResult | null>(null);
    const [currentRound, setCurrentRound] = useState<number>(0);
    const [gameFinished, setGameFinished] = useState<boolean>(false);
    const [betPlaced, setBetPlaced] = useState<boolean>(false);

    const [spinError, setSpinError] = useState<string | null>(null);

    const { reproducir, detener, silenciar } = useAudio();
    const [musicaMutada, setMusicaMutada] = useState<boolean>(
        () => localStorage.getItem('casino_music_muted') === 'true'
    );

    const Api_URL = import.meta.env.VITE_BASE_URL;
    const token = localStorage.getItem('token_casino');

    const { reelAngles, isSpinning, startSpinning, stopSpinning } =
        useSlotMachine(symbols, slotGameConf);

    const selectConfig = (config: any) => setSlotGameConf(config);
    const selectAnimal = (animal: any) => setAnimalSelected(animal);

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

    const fetchSymbols = async () => {
        if (!token) return;
        try {
            const response = await fetch(`${Api_URL}/api/CasinoGames/Symbols`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            });
            if (response.ok) {
                const data: SlotSymbol[] = await response.json();
                setSymbols(data);
            }
        } catch (error) {
            console.error("Error fetching symbols:", error);
        }
    };

    const fetchResultGame = async () => {
        if (!token || !slotGameConf || !animalSelected) return;

        const requestBody = {
            gameName: slotGameConf.machineName,
            animalId: animalSelected?.id ?? animalSelected,
            headOrTail: "head"
        };

        try {
            const response = await fetch(`${Api_URL}/api/CasinoGames/TragaPerrillas`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error("Error del servidor:", errorText);
                return;
            }

            const data: GameResult = await response.json();
            console.log(" Resultado recibido:", data);

            setGameResult(data);
            setCurrentRound(0);
            setGameFinished(false);
            setBetPlaced(true);

        } catch (error) {
            console.error("Error:", error);
        }
    };


    const handleSpin = () => {
        if (isSpinning || !slotGameConf) return;
        if (!betPlaced || !gameResult) {
            setSpinError("⚠️ You must place a bet before spinning!");
            setTimeout(() => setSpinError(null), 3000);
            return; // 🚫 Corta aquí, no gira
        }
        if (!betPlaced || !gameResult) {
            startSpinning(slotGameConf.numberOfReels, slotGameConf.numberOfRows);
            setTimeout(() => {
                stopSpinning(
                    Array.from({ length: slotGameConf.numberOfReels }).map(() =>
                        symbols?.map(s => s.symbolCode) ?? []
                    )
                );
            }, 2000);
            return;
        }


        if (currentRound >= gameResult.tragaperras.length) {
            setGameFinished(true);
            return;
        }

        const tirada = gameResult.tragaperras[currentRound];

        const byReel: string[][] = Array.from({ length: slotGameConf.numberOfReels }).map((_, reelIndex) =>
            tirada.map(fila => fila[reelIndex])
        );

        const nextRound = currentRound + 1;

        startSpinning(slotGameConf.numberOfReels, slotGameConf.numberOfRows);

        setTimeout(() => {
            stopSpinning(byReel);
            setCurrentRound(nextRound);

            if (nextRound >= gameResult.tragaperras.length) {
                const lastReelDelay = (slotGameConf.numberOfReels - 1) * 300 + 800 + 100;
                setTimeout(() => setGameFinished(true), lastReelDelay);
            }
        }, 1500);
    };

    useEffect(() => {
        fetchSymbols();
    }, []);

    const totalRounds = gameResult?.tragaperras.length ?? 0;

    return (<>
        <ActiveDrinkEffects disabled={true} />

        <div className="yago-machine-wrapper">

            <BackButton />

            <div className="stats_machine">
                <button className="btn-mute" onClick={manejarToggleMute}>
                    {musicaMutada ? '🔇' : '🔊'}
                </button>
                <p>🎰 Machine: {slotGameConf ? slotGameConf.machineName : "Not Selected"}</p>
                <p>🐾 Animal: {animalSelected ? animalSelected.name : "Not Selected"}</p>
                {betPlaced && !gameFinished && (
                    <p>🎲 Tirada: {currentRound} / {totalRounds}</p>
                )}
            </div>


            <div className="reel-window"
                style={{
                    "--row-height": "84px",
                    "--number-of-rows": slotGameConf?.numberOfRows,
                } as React.CSSProperties}
            >
                <div className="reel-scene">
                    {slotGameConf && symbols && symbols.length > 0 && (() => {
                        const n = symbols.length;
                        const anglePerSymbol = 360 / n;

                        // Calcula el tamaño de cada celda según cuántas filas/reels hay
                        // para que quepan en pantalla
                        const maxHeight = window.innerHeight * 0.55; // 55% de la pantalla
                        const maxWidth = window.innerWidth * 0.55;   // 55% de la pantalla

                        const cellByHeight = Math.floor(maxHeight / slotGameConf.numberOfRows) - 40;
                        const cellByWidth = Math.floor(maxWidth / slotGameConf.numberOfReels) - 80;

                        // El tamaño de celda es el menor de los dos para que quepa en ambos ejes
                        const cellSize = Math.min(cellByHeight, cellByWidth, 80);
                        const radius = Math.round(cellSize / (2 * Math.tan(Math.PI / n)));

                        return Array.from({ length: slotGameConf.numberOfReels }).map((_, reelIndex) => (
                            <div key={reelIndex} className="reel-column">
                                {Array.from({ length: slotGameConf.numberOfRows }).map((_, rowIndex) => (
                                    <div
                                        key={rowIndex}
                                        className="Reel"
                                        style={{
                                            width: `${cellSize}px`,
                                            height: `${cellSize}px`,
                                            transform: `rotateX(${reelAngles[reelIndex]?.[rowIndex] ?? 0}deg)`,
                                            transition: "none",
                                        }}
                                    >
                                        {symbols.map((symbol, symbolIndex) => {
                                            const itemAngle = anglePerSymbol * symbolIndex;
                                            return (
                                                <div
                                                    key={symbol.id}
                                                    className="reel-item"
                                                    style={{
                                                        transform: `rotateX(${itemAngle}deg) translateZ(${radius}px)`,
                                                        fontSize: `${cellSize * 0.45}px`,
                                                    }}
                                                >
                                                    {symbol.symbolCode}
                                                </div>
                                            );
                                        })}
                                    </div>
                                ))}
                            </div>
                        ));
                    })()}
                </div>
            </div>
            {gameFinished && (
                <div style={{
                    position: "absolute",
                    bottom: "18rem",
                    left: "50%",
                    transform: "translateX(-50%)",
                    textAlign: "center",
                    background: "rgba(0,0,0,0.8)",
                    padding: "1rem 2rem",
                    borderRadius: "1rem",
                    color: "white"
                }}>
                    <h2>🏁 Partida Terminada</h2>
                    <p>💰 Recompensa Total: <strong>{gameResult?.totalReward}</strong></p>
                    <p>🎯 Jackpots: <strong>{gameResult?.jackpots.length ?? 0}</strong></p>
                    {(gameResult?.jackpots.length ?? 0) > 0 && (
                        <p>🎉 ¡¡JACKPOT!!</p>
                    )}
                </div>
            )}


            <button id="spin"
                onClick={handleSpin}
                disabled={isSpinning || (betPlaced && gameFinished)}
            >
                {isSpinning
                    ? "Spining"
                    : betPlaced
                        ? `Spin (${currentRound}/${totalRounds})`
                        : "Spin"}
            </button>

            {/* ✅ Mensaje de error */}
            {spinError && (
                <ErrorPopup
                    message={spinError}
                    onClose={() => setSpinError(null)}
                />
            )}

            <button
                id="bet"
                style={{
                    opacity: (!slotGameConf || !animalSelected) ? 0.5 : 1
                }}
                onClick={() => {
                    fetchResultGame();
                    setGameFinished(false);
                }}
                disabled={!slotGameConf || !animalSelected || isSpinning}
            >
                Bet
            </button>
            <MachineConfig selectedMachine={selectConfig} />
            <AnimalsUser betAnimal={selectAnimal} />
        </div>
    </>);
}