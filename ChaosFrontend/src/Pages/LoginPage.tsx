import React from 'react';
import { useOutletContext } from 'react-router-dom';
import { LoginForm } from '../components/Auth/LoginForm';

const LoginPage: React.FC = () => {
    const {
        logoCasino,
        email,
        setEmail,
        password,
        setPassword,
        error,
        cargando,
        realizarLogin,
    } = useOutletContext<any>();

    return (
        <LoginForm
            logoCasino={logoCasino}
            error={error}
            cargando={cargando}
            manejarEnvioFormulario={realizarLogin}
            email={email}
            setEmail={setEmail}
            password={password}
            setPassword={setPassword}
        />
    );
};

export default LoginPage;