import { useRef, useState, useEffect } from "react";
import AnimalsUser from "../../components/AnimalsUser";
import '../../styles/EuropeanRoulette.css'
import BackButton from "../Components/BackButton";
import BetsTable from "../Components/BetsTable";
import ActiveDrinkEffects from "../Components/ActiveDrinkEffect";

import { useAudio } from '../../hooks/useAudio';
import musicaGame from '../../assets/Audios/casino_game_4.mp3';
import musicaIntro from '../../assets/Audios/casino_landing_soundtrack.mp3';
import ErrorPopup from "../Components/ErrorPopup";
export default function EuropeanRoulette() {
    const [animalSelected, setAnimalSelected] = useState<any | null>(null);
    const rouletteRef = useRef<HTMLDivElement>(null);
    const Api_URL = import.meta.env.VITE_BASE_URL
    const token = localStorage.getItem('token_casino')
    const ballRef = useRef<HTMLDivElement>(null)
    const [isSpinning, SetIsSpinning] = useState(false);
    const [resultPlay, setResultPlay] = useState(null);
    const [showResults, setShowResults] = useState(false);
    const [betError, setBetError] = useState<string | null>(null);

    const { reproducir, detener, silenciar } = useAudio();
    const [musicaMutada, setMusicaMutada] = useState<boolean>(
        () => localStorage.getItem('casino_music_muted') === 'true'
    );

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


    const animateRoulette = (winningNumber: number) => {
        if (!rouletteRef.current || !ballRef.current) return;
        const winningIndex = ROULETTE_WHEEL_ORDER.indexOf(winningNumber);
        const numberAngle = angle * winningIndex + angle / 2;
        const rouletteSpins = 20 * 360;
        const ballSpins = 24 * 360;
        const ballFinalAngle = -(ballSpins + (360 - numberAngle));

        rouletteRef.current.style.transition = "none";
        ballRef.current.style.transition = "none";
        rouletteRef.current.style.transform = "rotate(0deg)";
        ballRef.current.style.transform = "rotate(0deg)";

        void rouletteRef.current.offsetWidth;
        void ballRef.current.offsetWidth;

        rouletteRef.current.style.transition = "transform 10s cubic-bezier(0.17, 0.67, 0.12, 1)";
        rouletteRef.current.style.transform = `rotate(${rouletteSpins}deg)`;

        ballRef.current.style.transition = "transform 10s cubic-bezier(0.17, 0.67, 0.12, 1)";
        ballRef.current.style.transform = `rotate(${ballFinalAngle}deg)`;
    };

    const INITIAL_BET_NAMES = {
        selectedNumbers: [] as { number: string; animalName: string }[],
        redNumbers: "",
        blackNumbers: "",
        firstDozen: "",
        secondDozen: "",
        thirdDozen: "",
        firstHalf: "",
        secondHalf: "",
        firstRow: "",
        secondRow: "",
        thirdRow: "",
        evenNumbers: "",
        oddNumbers: "",
    };

    const [betNames, setBetNames] = useState(INITIAL_BET_NAMES);
    const INITIAL_BETS = {
        selectedNumbers: [],
        redNumbers: "00000000-0000-0000-0000-000000000000",
        blackNumbers: "00000000-0000-0000-0000-000000000000",
        firstDozen: "00000000-0000-0000-0000-000000000000",
        secondDozen: "00000000-0000-0000-0000-000000000000",
        thirdDozen: "00000000-0000-0000-0000-000000000000",
        firstHalf: "00000000-0000-0000-0000-000000000000",
        secondHalf: "00000000-0000-0000-0000-000000000000",
        firstRow: "00000000-0000-0000-0000-000000000000",
        secondRow: "00000000-0000-0000-0000-000000000000",
        thirdRow: "00000000-0000-0000-0000-000000000000",
        evenNumbers: "00000000-0000-0000-0000-000000000000",
        oddNumbers: "00000000-0000-0000-0000-000000000000"
    }
    const [bets, setBets] = useState(INITIAL_BETS)
    const fetchRouletteEuropean = async () => {
        if (!hasBets()) {
            setBetError("⚠️ You must place at least one bet before spinning!");
            setTimeout(() => setBetError(null), 3000);
            return;
        }

        // ✅ Validación: no debe haber un animal seleccionado sin colocar
        if (animalSelected !== null) {
            setBetError(`⚠️ Place ${animalSelected.name} on a bet before spinning!`);
            setTimeout(() => setBetError(null), 3000);
            return;
        }

        try {


            const response = await fetch(`${Api_URL}/api/CasinoGames/EuropeanRoullete`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(bets)

            })

            const data = await response.json();
            SetIsSpinning(true);
            animateRoulette(data.positionEuropeanRoulette);
            setTimeout(() => {
                setBets({ ...INITIAL_BETS, selectedNumbers: [] });
                SetIsSpinning(false);
                setResultPlay(data);
                setShowResults(true);
                setTimeout(() => {
                    setShowResults(false)
                }, 7000)
                setBetNames(INITIAL_BET_NAMES)
                setBets(INITIAL_BETS)
            }, 10000);

        } catch (error) {
            console.error("bad request, error from the server" + error)
        }
    }


    const hasBets = (): boolean => {
        const NULL_ID = "00000000-0000-0000-0000-000000000000";

        const hasCategoryBet = Object.entries(bets)
            .filter(([key]) => key !== "selectedNumbers")
            .some(([_, value]) => value !== NULL_ID);

        const hasNumberBet = bets.selectedNumbers.length > 0;

        return hasCategoryBet || hasNumberBet;
    };


    const betNumberSelected = (key: string) => {
        if (animalSelected == null) return;
        if (isAnimalAlreadyBet(animalSelected.id, key)) {
            setBetError(`⚠️ ${animalSelected.name} is already placed in another bet!`);
            setTimeout(() => setBetError(null), 3000);
            return;
        }
        bets.selectedNumbers = bets.selectedNumbers.filter(
            (entry) => !Object.keys(entry).includes(key)
        );
        bets.selectedNumbers.push({ [key]: animalSelected.id });


        setBetNames(prev => ({
            ...prev,
            selectedNumbers: [
                ...prev.selectedNumbers.filter(e => e.number !== key),
                { number: key, animalName: animalSelected.name }
            ]
        }));

        setAnimalSelected(null);
    };
    const isAnimalAlreadyBet = (animalId: string, excludeNumberKey?: string): boolean => {
        const NULL_ID = "00000000-0000-0000-0000-000000000000";


        const alreadyInCategory = Object.entries(bets)
            .filter(([key, value]) => key !== "selectedNumbers" && value !== NULL_ID)
            .some(([_, value]) => value === animalId);



        const alreadyInNumbers = bets.selectedNumbers
            .filter((entry) => !Object.keys(entry).includes(excludeNumberKey ?? ""))
            .some((entry) => Object.values(entry)[0] === animalId);

        return alreadyInCategory || alreadyInNumbers;
    };
    const betSelected = (betType: string) => {
        if (animalSelected == null) return;

        if (isAnimalAlreadyBet(animalSelected.id, betType)) {
            setBetError(`⚠️ ${animalSelected.name} is already placed in another bet!`);
            setTimeout(() => setBetError(null), 3000);
            return;
        }
        const betMap: Record<string, keyof typeof INITIAL_BETS> = {
            "1st 12": "firstDozen",
            "2st 12": "secondDozen",
            "3st 12": "thirdDozen",
            "1 to 18": "firstHalf",
            "Even": "evenNumbers",
            "🟥": "redNumbers",
            "⬛": "blackNumbers",
            "Odd": "oddNumbers",
            "19 to 36": "secondHalf",
            "1 Row": "firstRow",
            "2 Row": "secondRow",
            "3 Row": "thirdRow",
        };

        const key = betMap[betType];
        if (!key) return;

        setBets(prev => ({ ...prev, [key]: animalSelected.id }));
        setBetNames(prev => ({ ...prev, [key]: animalSelected.name }));
        setAnimalSelected(null);

    };

    const selectedAnimal = (animal: any) => setAnimalSelected(animal)
    const ROULETTE_WHEEL_ORDER = [
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34,
        6, 27, 13, 36, 11, 30, 8, 23, 10, 5,
        24, 16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    ];
    const angle = 360 / ROULETTE_WHEEL_ORDER.length;
    const radius = innerWidth > 500 ? 200 : 130;

    const betOptions = [["1st 12", "2st 12", "3st 12"], ["1 to 18", "Even", "🟥", "⬛", "Odd", "19 to 36"]];

    const getColor = (index: number) => {
        if (index === 0) return "#006400";
        const numberPosition: number = ROULETTE_WHEEL_ORDER.indexOf(index);
        return numberPosition % 2 === 0 ? "#1a1a1a" : "#c0392b";

    }
    return (
        <>
            <BackButton />

            <main id="european-roulette">
                <div id="table">
                    <button className="btn-mute" onClick={manejarToggleMute}>
                        {musicaMutada ? '🔇' : '🔊'}
                    </button>
                    <div style={{ position: "fixed", top: "-20rem", width: `${radius * 2}px`, height: `${radius * 2}px` }}>

                        <aside id="roulette" ref={rouletteRef} style={{
                            position: "absolute",
                            top: 0,
                            left: 0,
                            width: `${radius * 2}px`,
                            height: `${radius * 2}px`,
                            borderRadius: "50%",
                            background: (() => {
                                const stops = Array.from({ length: ROULETTE_WHEEL_ORDER.length }).map((_, i) => {
                                    const start = angle * i;
                                    const end = angle * (i + 1);
                                    return `${getColor(ROULETTE_WHEEL_ORDER[i])} ${start}deg ${end}deg`;
                                });
                                return `conic-gradient(${stops.join(", ")})`;
                            })(),
                        }}>
                            {Array.from({ length: ROULETTE_WHEEL_ORDER.length }).map((_, index) => {
                                const midAngle = angle * index + angle / 2;
                                const midRad = (midAngle - 90) * (Math.PI / 180);
                                const x = radius + (radius * 0.85) * Math.cos(midRad);
                                const y = radius + (radius * 0.85) * Math.sin(midRad);

                                return (
                                    <div
                                        key={index}
                                        style={{
                                            position: "absolute",
                                            left: `${x}px`,
                                            top: `${y}px`,
                                            transform: `translate(-50%, -50%) rotate(${angle * index + angle / 2}deg)`,
                                            color: "white",
                                            fontSize: radius / 10,
                                            fontWeight: "bold",
                                            pointerEvents: "none",
                                        }}
                                    >
                                        {ROULETTE_WHEEL_ORDER[index]}
                                    </div>
                                );
                            })}

                            <div style={{
                                position: "absolute",
                                width: "40px",
                                height: "40px",
                                borderRadius: "50%",
                                background: "#006400",
                                top: "50%",
                                left: "50%",
                                transform: "translate(-50%, -50%)",
                                border: "2px solid gold",
                                zIndex: 2,
                            }} />
                        </aside>


                        <div
                            ref={ballRef}
                            style={{
                                position: "absolute",
                                top: 0,
                                left: 0,
                                width: `${radius * 2}px`,
                                height: `${radius * 2}px`,
                                borderRadius: "50%",
                                pointerEvents: "none",
                                zIndex: 3,
                                transformOrigin: "center center",
                            }}
                        >
                            <div style={{
                                position: "absolute",
                                top: "4px",
                                left: "50%",
                                transform: "translateX(-50%)",
                                width: "14px",
                                height: "14px",
                                borderRadius: "50%",
                                background: "radial-gradient(circle at 35% 35%, #ffffff, #aaaaaa)",
                                boxShadow: "0 2px 6px rgba(0,0,0,0.6)",
                            }} />
                        </div>

                    </div>

                    <table id="numbers_table">
                        <tbody>
                            {Array.from({ length: 3 }).map((_, rowIndex) => (
                                <tr key={rowIndex + "index"}>

                                    {rowIndex === 0 && (
                                        <td
                                            rowSpan={3}
                                            style={{
                                                background: getColor(0),
                                                color: "white",
                                                textAlign: "center",
                                                fontWeight: "bold",
                                                fontSize: "40px",
                                                padding: "1rem",
                                                verticalAlign: "middle",
                                            }}
                                            onClick={() => betNumberSelected("0")}
                                        >
                                            0
                                        </td>
                                    )}

                                    {Array.from({ length: 13 }).map((_, colIndex) => {
                                        const num = (3 - rowIndex) + (colIndex * 3);
                                        return (
                                            colIndex < 12 ? <td
                                                key={colIndex + "colIndex"}
                                                style={{
                                                    background: getColor(num),
                                                    color: "white",
                                                    textAlign: "center",
                                                    fontWeight: "bold",
                                                    padding: " 1rem ",
                                                    cursor: "pointer",
                                                    margin: "0"
                                                }}
                                                onClick={() => betNumberSelected(num.toString())}
                                            >
                                                {num}
                                            </td> :
                                                <td
                                                    key={colIndex + "colIndex"}
                                                    style={{

                                                        backgroundColor: 'rgb(3, 70, 8)',
                                                        color: "white",
                                                        textAlign: "center",
                                                        fontWeight: "bold",
                                                        padding: " 1rem 0.75rem ",
                                                        cursor: "pointer",
                                                        margin: "0"
                                                    }}
                                                    onClick={() => betSelected((3 - rowIndex) + " Row")}
                                                >
                                                    {(3 - rowIndex) + " Row"}
                                                </td>
                                        );
                                    })}

                                </tr>
                            ))}
                            {Array.from({ length: betOptions.length }).map((_, rowIndex) => (
                                <tr key={rowIndex + "betsTypes"}>

                                    {Array.from({ length: betOptions[rowIndex].length }).map((_, columnIndex) => (
                                        <td
                                            key={columnIndex + "valueBet"}
                                            colSpan={rowIndex > 0 ? (betOptions[rowIndex][columnIndex] === "🟥" || betOptions[rowIndex][columnIndex] === "⬛") ? 3 : 2 : 5}
                                            style={{
                                                backgroundColor: 'rgb(3, 70, 8)',
                                                color: "white",
                                                textAlign: "center",
                                                fontWeight: "bold",
                                                fontSize: "35px",
                                                padding: " 1rem 0.75rem ",
                                                cursor: "pointer",
                                                margin: "0"
                                            }}
                                            onClick={() => betSelected(betOptions[rowIndex][columnIndex])}
                                        >{betOptions[rowIndex][columnIndex]}</td>
                                    ))}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    <BetsTable bets={bets} betNames={betNames} />

                    <button
                        id="start-button"
                        onClick={fetchRouletteEuropean}
                        disabled={isSpinning}
                        style={{
                            opacity: isSpinning || !hasBets() ? 0.5 : 1,
                            cursor: isSpinning || !hasBets() ? "not-allowed" : "pointer",
                        }}
                    >
                        {isSpinning ? "🎰 Spinning..." : "🎲 Play"}
                    </button>
                    {showResults && <legend id="resultRoulette">
                        <p>{"Bet: " + (resultPlay.bet | 0) + "€"}</p>
                        <p>{"Reward: " + (resultPlay.moneyBack | 0) + " €"}</p>
                        <p>{(resultPlay.resultMessage)}</p>
                    </legend>}
                </div>
            </main>
            <AnimalsUser betAnimal={selectedAnimal} />
            {betError && <ErrorPopup
                message={betError}
                onClose={() => setBetError(null)}
            />}
        </>
    );
}