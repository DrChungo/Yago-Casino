import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import logoCasino from '../assets/CasinoImages/casino_logo.png';
import '../components/Auth/LoginForm.css'; // Reuse Royale login form styling

const RegisterPage: React.FC = () => {
    const [name, setName] = useState<string>('');
    const [email, setEmail] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [error, setError] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [cargando, setCargando] = useState<boolean>(false);
    const navigate = useNavigate();

    const manejarRegistro = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        setError(null);
        setSuccessMessage(null);
        setCargando(true);

        try {
            const urlApi = 'https://localhost:7101/api/auth/SignIn';

            const respuesta = await fetch(urlApi, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ name, email, password }),
            });

            const datos = await respuesta.json();

            if (!respuesta.ok || !datos.result) {
                throw new Error(datos.message || 'Error occurred during registration.');
            }

            setSuccessMessage(datos.message || 'User created successfully! Redirecting to login...');
            
            // Redirect to login after a short delay
            setTimeout(() => {
                navigate('/login');
            }, 2000);

        } catch (err: any) {
            setError(err.message || 'Could not connect to the server.');
        } finally {
            setCargando(false);
        }
    };

    return (
        <div className="centered-container">
            <div className="form-container-royale">
                <form onSubmit={manejarRegistro}>
                    <img src={logoCasino} className="logo" alt="Logo Casino" />

                    {error && <div className="error-message-royale">{error}</div>}
                    {successMessage && (
                        <div className="error-message-royale" style={{ borderColor: '#d4af37', color: '#d4af37', backgroundColor: 'rgba(212, 175, 55, 0.15)' }}>
                            {successMessage}
                        </div>
                    )}

                    <div className="input-group">
                        <input
                            type="text"
                            placeholder="Full Name"
                            required
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            disabled={cargando}
                        />
                    </div>

                    <div className="input-group">
                        <input
                            type="email"
                            placeholder="Email"
                            required
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            disabled={cargando}
                        />
                    </div>

                    <div className="input-group">
                        <input
                            type="password"
                            placeholder="Password"
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            disabled={cargando}
                        />
                    </div>

                    <button type="submit" className="boton-submit" disabled={cargando}>
                        {cargando ? 'CREATING ACCOUNT...' : 'REGISTER'}
                    </button>

                    <div className="form-links" onClick={() => navigate('/login')}>
                        <span>Already have an account? Sign In</span>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default RegisterPage;
