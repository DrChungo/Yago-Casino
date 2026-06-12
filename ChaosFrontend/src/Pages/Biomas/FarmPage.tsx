/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useRef, useState } from 'react';
import '../../styles/Farm.Modules.css';
import '../../styles/LoadingPages.css';
import imagenFondo from '../../assets/BiomasBG/casino_establo.png';
import { chequearColisionRectangulos, limitesCielo, limite, obstaculos } from '../../AnimalMovement/FarmCollisions';
import farmMusic from '../../assets/Audios/farm_soundtrack.m4a';
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

export default function FarmPage() {

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

    // ✅ useEffect SEPARADO — solo para quitar el fondo oscuro del body mientras estamos en FarmPage
    useEffect(() => {
        document.body.classList.add('farm-page');
        document.documentElement.classList.add('farm-page');

        // ✅ Forzamos el fondo del root directamente por si el CSS no llega a tiempo
        const prevBg = document.documentElement.style.background;
        const prevBodyBg = document.body.style.backgroundColor;
        document.documentElement.style.background = 'transparent';
        document.body.style.backgroundColor = 'transparent';

        return () => {
            document.body.classList.remove('farm-page');
            document.documentElement.classList.remove('farm-page');

            // ✅ Restauramos lo que había antes
            document.documentElement.style.background = prevBg;
            document.body.style.backgroundColor = prevBodyBg;
        };
    }, []);

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

                const arrayAnimales = jsonResult.data || jsonResult.item3 || [];
                const arrayHabi = jsonResult2 || jsonResult2.item3 || [];

                const configByType = new Map();
                arrayHabi.forEach((h: any) => {
                    const tipo = h.animalType || h.typeAnimal;
                    if (tipo) configByType.set(tipo.toLowerCase(), h);
                });

                if (arrayAnimales.length === 0) {
                    console.warn("No hay animales o el formato es incorrecto");
                    return;
                }

                if (arrayHabi.length === 0) {
                    console.warn("No cargo bien los animales");
                    return;
                }

                const FarmAnimals: AnimalData[] = arrayAnimales.map((ani: any) => {
                    const tipoAnimal = ani.typeAnimal || ani.tipo || 'horse';
                    const config = configByType.get(tipoAnimal.toLowerCase());
                    const habitat = config?.habitat;

                    const esMecha = ani.rarity === true || ani.Rarity === true;
                    let imgSeleccionada = "";

                    const config2 = getAnimalConfig(tipoAnimal);
                    const anchoHitbox = config2?.anchoHitbox ?? 40;
                    const altoHitbox = config2?.altoHitbox ?? 20;
                    const escala = config2?.escala ?? 3;

                    const limpiarSvg = (svg: string) =>
                        svg.replace(/<\?xml.*?\?>/, '').replace(/<!--.*?-->/g, '').trim();

                    const svgBd = esMecha ? config?.imageUrlMecha : config?.imageUrlNormal;

                    if (svgBd && svgBd.trim() !== "") {
                        try {
                            const svgBase64 = btoa(unescape(encodeURIComponent(limpiarSvg(svgBd))));
                            imgSeleccionada = `data:image/svg+xml;base64,${svgBase64}`;
                        } catch (e) {
                            console.error("Error SVG:", e);
                        }
                    }

                    const esCielo = habitat === 'Sky';
                    const zona = esCielo ? zonasSpawn.cielo : zonasSpawn.suelo;

                    return {
                        id: ani.id,
                        tipo: tipoAnimal,
                        habitat,
                        x: zona.minX + Math.random() * (zona.maxX - zona.minX),
                        y: zona.minY + Math.random() * (zona.maxY - zona.minY),
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
                }).filter((a: { habitat: string }) =>
                    a.habitat === 'Farm' || a.habitat === 'Sky'
                );

                estadoJuego.current.animales = FarmAnimals;
                setAnimalesRender(FarmAnimals);
                requestRef.current = requestAnimationFrame(bucleDeJuego);

            } catch (error) {
                console.error("Error al cargar los animales:", error);
            }
        };

        function bucleDeJuego() {
            estadoJuego.current.animales.forEach(animal => {
                const limites = animal.habitat === 'Sky' ? limitesCielo : limite;

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
                        animal.x = animalCentroX < obs.x + obs.ancho / 2
                            ? obs.x - animal.hitboxAncho
                            : obs.x + obs.ancho;
                        animal.vx = animalCentroX < obs.x + obs.ancho / 2
                            ? -Math.abs(animal.vx)
                            : Math.abs(animal.vx);
                        animal.cooldownIA = 0;
                        break;
                    }
                }

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
                        animal.y = animalCentroY < obs.y + obs.alto / 2
                            ? obs.y - animal.hitboxAlto
                            : obs.y + obs.alto;
                        animal.vy = animalCentroY < obs.y + obs.alto / 2
                            ? -Math.abs(animal.vy)
                            : Math.abs(animal.vy);
                        animal.cooldownIA = 0;
                        break;
                    }
                }

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

    return (
        <div className="pagina-juego">
            <audio ref={audioRef} src={farmMusic} loop />

            <div className="lienzo-corral">

                {/* ✅ La imagen siempre está montada desde el primer render */}
                <img src={imagenFondo} alt="Fondo del corral" className="imagen-fondo" />

                {/* ✅ Overlay de carga encima de la imagen — desaparece sin desmontar la imagen */}
                {cargando && (
                    <div className="loading-overlay">
                        <LoadingHamster />
                    </div>
                )}

                {/* ✅ Controles y entidades solo cuando ya cargó */}
                {!cargando && (
                    <>
                        <BackButton biome="Farm" />

                        <MusicButton
                            playing={musicaSonando}
                            onToggle={toggleMusica}
                            biome="Farm"
                        />

                        <BiomeSelector currentBiome="Farm" />

                        <div className="capa-entidades">
                            {animalesRender.map((animal) => (
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
                    </>
                )}
            </div>
        </div>
    );
}