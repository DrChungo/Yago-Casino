/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-useless-assignment */
import { useEffect, useRef, useState } from 'react';
import '../../styles/Farm.Modules.css';
import '../../styles/LoadingPages.css';
import imagenFondo from '../../assets/BiomasBG/casino_ocean.png';
import { chequearColisionRectangulos, limite } from '../../AnimalMovement/OceanCollisions';
import OceanMusic from '../../assets/Audios/ocean_soundtrack.m4a';
import { useNavigate } from "react-router-dom";
import LoadingHamster from '../Components/LoadingHamster';
import BackButton from '../Components/BackButton';
import { getAnimalConfig } from '../../AnimalMovement/AnimalCollisions';
import BiomeSelector from '../Components/BiomeSelector';
import MusicButton from '../Components/MusicButton';

interface AnimalData {
    id: string;
    tipo: string;
    x: number;
    y: number;
    vx: number;
    vy: number;
    hitboxAncho: number;
    hitboxAlto: number;
    renderAncho: number;
    renderAlto: number;
    img: string;
    cooldownIA: number;
    raro: boolean;
    habitat: string;
}

// ✅ Sin obstáculos en Ocean por ahora
const obstaculos: { id: string; x: number; y: number; ancho: number; alto: number }[] = [];

export default function OceanPage() {

    const navigate = useNavigate();
    const [cargando, setCargando] = useState(true);
    const [animalesRender, setAnimalesRender] = useState<AnimalData[]>([]);

    const estadoJuego = useRef<{ animales: AnimalData[] }>({ animales: [] });
    const requestRef = useRef<number | undefined>(undefined);
    const token = localStorage.getItem('token_casino');

    const [musicaSonando, setMusicaSonando] = useState(false);
    const audioRef = useRef<HTMLAudioElement>(null);

    const toggleMusica = () => {
        if (audioRef.current) {
            if (musicaSonando) {
                audioRef.current.pause();
            } else {
                audioRef.current.volume = 0.2;
                audioRef.current.play();
            }
            setMusicaSonando(!musicaSonando);
        }
    };

    useEffect(() => {
        const iniciarJuego = async () => {
            setCargando(true);
            try {
                const response = await fetch('https://localhost:7101/api/Animal/GetAnimalByOwnerId', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    },
                });
                const habi = await fetch('https://localhost:7101/api/AnimalValueConfig/images', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok || !habi.ok) throw new Error("Error en el fetch");

                const jsonResult2 = await habi.json();
                const jsonResult = await response.json();
                console.log("Lo que llega de .NET:", jsonResult, jsonResult2);

                const arrayAnimales = jsonResult.data || jsonResult.item3 || [];
                const arrayHabi = jsonResult2 || [];

                if (arrayAnimales.length === 0) {
                    console.warn("No hay animales o el formato es incorrecto");
                    setCargando(false);
                    return;
                }

                if (arrayHabi.length === 0) {
                    console.warn("No cargo bien los animales");
                    setCargando(false);
                    return;
                }

                const configByType = new Map();
                arrayHabi.forEach((h: any) => {
                    const tipo = h.animalType || h.typeAnimal;
                    if (tipo) configByType.set(tipo.toLowerCase(), h);
                });

                const OceanAnimals: AnimalData[] = arrayAnimales.map((ani: any) => {
                    const tipoAnimal = ani.typeAnimal || ani.tipo || 'shark';
                    const config = configByType.get(tipoAnimal.toLowerCase());
                    const habitat = config?.habitat;
                    const esMecha = ani.rarity === true || ani.Rarity === true;

                    const config2 = getAnimalConfig(tipoAnimal);
                    const anchoHitbox = config2?.anchoHitbox ?? 40;
                    const altoHitbox = config2?.altoHitbox ?? 20;
                    const escala = config2?.escala ?? 3;

                    const limpiarSvg = (svg: string) =>
                        svg.replace(/<\?xml.*?\?>/, '').replace(/<!--.*?-->/g, '').trim();

                    const svgBd = esMecha ? config?.imageUrlMecha : config?.imageUrlNormal;
                    let imgSeleccionada = "";

                    if (svgBd && svgBd.trim() !== "") {
                        try {
                            const svgLimpio = limpiarSvg(svgBd);
                            const svgBase64 = btoa(unescape(encodeURIComponent(svgLimpio)));
                            imgSeleccionada = `data:image/svg+xml;base64,${svgBase64}`;
                        } catch (e) {
                            console.error("Error SVG:", e);
                        }
                    }

                    // ✅ Spawn dentro de los límites de OceanCollisions
                    const spawnX = limite.minX + Math.random() * (limite.maxX - limite.minX - anchoHitbox);
                    const spawnY = limite.minY + Math.random() * (limite.maxY - limite.minY - altoHitbox);

                    return {
                        id: ani.id,
                        tipo: tipoAnimal,
                        habitat,
                        x: spawnX,
                        y: spawnY,
                        hitboxAncho: anchoHitbox,
                        hitboxAlto: altoHitbox,
                        renderAncho: anchoHitbox * escala,
                        renderAlto: altoHitbox * escala,
                        vx: 0,
                        vy: 0,
                        img: imgSeleccionada,
                        cooldownIA: Math.random() * 60,
                        raro: esMecha
                    };
                }).filter((a: { habitat: string }) => a.habitat === 'Ocean');

                estadoJuego.current.animales = OceanAnimals;
                setAnimalesRender(OceanAnimals);
                setCargando(false);

                requestRef.current = requestAnimationFrame(bucleDeJuego);
            } catch (error) {
                console.error("Error al cargar los animales:", error);
                setCargando(false);
            }
        };

        function bucleDeJuego() {
            estadoJuego.current.animales.forEach(animal => {
                animal.cooldownIA--;

                if (animal.cooldownIA <= 0) {
                    if (Math.random() > 0.4) {
                        animal.vx = (Math.random() - 0.5) * 0.8;
                        animal.vy = (Math.random() - 0.5) * 0.5;
                    } else {
                        animal.vx = 0;
                        animal.vy = 0;
                    }
                    animal.cooldownIA = 90 + Math.random() * 120;
                }

                // ====== MOVER EN X ======
                animal.x += animal.vx;

                if (animal.x <= limite.minX) {
                    animal.x = limite.minX;
                    animal.vx = Math.abs(animal.vx);
                    animal.cooldownIA = 0;
                }
                if (animal.x + animal.hitboxAncho >= limite.maxX) {
                    animal.x = limite.maxX - animal.hitboxAncho;
                    animal.vx = -Math.abs(animal.vx);
                    animal.cooldownIA = 0;
                }

                for (const obs of obstaculos) {
                    const rect = { x: animal.x, y: animal.y, ancho: animal.hitboxAncho, alto: animal.hitboxAlto };
                    if (chequearColisionRectangulos(rect, obs)) {
                        const animalCentroX = animal.x + animal.hitboxAncho / 2;
                        const obsCentroX = obs.x + obs.ancho / 2;
                        if (animalCentroX < obsCentroX) {
                            animal.x = obs.x - animal.hitboxAncho;
                            animal.vx = -Math.abs(animal.vx);
                        } else {
                            animal.x = obs.x + obs.ancho;
                            animal.vx = Math.abs(animal.vx);
                        }
                        animal.cooldownIA = 0;
                        break;
                    }
                }

                // ====== MOVER EN Y ======
                animal.y += animal.vy;

                if (animal.y <= limite.minY) {
                    animal.y = limite.minY;
                    animal.vy = Math.abs(animal.vy);
                    animal.cooldownIA = 0;
                }
                if (animal.y + animal.hitboxAlto >= limite.maxY) {
                    animal.y = limite.maxY - animal.hitboxAlto;
                    animal.vy = -Math.abs(animal.vy);
                    animal.cooldownIA = 0;
                }

                for (const obs of obstaculos) {
                    const rect = { x: animal.x, y: animal.y, ancho: animal.hitboxAncho, alto: animal.hitboxAlto };
                    if (chequearColisionRectangulos(rect, obs)) {
                        const animalCentroY = animal.y + animal.hitboxAlto / 2;
                        const obsCentroY = obs.y + obs.alto / 2;
                        if (animalCentroY < obsCentroY) {
                            animal.y = obs.y - animal.hitboxAlto;
                            animal.vy = -Math.abs(animal.vy);
                        } else {
                            animal.y = obs.y + obs.alto;
                            animal.vy = Math.abs(animal.vy);
                        }
                        animal.cooldownIA = 0;
                        break;
                    }
                }

                // ✅ Actualizar DOM
                const elementoDOM = document.getElementById(`animal-${animal.id}`);
                if (elementoDOM) {
                    elementoDOM.style.left = `${(animal.x / 1000) * 100}%`;
                    elementoDOM.style.top = `${(animal.y / 1000) * 100}%`;  // 👈 maxY es 1000 en Ocean
                }
            });

            requestRef.current = requestAnimationFrame(bucleDeJuego);
        }

        iniciarJuego();

        return () => { if (requestRef.current) cancelAnimationFrame(requestRef.current); };
    }, [token]);

    if (cargando) {
        return (
            <div className="fullscreen-loader">
                <img src={imagenFondo} alt="Fondo del océano" className="imagen-fondo" />
                <LoadingHamster />
            </div>
        );
    }

    return (
        <div className="pagina-juego">

            {/* ✅ Solo un límite — el de OceanCollisions */}
            <div
                style={{
                    position: 'absolute',
                    left: `${(limite.minX / 1000) * 100}%`,
                    top: `${(limite.minY / 1000) * 100}%`,
                    width: `${((limite.maxX - limite.minX) / 1000) * 100}%`,
                    height: `${((limite.maxY - limite.minY) / 1000) * 100}%`,
                    zIndex: 5,
                    pointerEvents: 'none'
                }}
            />

            <BackButton biome="Ocean" />

            <MusicButton
                playing={musicaSonando}
                onToggle={toggleMusica}
                biome="Ocean"
            />

            <BiomeSelector currentBiome="Ocean" />

            <audio ref={audioRef} src={OceanMusic} loop />

            <div className="lienzo-corral">
                <img src={imagenFondo} alt="Fondo del océano" className="imagen-fondo" />

                <div className="capa-entidades">
                    {obstaculos.map(obs => (
                        <div
                            key={obs.id}
                            style={{
                                position: 'absolute',
                                left: `${(obs.x / 1000) * 100}%`,
                                top: `${(obs.y / 1000) * 100}%`,
                                width: `${(obs.ancho / 1000) * 100}%`,
                                height: `${(obs.alto / 1000) * 100}%`,
                            }}
                        />
                    ))}

                    {animalesRender.map((animal) => (
                        <div
                            key={animal.id}
                            id={`animal-${animal.id}`}
                            style={{
                                position: 'absolute',
                                width: `${(animal.hitboxAncho / 1000) * 100}%`,
                                height: `${(animal.hitboxAlto / 1000) * 100}%`,
                                overflow: 'visible',
                            }}
                        >
                            <div
                                style={{
                                    position: 'absolute',
                                    width: `${(animal.renderAncho / animal.hitboxAncho) * 100}%`,
                                    height: `${(animal.renderAlto / animal.hitboxAlto) * 100}%`,
                                    left: '50%',
                                    top: '50%',
                                    transform: 'translate(-50%, -50%)',
                                    backgroundImage: `url(${animal.img})`,
                                    backgroundSize: 'contain',
                                    backgroundRepeat: 'no-repeat',
                                    backgroundPosition: 'center',
                                }}
                            />
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}