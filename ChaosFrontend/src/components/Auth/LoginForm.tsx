import React from 'react';
import { Link } from 'react-router-dom';
import './LoginForm.css';

interface LoginFormProps {
    logoCasino: string;
    error: string | null;
    cargando: boolean;
    manejarEnvioFormulario: (e: React.FormEvent<HTMLFormElement>) => Promise<void>;
    email: string;
    setEmail: (valor: string) => void;
    password: string;
    setPassword: (valor: string) => void;
}

export const LoginForm: React.FC<LoginFormProps> = ({
    logoCasino,
    error,
    cargando,
    manejarEnvioFormulario,
    email,
    setEmail,
    password,
    setPassword,
}) => {
    return (
        <div className="centered-container">
            <div className="form-container-royale">
                <form onSubmit={manejarEnvioFormulario}>
                    <img src={logoCasino} className="logo" alt="Logo Casino" />

                    {error && <div className="error-message-royale">{error}</div>}

                    <div className="input-group">
                        <input
                            type="text"
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
                        {cargando ? 'VERIFYING...' : 'ENTER THE CASINO'}
                    </button>

                    <div className="form-links" style={{ display: 'flex', flexDirection: 'column', gap: '10px', alignItems: 'center' }}>
                        <span>Forgot the password ?</span>
                        <Link to="/register" style={{ textDecoration: 'none', color: 'inherit', fontSize: '15px' }}>
                            <span style={{ transition: 'color 300ms' }} onMouseEnter={(e) => e.currentTarget.style.color = '#d4af37'} onMouseLeave={(e) => e.currentTarget.style.color = 'inherit'}>
                                Don't have an account? Sign Up
                            </span>
                        </Link>
                    </div>
                </form>
            </div>
        </div>
    );
};