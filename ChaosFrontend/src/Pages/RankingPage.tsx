import {
    getTopAnimals,
    getUserById,
    getAnimalImages,
    type AnimalRanking,
    type UserData,
} from '../services/rankingService';
import PodiumColumn from '../Pages/Components/PodiumColumn';
import fondoPodio from '../assets/CasinoImages/casino_podio.png';
import '../styles/Ranking.css';
import BackButton from './Components/BackButton';
import LoadingHamster from '../Pages/Components/LoadingHamster';
import { useEffect, useState } from 'react';
import { getAnimalImageUrl } from '../services/animalImageService';

interface EnrichedAnimal {
    animal: AnimalRanking;
    owner: UserData | null;
    svgImg: string;
    place: 1 | 2 | 3;
}

export default function RankingPage() {
    const [data, setData] = useState<EnrichedAnimal[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function load() {
            try {
                const [animals, images] = await Promise.all([
                    getTopAnimals(),
                    getAnimalImages(),
                ]);

                const configByType = new Map<string, any>();
                images.forEach((h: any) => {
                    const tipo = h.animalType || h.typeAnimal;
                    if (tipo) configByType.set(tipo.toLowerCase(), h);
                });

                const uniqueOwnerIds = [...new Set(animals.map(a => a.ownerId))];
                const ownerResults = await Promise.all(
                    uniqueOwnerIds.map(id => getUserById(id))
                );
                const ownerMap = new Map<string, UserData | null>();
                uniqueOwnerIds.forEach((id, i) => ownerMap.set(id, ownerResults[i]));

                const enriched: EnrichedAnimal[] = animals.map((animal, i) => {
                    const config = configByType.get(animal.typeAnimal.toLowerCase());
                    const rawSvg = animal.rarity
                        ? config?.imageUrlMecha
                        : config?.imageUrlNormal;
                    const svgImg = rawSvg ? getAnimalImageUrl(rawSvg) : '';

                    return {
                        animal,
                        owner: ownerMap.get(animal.ownerId) ?? null,
                        svgImg,
                        place: (i + 1) as 1 | 2 | 3,
                    };
                });

                setData(enriched);
            } catch (e: any) {
                setError(e.message);
            } finally {
                setLoading(false);
            }
        }
        load();
    }, []);

    if (loading) {
        return (
            <div className="fullscreen-loader">
                <img src={fondoPodio} alt="Podio Casino" className="ranking-bg" />
                <LoadingHamster />
            </div>
        );
    }

    if (error) {
        return <div className="ranking-state ranking-error">⚠️ {error}</div>;
    }

    // Orden visual: [2°, 1°, 3°]
    const podiumOrder = [data[1], data[0], data[2]];

    return (
        <div className="ranking-page">
            <BackButton />

            {/* ✅ Título arriba a la derecha */}
            <div className="ranking-title">
                🏆 MOST VALUES ANIMALS 🏆
            </div>

            <div className="ranking-canvas">

                {/* Fondo */}
                <img
                    src={fondoPodio}
                    alt="Podio Casino"
                    className="ranking-bg"
                />

                {/* 🥈 2° puesto */}
                <div className="animal-slot slot-2">
                    <PodiumColumn
                        animal={podiumOrder[0].animal}
                        owner={podiumOrder[0].owner}
                        place={2}
                        svgImg={podiumOrder[0].svgImg}
                    />
                </div>

                {/* 🥇 1° puesto */}
                <div className="animal-slot slot-1">
                    <PodiumColumn
                        animal={podiumOrder[1].animal}
                        owner={podiumOrder[1].owner}
                        place={1}
                        svgImg={podiumOrder[1].svgImg}
                    />
                </div>

                {/* 🥉 3° puesto */}
                <div className="animal-slot slot-3">
                    <PodiumColumn
                        animal={podiumOrder[2].animal}
                        owner={podiumOrder[2].owner}
                        place={3}
                        svgImg={podiumOrder[2].svgImg}
                    />
                </div>

            </div>
        </div>
    );
}