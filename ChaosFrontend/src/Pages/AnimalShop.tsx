/* eslint-disable react-hooks/immutability */
import { useEffect, useRef, useState } from 'react';
import imagenFondo from '../assets/casino_shop.png';
import { useNavigate } from 'react-router-dom';
import AnimalToBetInGame from '../components/AnimalToBetInGame';
import AnimalRoulette from '../components/AnimalRoulette/index.tsx';
import '../styles/AnimalShop.css'
import '../styles/LoadingPages.css'
import LoadingHamster from './Components/LoadingHamster.tsx';
import BackButton from './Components/BackButton.tsx';
import gotera from '../assets/Audios/gotera.mp3';
import casinoMusic from '../assets/Audios/casino_landing_soundtrack.mp3';
import MusicButton from './Components/MusicButton.tsx';

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
    WHALE: 'WHALE'
} as const;

type AnimalType = typeof AnimalType[keyof typeof AnimalType];

interface ApiResponse {
    success: boolean;
    message: string;
    data: Animal[] | null;
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

function AnimalShop() {
    const [userBalance, setUserBalance] = useState<number>(0);
    const navigate = useNavigate();
    const [animals, setAnimals] = useState<Animal[]>([]);
    const [animalsToSell, setAnimalsToSell] = useState<Animal[]>([]);
    const [selectedAnimal, setSelectedAnimal] = useState<Animal | null>(null);
    const [selectedAnimalToSell, setSelectedAnimalToSell] = useState<Animal | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [errorToSell, setErrorToSell] = useState<string | null>(null);
    const [correct, setCorrect] = useState<string | null>(null);
    const [correctInSell, setCorrectInSell] = useState<string | null>(null);
    const [isLoadingAnimals, setIsLoadingAnimals] = useState(true);
    const [customName, setCustomName] = useState('');
    const [musicaSonando, setMusicaSonando] = useState(false);
    const audioRef = useRef<HTMLAudioElement>(null);
    const audioRef2 = useRef<HTMLAudioElement>(null);
    const audioCtxRef = useRef<AudioContext | null>(null);
    const sourceRef = useRef<MediaElementAudioSourceNode | null>(null);
    const reverbRef = useRef<ConvolverNode | null>(null);
    const filterRef = useRef<BiquadFilterNode | null>(null);
    const gainRef = useRef<GainNode | null>(null);
    const isConnectedRef = useRef(false);

    const createReverb = (ctx: AudioContext): ConvolverNode => {
        const convolver = ctx.createConvolver();
        const sampleRate = ctx.sampleRate;
        const duration = 3;
        const decay = 3.0;
        const length = sampleRate * duration;
        const impulse = ctx.createBuffer(2, length, sampleRate);

        for (let channel = 0; channel < 2; channel++) {
            const data = impulse.getChannelData(channel);
            for (let i = 0; i < length; i++) {
                data[i] = (Math.random() * 2 - 1) * Math.pow(1 - i / length, decay);
            }
        }

        convolver.buffer = impulse;
        return convolver;
    };

    const applyBasementEffect = () => {
        if (!audioRef2.current || isConnectedRef.current) return;

        const ctx = new AudioContext();
        audioCtxRef.current = ctx;

        // 1️⃣ Fuente — conecta el <audio> al contexto
        const source = ctx.createMediaElementSource(audioRef2.current);
        sourceRef.current = source;

        // 2️⃣ Lowpass filter — corta agudos, suena amortiguado/lejano
        const filter = ctx.createBiquadFilter();
        filter.type = 'lowpass';
        filter.frequency.value = 800;   // Hz — más bajo = más apagado (prueba entre 600-1200)
        filter.Q.value = 0.8;
        filterRef.current = filter;

        // 3️⃣ Reverb — eco de habitación húmeda
        const reverb = createReverb(ctx);
        reverbRef.current = reverb;

        // 4️⃣ Gain — volumen final
        const gain = ctx.createGain();
        gain.gain.value = 0.35;         // más bajo = más lejano
        gainRef.current = gain;

        // 5️⃣ Cadena: source → filter → reverb → gain → salida
        source.connect(filter);
        filter.connect(reverb);
        reverb.connect(gain);
        gain.connect(ctx.destination);

        isConnectedRef.current = true;
    };

    const toggleMusica = () => {
        if (!audioRef.current || !audioRef2.current) return;

        if (musicaSonando) {
            audioRef.current.pause();
            audioRef2.current.pause();
            audioCtxRef.current?.suspend();
        } else {
            applyBasementEffect();

            audioCtxRef.current?.resume();

            audioRef.current.volume = 0.2;
            audioRef.current.play();

            audioRef2.current.volume = 1.0;
            audioRef2.current.play();
        }

        setMusicaSonando(!musicaSonando);
    };



    const fetchShopAnimals = async () => {
        try {
            setError(null);

            const token = localStorage.getItem('token_casino');

            if (!token) {
                setError('No estás autenticado. Redirigiendo...');
                setTimeout(() => navigate('/'), 2000);
                return;
            }

            const response = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/GetShopAnimals`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            const values = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/GetValues`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.status === 401 || values.status === 401) {
                setError('Tu sesión ha expirado. Por favor inicia sesión nuevamente.');
                localStorage.removeItem('token_casino');
                setTimeout(() => navigate('/'), 2000);
                return;
            }

            if (!response.ok || !values.ok) {
                const errorData = await response.json().catch(() => null);
                throw new Error(errorData?.message || `Error ${response.status}: No se pudieron cargar los animales`);
            }

            const apiResponse: ApiResponse = await response.json();
            const apiValues = await values.json();

            const animalesConValor = apiResponse.data!.map(animal => ({
                ...animal,
                value: apiValues[animal.typeAnimal] ?? animal.value,
                rarity: false
            }));

            if (!apiResponse.success || !animalesConValor) {
                setError(apiResponse.message || 'No tienes animales disponibles');
                setAnimals([]);
                return;
            }

            setAnimals(animalesConValor);

            if (animalesConValor.length === 0) {
                setError('No tienes animales disponibles para apostar. ¡Compra algunos en la tienda!');
            }

        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : 'Error al cargar los animales';
            setError(errorMessage);
            console.error('Error fetching animals:', err);
        }
    };

    const fetchUserBalance = async () => {
        const token = localStorage.getItem('token_casino');
        if (!token) return;

        const response = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/User/GetMyUser`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) return;

        const data = await response.json();
        setUserBalance(data.data.wallet);
    };

    const fetchUserAnimals = async () => {
        try {
            setError(null);

            const token = localStorage.getItem('token_casino');

            if (!token) {
                setError('No estás autenticado. Redirigiendo...');
                setTimeout(() => navigate('/'), 2000);
                return;
            }

            const myanimalsResponse = await fetch(
                `${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/GetAnimalByOwnerId`,
                {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            if (myanimalsResponse.status === 401) {
                setError('Tu sesión ha expirado. Por favor inicia sesión nuevamente.');
                localStorage.removeItem('token_casino');
                setTimeout(() => navigate('/'), 2000);
                return;
            }

            if (!myanimalsResponse.ok) {
                const errorData = await myanimalsResponse.json().catch(() => null);
                throw new Error(errorData?.message || `Error ${myanimalsResponse.status}: No se pudieron cargar los animales`);
            }

            const apiAnimals = await myanimalsResponse.json();


            if (!apiAnimals?.data) {
                setErrorToSell('No tienes animales disponibles');
                setAnimalsToSell([]);
                return;
            }


            setAnimalsToSell(apiAnimals.data);

        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : 'Error al cargar los animales';
            setError(errorMessage);
            console.error('Error fetching animals:', err);
        }
    };
    useEffect(() => {
        const loadData = async () => {
            const token = localStorage.getItem('token_casino');
            await Promise.all([
                fetchUserAnimals(),
                fetchShopAnimals(),
                fetchUserBalance()
            ]);

            if (!token) {
                setError('No estás autenticado. Por favor inicia sesión nuevamente.');
                navigate('/login');
                return;
            }

            try {


                await Promise.all([
                    fetchUserAnimals(),
                    fetchShopAnimals()
                ]);


            } catch (err) {
                console.error(err);
            } finally {
                setIsLoadingAnimals(false);
            }
            return () => {
                audioCtxRef.current?.close();
            };
        };

        loadData();
    }, []);

    const handleSellAnimal = async () => {
        const token = localStorage.getItem('token_casino');
        try {
            if (!selectedAnimalToSell) {
                setErrorToSell('Selecciona un animal primero');
                return;
            }

            const response = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/Sell`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(selectedAnimalToSell.id)
            });

            if (!response.ok) {
                const errorData = await response.text();
                console.error("ERROR BACKEND:", errorData);
                throw new Error('Error al vender');
            }

            await fetchUserAnimals();
            await fetchUserBalance(); // Update user balance
            const result = await response.text();
            console.log(result);
            setSelectedAnimalToSell(null);
            setCorrectInSell("Vendido!")

        } catch (err) {
            console.error(err);
            setErrorToSell('Error al vender');
        }
    };

    const handleBuyAnimal = async () => {
        const token = localStorage.getItem('token_casino');
        try {
            if (!selectedAnimal) {
                setError('Selecciona un animal primero');
                return;
            }

            const request = {
                animalId: selectedAnimal.id,
                name: customName
            };

            const response = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/BuyAnimal`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(request)
            });

            if (!response.ok) {
                const errorData = await response.text();
                console.error("ERROR BACKEND:", errorData);
                throw new Error('Error al comprar');
            }

            const result = await response.text();
            await fetchShopAnimals();
            await fetchUserAnimals();
            await fetchUserBalance(); // Update user balance
            console.log(result);
            setSelectedAnimal(null);
            setCustomName("");
            setCorrect("Comprado!")

        } catch (err) {
            console.error(err);
            setError('Error en la compra');
        }
    };

    const handleRouletteFinished = () => {
        fetchUserAnimals();
        fetchShopAnimals();
        fetchUserBalance();
    };

    if (isLoadingAnimals) {
        return (
            <div className="fullscreen-loader">
                <img
                    src={imagenFondo}
                    alt="Fondo del corral"
                    className="imagen-fondo"
                />
                <LoadingHamster />
            </div>
        );
    }
    return (
        <div className="lienzo-corral">

            <MusicButton
                playing={musicaSonando}
                onToggle={toggleMusica}
                biome="Legend"
            />

            <BackButton />
            <audio ref={audioRef} src={gotera} loop />
            <audio ref={audioRef2} src={casinoMusic} loop />
            <img
                src={imagenFondo}
                alt="Fondo del corral"
                className="imagen-fondo"
            />

            <div className="coinflip-content">

                {/* ── IZQUIERDA — Comprar ── */}
                <div className="left-panel">
                    <AnimalToBetInGame
                        animals={animals}
                        selectedAnimal={selectedAnimal}
                        onSelectAnimal={setSelectedAnimal}
                        error={error}
                        isForBet={false}
                        correct={correct}
                    />
                    {/* ✅ Siempre renderizado, solo se oculta visualmente */}
                    <div className={`buy-panel ${!selectedAnimal ? 'buy-panel--hidden' : ''}`}>
                        <input
                            type="text"
                            placeholder="Nombre opcional"
                            value={customName}
                            onChange={(e) => setCustomName(e.target.value)}
                            className="buy-input"
                        />
                        <button
                            onClick={handleBuyAnimal}
                            className="buy-button"
                        >
                            Comprar Animal
                        </button>
                    </div>
                </div>

                {/* ── CENTRO — Ruleta 🎰 ── */}
                <div className="center-panel">
                    <AnimalRoulette
                        onFinished={handleRouletteFinished}
                        userBalance={userBalance} // 👈
                    />
                </div>

                {/* ── DERECHA — Vender ── */}
                <div className="right-panel">
                    <AnimalToBetInGame
                        animals={animalsToSell}
                        selectedAnimal={selectedAnimalToSell}
                        onSelectAnimal={setSelectedAnimalToSell}
                        error={errorToSell}
                        isForBet={false}
                        isForSale={true}
                        correct={correctInSell}
                    />
                    {/* ✅ Siempre renderizado, solo se oculta visualmente */}
                    <div className={`buy-panel ${!selectedAnimalToSell ? 'buy-panel--hidden' : ''}`}>
                        <button
                            onClick={handleSellAnimal}
                            className="sell-button"
                        >
                            Vender Animal
                        </button>
                    </div>
                </div>

            </div>
        </div>
    );
}

export default AnimalShop;