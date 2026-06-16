/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-useless-assignment */
import { useEffect, useRef, useState } from 'react';
import '../../styles/Farm.Modules.css';
import '../../styles/LoadingPages.css'
import imagenFondo from '../../assets/BiomasBG/casino_desert.png';
import { chequearColisionRectangulos, limitesCielo, limite, obstaculos } from '../../AnimalMovement/DesertCollisions';
import BackButton from '../Components/BackButton';
import EditorButton from '../Components/EditorButton';
import { getAnimalConfig } from '../../AnimalMovement/AnimalCollisions';
import BiomeSelector from '../Components/BiomeSelector';
import MusicButton from '../Components/MusicButton';
import { getAnimalImageUrl } from '../../services/animalImageService';
import DesertMusic from '../../assets/Audios/dessert_soundtrack.m4a';
import { useNavigate } from "react-router-dom";
import LoadingHamster from '../Components/LoadingHamster';


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

export default function DesertPage() {

    const zonasSpawn = {
        suelo: { minX: 300, maxX: 700, minY: 320, maxY: 550 },
        cielo: { minX: 0, maxX: 1000, minY: 50, maxY: 200 }
    };

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
                const response = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/Animal/GetAnimalByOwnerId`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    },
                });
                const habi = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/AnimalValueConfig/images`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (!response.ok || !habi.ok) throw new Error("Error en el fetch");
                if (response.ok && habi.ok) {
                    setCargando(false);
                }

                const jsonResult2 = await habi.json();
                const jsonResult = await response.json();
                console.log("Lo que llega de .NET:", jsonResult, jsonResult2);

                const arrayAnimales = jsonResult.data || jsonResult.item3 || [];
                const arrayHabi = jsonResult2 || jsonResult2.item3 || [];

                const configByType = new Map();
                arrayHabi.forEach((h: any) => {
                    const tipo = h.animalType || h.typeAnimal;
                    if (tipo) {
                        configByType.set(tipo.toLowerCase(), h);
                    }
                });

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

                const DesertAnimals: AnimalData[] = arrayAnimales.map((ani: any) => {
                    const tipoAnimal = ani.typeAnimal || ani.tipo || 'horse';
                    const config = configByType.get(tipoAnimal.toLowerCase());
                    const habitat = config?.habitat;

                    const esMecha = ani.rarity === true || ani.Rarity === true;

                    const config2 = getAnimalConfig(tipoAnimal);

                    const anchoHitbox = config2?.anchoHitbox ?? 40;
                    const altoHitbox = config2?.altoHitbox ?? 20;
                    const escala = config2?.escala ?? 3;


                    const svgBd = esMecha
                        ? config?.imageUrlMecha
                        : config?.imageUrlNormal;
                    const imgSeleccionada = getAnimalImageUrl(svgBd);

                    const esCielo = habitat === 'Desert / Sky';
                    const zona = esCielo ? zonasSpawn.cielo : zonasSpawn.suelo;

                    const hitboxAncho = anchoHitbox;
                    const hitboxAlto = altoHitbox;
                    const renderAncho = anchoHitbox * escala;
                    const renderAlto = altoHitbox * escala;

                    return {
                        id: ani.id,
                        tipo: tipoAnimal,
                        habitat,
                        x: zona.minX + Math.random() * (zona.maxX - zona.minX),
                        y: zona.minY + Math.random() * (zona.maxY - zona.minY),
                        hitboxAncho,
                        hitboxAlto,
                        renderAncho,
                        renderAlto,
                        vx: 0,
                        vy: 0,
                        img: imgSeleccionada,
                        cooldownIA: Math.random() * 60,
                        raro: esMecha
                    };
                }).filter((a: { habitat: string }) =>
                    a.habitat === 'Desert' || a.habitat === 'Desert / Sky'
                );

                estadoJuego.current.animales = DesertAnimals;
                setAnimalesRender(DesertAnimals);
                setCargando(false);

                requestRef.current = requestAnimationFrame(bucleDeJuego);
            } catch (error) {
                console.error("Error al cargar los animales:", error);
            }
        };

        function bucleDeJuego() {
            estadoJuego.current.animales.forEach(animal => {
                const limites = animal.habitat === 'Desert / Sky' ? limitesCielo : limite;

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

                if (animal.x <= limites.minX) {
                    animal.x = limites.minX;
                    animal.vx = Math.abs(animal.vx);
                    animal.cooldownIA = 0;
                }
                if (animal.x + animal.hitboxAncho >= limites.maxX) {
                    animal.x = limites.maxX - animal.hitboxAncho;
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

                if (animal.y <= limites.minY) {
                    animal.y = limites.minY;
                    animal.vy = Math.abs(animal.vy);
                    animal.cooldownIA = 0;
                }
                if (animal.y + animal.hitboxAlto >= limites.maxY) {
                    animal.y = limites.maxY - animal.hitboxAlto;
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

                // ✅ ACTUALIZAR DOM — mueve el div HITBOX
                const elementoDOM = document.getElementById(`animal-${animal.id}`);
                if (elementoDOM) {
                    elementoDOM.style.left = `${(animal.x / 1000) * 100}%`;
                    elementoDOM.style.top = `${(animal.y / 600) * 100}%`;
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
                <img src={imagenFondo} alt="Fondo del corral" className="imagen-fondo" />
                <LoadingHamster />
            </div>
        );
    }

    return (
        <div className="pagina-juego">

            <div
                style={{
                    position: 'absolute',
                    left: `${(limite.minX / 1000) * 100}%`,
                    top: `${(limite.minY / 600) * 100}%`,
                    width: `${((limite.maxX - limite.minX) / 1000) * 100}%`,
                    height: `${((limite.maxY - limite.minY) / 600) * 100}%`,
                    zIndex: 5,
                    pointerEvents: 'none'
                }}
            />
            <div
                style={{
                    position: 'absolute',
                    left: `${(limitesCielo.minX / 1000) * 100}%`,
                    top: `${(limitesCielo.minY / 600) * 100}%`,
                    width: `${((limitesCielo.maxX - limitesCielo.minX) / 1000) * 100}%`,
                    height: `${((limitesCielo.maxY - limitesCielo.minY) / 600) * 100}%`,
                    zIndex: 5,
                    pointerEvents: 'none'
                }}
            />

            <BackButton biome="Desert" />
            <EditorButton />
            <MusicButton 
                playing={musicaSonando}
                onToggle={toggleMusica}
                biome="Desert"
            />
            <BiomeSelector currentBiome="Desert" />

            <audio ref={audioRef} src={DesertMusic} loop />

            <div className="lienzo-corral">
                <img src={imagenFondo} alt="Fondo del corral" className="imagen-fondo" />

                {!cargando && (
                    <div className="capa-entidades">
                        {obstaculos.map(obs => (
                            <div
                                key={obs.id}
                                style={{
                                    position: 'absolute',
                                    left: `${(obs.x / 1000) * 100}%`,
                                    top: `${(obs.y / 600) * 100}%`,
                                    width: `${(obs.ancho / 1000) * 100}%`,
                                    height: `${(obs.alto / 600) * 100}%`
                                }}
                            />
                        ))}

                        {animalesRender.map((animal) => (
                            // ✅ DIV HITBOX — tamaño lógico, posición real
                            <div
                                key={animal.id}
                                id={`animal-${animal.id}`}
                                style={{
                                    position: 'absolute',
                                    width: `${(animal.hitboxAncho / 1000) * 100}%`,
                                    height: `${(animal.hitboxAlto / 600) * 100}%`,
                                    overflow: 'visible',     
                                }}
                            >
                                {/* ✅ DIV VISUAL — más grande, centrado sobre la hitbox */}
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
                )}
            </div>
        </div>
    );
}