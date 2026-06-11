import React from 'react';
import { useOutletContext } from 'react-router-dom';
import { StartButton } from '../components/UI/StartButton';

const IntroPage: React.FC = () => {
    const { handleStartPressed, handleHoverStart } = useOutletContext<any>();

    return (
        <StartButton onStart={handleStartPressed} onHover={handleHoverStart} />
    );
};

export default IntroPage;