import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const parseJwt = (token: string) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
};

export default function EditorButton() {
    const navigate = useNavigate();
    const [esAdmin, setEsAdmin] = useState(false);
    const token = localStorage.getItem('token_casino');

    useEffect(() => {
        const verificarAdmin = async () => {
            if (!token) return;

            // 1. Intentar por JWT decodificado
            try {
                const payload = parseJwt(token);
                const role = payload?.role || payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
                if (role === 'Admin') {
                    setEsAdmin(true);
                    return;
                }
            } catch (e) {
                console.error("Error al decodificar JWT", e);
            }

            // 2. Fallback a la API de GetMyUser
            try {
                const response = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/User/GetMyUser`, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    if (data?.data?.isAdmin === true) {
                        setEsAdmin(true);
                    }
                }
            } catch (error) {
                console.error("Error al obtener los datos de usuario para verificar admin:", error);
            }
        };

        verificarAdmin();
    }, [token]);

    if (!esAdmin) return null;

    return (
        <button
            onClick={() => navigate('/dev/animal-editor')}
            className="back-button editor-btn"
        >
            Editor
        </button>
    );
}
