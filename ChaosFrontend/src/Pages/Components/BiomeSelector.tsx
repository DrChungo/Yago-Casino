import '../../styles/BiomeSelector.css';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

interface Biome {
    name: string;
    route: string;
    emoji: string;
}

const BIOMES: Biome[] = [
    { name: 'Farm', route: '/farm', emoji: '🌾' },
    { name: 'Ocean', route: '/ocean', emoji: '🌊' },
    { name: 'Desert', route: '/desert', emoji: '🏜️' },
    { name: 'Swamp', route: '/swamp', emoji: '🌿' },
    { name: 'Jungle', route: '/jungle', emoji: '🌴' },
    { name: 'Savanna', route: '/savanna', emoji: '🦁' },
    { name: 'Jurassic', route: '/jurassic', emoji: '🦕' },
    { name: 'Urban', route: '/urban', emoji: '🏙️' },
    { name: 'Legend', route: '/legendary', emoji: '✨' },
];

interface BiomeSelectorProps {
    currentBiome?: string;
}

function BiomeSelector({ currentBiome }: BiomeSelectorProps) {
    const navigate = useNavigate();
    const [open, setOpen] = useState(false);

    const handleSelect = (route: string) => {
        setOpen(false);
        navigate(route);
    };

    return (
        <div className="biome-selector" data-biome={currentBiome}>

            {/* Botón principal que abre el desplegable */}
            <button
                className="biome-selector__trigger"
                data-biome={currentBiome}
                onClick={() => setOpen(prev => !prev)}
                aria-expanded={open}
                aria-haspopup="listbox"
            >
                🌍 Biomas {open ? '▲' : '▼'}
            </button>

            {/* Lista desplegable */}
            {open && (
                <ul className="biome-selector__dropdown" role="listbox">
                    {BIOMES.filter(b => b.name !== currentBiome).map(biome => (
                        <li
                            key={biome.name}
                            className="biome-selector__option"
                            data-biome={biome.name}
                            role="option"
                            onClick={() => handleSelect(biome.route)}
                        >
                            <span className="biome-selector__emoji">{biome.emoji}</span>
                            {biome.name}
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default BiomeSelector;