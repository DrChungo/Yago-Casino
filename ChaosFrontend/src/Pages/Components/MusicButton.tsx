import '../../styles/MusicButton.css';

interface MusicButtonProps {
    playing: boolean;
    onToggle: () => void;
    biome?: string;
}

function MusicButton({ playing, onToggle, biome }: MusicButtonProps) {
    return (
        <button
            className="music-button"
            data-biome={biome}
            onClick={onToggle}
            aria-label={playing ? 'Silenciar música' : 'Reproducir música'}
            title={playing ? 'Silenciar música' : 'Reproducir música'}
        >
            <span className="music-button__icon">{playing ? '🔊' : '🔇'}</span>
            <span className="music-button__label">{playing ? 'ON' : 'OFF'}</span>
        </button>
    );
}

export default MusicButton;