import { useEffect, useRef } from 'react';

interface AudioOptions {
    loop?: boolean;
    volume?: number;
}

const globalAudios: Record<string, HTMLAudioElement> = {};
let audioDesbloqueado = false;

export const useAudio = () => {
    // Store audio instances using refs to avoid duplicates across renders
    const audiosRef = useRef<Record<string, HTMLAudioElement>>(globalAudios);

    useEffect(() => {
        if (audioDesbloqueado) return;
        // Global listener to unlock AudioContext blocked by browser autoplay policies
        const habilitarAudioNavegador = () => {
            const ContextoAudio = window.AudioContext || (window as any).webkitAudioContext;
            if (ContextoAudio) {
                const contexto = new ContextoAudio();
                if (contexto.state === 'suspended') {
                    contexto.resume();
                }
            }

            // On the user's first interaction, retry playing any paused unmuted tracks
            Object.values(audiosRef.current).forEach((audio) => {
                if (audio.paused && !audio.muted && audio.loop) {
                    audio.play().catch(() => {});
                }
            });
            
            audioDesbloqueado = true;
            // Once the user interacts, remove the listener
            window.removeEventListener('click', habilitarAudioNavegador);
        };

        window.addEventListener('click', habilitarAudioNavegador);
        return () => window.removeEventListener('click', habilitarAudioNavegador);
    }, []);

    /**
     * Play an audio file.
     */

    const reproducir = (ruta: string, opciones: AudioOptions = {}) => {
        // Si ya existe Y ya está sonando, no hacer nada
        if (audiosRef.current[ruta]) {
            const audioExistente = audiosRef.current[ruta];
            if (!audioExistente.paused) return; // Ya está reproduciéndose
            
            // Si existe pero está pausado, simplemente reanudar
            audioExistente.play().catch((error) => {
                console.warn("Audio autoplay bloqueado:", error.message);
            });
            return;
        }

        // Solo crear instancia si no existe en el singleton
        const nuevoAudio = new Audio(ruta);
        nuevoAudio.loop = opciones.loop ?? false;
        nuevoAudio.volume = opciones.volume ?? 1.0;
        audiosRef.current[ruta] = nuevoAudio;

        nuevoAudio.play().catch((error) => {
            console.warn("Audio autoplay bloqueado:", error.message);
        });
    };

    const detener = (ruta: string) => {
        const audio = audiosRef.current[ruta];
        if (audio) {
            audio.pause();
            audio.currentTime = 0;
        }
    };

    const detenerTodo = () => {
        Object.values(audiosRef.current).forEach((audio) => {
            audio.pause();
            audio.currentTime = 0;
        });
    };

    const silenciar = (ruta: string, silenciar: boolean) => {
        const audio = audiosRef.current[ruta];
        if (audio) {
            audio.muted = silenciar;
        }
    };

    return { reproducir, detener, detenerTodo, silenciar };
};
