import { useState } from 'react';

export const useAuth = (onLoginSuccess: () => void, onLogoutSuccess: () => void) => {
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [error, setError] = useState<string | null>(null);
    const [cargando, setCargando] = useState<boolean>(false);
    const [estaLogueado, setEstaLogueado] = useState<boolean>(() => {
        // Lazy initialization: check if a token already existed when reloading the website
        return !!localStorage.getItem('token_casino');
    });
    const Api_URL = import.meta.env.VITE_BASE_URL;
    const realizarLogin = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError(null);
        setCargando(true);

        try {
            const urlApi = `${Api_URL}/api/auth/Login`;

            const respuesta = await fetch(urlApi, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            if (!respuesta.ok) {
                const datosError = await respuesta.json().catch(() => null);
                throw new Error(datosError?.mensaje || 'Incorrect credentials or server error.');
            }

            const datosUsuario = await respuesta.json();
            console.log("Logged in successfully!", datosUsuario);

            // Token persistence
            localStorage.setItem('token_casino', datosUsuario.token);
            setEstaLogueado(true);
            onLoginSuccess();

        } catch (err: any) {
            setError(err.message || 'Could not connect to the server.');
        } finally {
            setCargando(false);
        }
    };

    const realizarLogout = (): void => {
        localStorage.removeItem('token_casino');
        setEstaLogueado(false);
        setEmail('');
        setPassword('');
        setError(null);
        onLogoutSuccess();
    };

    return {
        email,
        setEmail,
        password,
        setPassword,
        error,
        cargando,
        estaLogueado,
        realizarLogin,
        realizarLogout,
    };
};
