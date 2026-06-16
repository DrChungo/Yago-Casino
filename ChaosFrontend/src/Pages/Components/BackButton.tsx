import '../../styles/BackButton.css'
import { useNavigate } from 'react-router-dom';

// BackButton.tsx
interface BackButtonProps {
    biome?: string;
}

function BackButton({ biome }: BackButtonProps) {
    const navigate = useNavigate(); 

    return (
        <button
            className="back-button"
            data-biome={biome}
            onClick={() => navigate('/lobby')}
        >
            Back Lobby
        </button>
    );
}

export default BackButton;