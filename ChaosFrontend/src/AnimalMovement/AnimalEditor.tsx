import { useState, useEffect } from 'react';
import { animalConfigs, setRuntimeAnimalConfigs } from '../AnimalMovement/AnimalCollisions';
import type { AnimalConfig } from '../AnimalMovement/AnimalCollisions';
import { getAnimalImageUrl } from '../services/animalImageService';
import { useNavigate } from 'react-router-dom';

const PAGE_SIZE = 3;

export default function AnimalEditor() {
    const navigate = useNavigate();
    const token = localStorage.getItem('token_casino');
    const [configs, setConfigs] = useState<AnimalConfig[]>([]);
    const [imagenes, setImagenes] = useState<Map<string, string>>(new Map());
    const [pagina, setPagina] = useState(0);
    const [filtro, setFiltro] = useState('');

    const configsFiltradas = configs.filter(c =>
        c.tipo.toLowerCase().includes(filtro.toLowerCase()) ||
        c.habitat.toLowerCase().includes(filtro.toLowerCase())
    );

    const totalPaginas = Math.ceil(configsFiltradas.length / PAGE_SIZE);
    const animalesPagina = configsFiltradas.slice(pagina * PAGE_SIZE, pagina * PAGE_SIZE + PAGE_SIZE);

    useEffect(() => {
        setPagina(0);
    }, [filtro]);

    useEffect(() => {
        const cargarConfigs = async () => {
            try {
                const res = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/dev/animalconfigs`, {
                    headers: { 'Authorization': `Bearer ${token}` }
                });
                if (res.ok) {
                    const data = await res.json();
                    setConfigs(data);
                }
            } catch (error) {
                console.error("Error loading animal configs:", error);
            }
        };
        cargarConfigs();
    }, [token]);

    useEffect(() => {
        const cargarImagenes = async () => {
            const habi = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/AnimalValueConfig/images`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            const data = await habi.json();

            const mapa = new Map<string, string>();
            data.forEach((h: any) => {
                const tipo = (h.animalType || h.typeAnimal || '').toLowerCase();
                const svgRaw = h.imageUrlNormal;
                if (svgRaw) {
                    mapa.set(tipo, getAnimalImageUrl(svgRaw));
                }
            });
            setImagenes(mapa);
        };
        cargarImagenes();
    }, [token]);

    const actualizar = (tipo: string, campo: keyof AnimalConfig, valor: number) => {
        setConfigs(prev =>
            prev.map(c => c.tipo === tipo ? { ...c, [campo]: valor } : c)
        );
    };

    const copiarConfig = () => {
        const lineas = configs.map(c =>
            `    { tipo: '${c.tipo}', habitat: '${c.habitat}', anchoHitbox: ${c.anchoHitbox}, altoHitbox: ${c.altoHitbox}, escala: ${c.escala} },`
        ).join('\n');
        const texto = `export const animalConfigs: AnimalConfig[] = [\n${lineas}\n];`;
        navigator.clipboard.writeText(texto);
        alert('✅ Config copiada! Pegala en AnimalCollisions.ts');
    };

    const guardarConfigs = async () => {
        try {
            const res = await fetch(`${import.meta.env.VITE_BASE_URL || 'https://localhost:7101'}/api/dev/animalconfigs`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(configs)
            });
             if (res.ok) {
                setRuntimeAnimalConfigs(configs);
                alert('💾 ¡Configuraciones guardadas correctamente en animalConfigs.json!');
            } else {
                alert('❌ Error al guardar las configuraciones');
            }
        } catch (error) {
            console.error("Error saving configs:", error);
            alert('❌ Error de red al guardar');
        }
    };

    return (
        <div style={{
            backgroundColor: '#1a1a2e',
            height: '100vh',
            display: 'flex',
            flexDirection: 'column',
            padding: '16px',
            color: 'white',
            boxSizing: 'border-box',
            overflow: 'hidden'
        }}>

            {/* ── HEADER ── */}
            <div style={{ textAlign: 'center', marginBottom: '12px', flexShrink: 0 }}>
                <h1 style={{ margin: '0 0 8px', color: 'white', marginBottom: '30px' }}>🦕 Editor de Hitboxes</h1>
                <div style={{ display: 'flex', justifyContent: 'center', gap: '12px' }}>
                    <button
                        onClick={() => navigate('/farm')}
                        style={{
                            padding: '10px 24px', backgroundColor: '#e74c3c',
                            color: 'white', border: 'none', borderRadius: '8px',
                            cursor: 'pointer', fontSize: '15px', fontWeight: 'bold'
                        }}
                    >
                        🌾 Volver al Hábitat
                    </button>
                    <button
                        onClick={guardarConfigs}
                        style={{
                            padding: '10px 24px', backgroundColor: '#e67e22',
                            color: 'white', border: 'none', borderRadius: '8px',
                            cursor: 'pointer', fontSize: '15px', fontWeight: 'bold'
                        }}
                    >
                        💾 Guardar Cambios
                    </button>
                    <button
                        onClick={copiarConfig}
                        style={{
                            padding: '10px 24px', backgroundColor: '#27ae60',
                            color: 'white', border: 'none', borderRadius: '8px',
                            cursor: 'pointer', fontSize: '15px', fontWeight: 'bold'
                        }}
                    >
                        📋 Copiar config completa
                    </button>
                </div>
            </div>

            {/* ── BUSCADOR ── */}
            <div style={{
                display: 'flex',
                justifyContent: 'center',
                marginBottom: '16px',
                flexShrink: 0
            }}>
                <div style={{
                    position: 'relative',
                    width: '100%',
                    maxWidth: '400px'
                }}>
                    <input
                        type="text"
                        placeholder="Buscar por tipo o hábitat..."
                        value={filtro}
                        onChange={(e) => setFiltro(e.target.value)}
                        style={{
                            width: '100%',
                            padding: '12px 40px 12px 40px',
                            fontSize: '16px',
                            backgroundColor: '#0f3460',
                            border: '2px solid #2980b9',
                            borderRadius: '24px',
                            color: 'white',
                            outline: 'none',
                            transition: 'border-color 0.2s, box-shadow 0.2s',
                            boxSizing: 'border-box'
                        }}
                    />
                    <span style={{
                        position: 'absolute',
                        left: '14px',
                        top: '50%',
                        transform: 'translateY(-50%)',
                        fontSize: '18px',
                        pointerEvents: 'none',
                        opacity: 0.7
                    }}>🔍</span>
                    {filtro && (
                        <button
                            onClick={() => setFiltro('')}
                            style={{
                                position: 'absolute',
                                right: '14px',
                                top: '50%',
                                transform: 'translateY(-50%)',
                                background: 'none',
                                border: 'none',
                                color: '#aaa',
                                fontSize: '16px',
                                cursor: 'pointer',
                                padding: '4px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center'
                            }}
                        >
                            ✖
                        </button>
                    )}
                </div>
            </div>

            {/* ── NAVEGACION ── */}
            <div style={{
                display: 'flex', alignItems: 'center', justifyContent: 'center',
                gap: '16px', marginBottom: '16px', flexShrink: 0
            }}>
                <button
                    onClick={() => setPagina(p => Math.max(0, p - 1))}
                    disabled={pagina === 0}
                    style={{
                        padding: '8px 20px', fontSize: '18px',
                        backgroundColor: pagina === 0 ? '#333' : '#2980b9',
                        color: 'white', border: 'none', borderRadius: '8px',
                        cursor: pagina === 0 ? 'not-allowed' : 'pointer'
                    }}
                >
                    ◀ Anterior
                </button>

                {/* Dots de navegacion */}
                <div style={{ display: 'flex', gap: '6px', alignItems: 'center' }}>
                    {Array.from({ length: totalPaginas }).map((_, i) => (
                        <div
                            key={i}
                            onClick={() => setPagina(i)}
                            style={{
                                width: i === pagina ? '20px' : '8px',
                                height: '8px',
                                borderRadius: '4px',
                                backgroundColor: i === pagina ? '#27ae60' : '#555',
                                cursor: 'pointer',
                                transition: 'all 0.2s'
                            }}
                        />
                    ))}
                </div>

                <button
                    onClick={() => setPagina(p => Math.min(totalPaginas - 1, p + 1))}
                    disabled={pagina === totalPaginas - 1}
                    style={{
                        padding: '8px 20px', fontSize: '18px',
                        backgroundColor: pagina === totalPaginas - 1 ? '#333' : '#2980b9',
                        color: 'white', border: 'none', borderRadius: '8px',
                        cursor: pagina === totalPaginas - 1 ? 'not-allowed' : 'pointer'
                    }}
                >
                    Siguiente ▶
                </button>
            </div>

            {/* Contador */}
            <p style={{ textAlign: 'center', margin: '0 0 16px', color: '#aaa', fontSize: '13px', flexShrink: 0 }}>
                Página {totalPaginas > 0 ? pagina + 1 : 0} de {totalPaginas} — Animales {configsFiltradas.length > 0 ? pagina * PAGE_SIZE + 1 : 0} a {Math.min((pagina + 1) * PAGE_SIZE, configsFiltradas.length)} de {configsFiltradas.length}
            </p>

            {/* ── CARDS ── */}
            <div style={{
                display: 'flex',
                gap: '20px',
                justifyContent: 'center',
                flex: 1,
                minHeight: 0
            }}>
                {configsFiltradas.length === 0 ? (
                    <div style={{
                        display: 'flex',
                        flexDirection: 'column',
                        alignItems: 'center',
                        justifyContent: 'center',
                        color: '#aaa',
                        fontSize: '18px',
                        width: '100%',
                        padding: '40px'
                    }}>
                        🔍 No se encontraron animales con el término de búsqueda.
                    </div>
                ) : (
                    animalesPagina.map(cfg => {
                        const img = imagenes.get(cfg.tipo.toLowerCase());
                        const renderAncho = cfg.anchoHitbox * cfg.escala;
                        const renderAlto = cfg.altoHitbox * cfg.escala;

                        return (
                            <div key={cfg.tipo} style={{
                                backgroundColor: '#16213e',
                                borderRadius: '12px',
                                padding: '20px',
                                flex: 1,
                                maxWidth: '380px',
                                display: 'flex',
                                flexDirection: 'column',
                                boxShadow: '0 4px 12px rgba(0,0,0,0.4)'
                            }}>
                                {/* Nombre y habitat */}
                                <h3 style={{ margin: '0 0 2px', textTransform: 'capitalize', fontSize: '18px' }}>
                                    {cfg.tipo}
                                </h3>
                                <p style={{ margin: '0 0 12px', fontSize: '12px', color: '#aaa' }}>
                                    🌍 {cfg.habitat}
                                </p>

                                {/* PREVIEW */}
                                <div style={{
                                    position: 'relative',
                                    flex: 1,
                                    minHeight: '200px',
                                    backgroundColor: '#0f3460',
                                    borderRadius: '10px',
                                    marginBottom: '16px',
                                    overflow: 'hidden'
                                }}>
                                    {img ? (
                                        <div style={{
                                            position: 'absolute',
                                            width: `${renderAncho}px`,
                                            height: `${renderAlto}px`,
                                            left: '50%',
                                            top: '50%',
                                            transform: 'translate(-50%, -50%)',
                                            backgroundImage: `url(${img})`,
                                            backgroundSize: 'contain',
                                            backgroundRepeat: 'no-repeat',
                                            backgroundPosition: 'center',
                                        }} />
                                    ) : (
                                        <div style={{
                                            position: 'absolute', left: '50%', top: '50%',
                                            transform: 'translate(-50%, -50%)',
                                            color: '#555', fontSize: '13px'
                                        }}>
                                            Sin imagen
                                        </div>
                                    )}

                                    {/* Hitbox */}
                                    <div style={{
                                        position: 'absolute',
                                        width: `${cfg.anchoHitbox}px`,
                                        height: `${cfg.altoHitbox}px`,
                                        left: '50%',
                                        top: '50%',
                                        transform: 'translate(-50%, -50%)',
                                        border: '2px solid lime',
                                        boxSizing: 'border-box',
                                        pointerEvents: 'none'
                                    }} />

                                    {/* Medidas en esquina */}
                                    <div style={{
                                        position: 'absolute', bottom: '6px', right: '8px',
                                        fontSize: '11px', color: 'lime', opacity: 0.8
                                    }}>
                                        {cfg.anchoHitbox} × {cfg.altoHitbox}
                                    </div>
                                </div>

                                {/* SLIDERS */}
                                {(
                                    [
                                        { label: '📐 Hitbox Ancho', campo: 'anchoHitbox' as keyof AnimalConfig, min: 1, max: 200, step: 0.5 },
                                        { label: '📐 Hitbox Alto', campo: 'altoHitbox' as keyof AnimalConfig, min: 1, max: 200, step: 0.5 },
                                        { label: '🔍 Escala Visual', campo: 'escala' as keyof AnimalConfig, min: 0.1, max: 20, step: 0.1 },
                                    ] as const
                                ).map(({ label, campo, min, max, step }) => (
                                    <div key={campo} style={{ marginBottom: '10px' }}>
                                        <label style={{ fontSize: '13px', display: 'block', marginBottom: '4px' }}>
                                            {label}: <strong style={{ color: '#4fc3f7' }}>
                                                {(cfg[campo] as number).toFixed(1)}
                                            </strong>
                                        </label>
                                        <input
                                            type="range"
                                            min={min}
                                            max={max}
                                            step={step}
                                            value={cfg[campo] as number}
                                            onChange={e => actualizar(cfg.tipo, campo, Number(e.target.value))}
                                            style={{ width: '100%', accentColor: '#4fc3f7' }}
                                        />
                                    </div>
                                ))}
                            </div>
                        );
                    })
                )}
            </div>
        </div>
    );
}