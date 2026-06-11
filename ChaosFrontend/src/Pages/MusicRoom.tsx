import { useState, useRef, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";

import track1 from "../assets/Audios/casino_game_1.mp3";
import track2 from "../assets/Audios/casino_game_2.mp3";
import track4 from "../assets/Audios/casino_game_4.mp3";
import track3 from "../assets/Audios/casino_game_3.mp3";
import musicaIntro from "../assets/Audios/casino_landing_soundtrack.mp3";
import farm from "../assets/Audios/farm_soundtrack.m4a"
import track5 from "../assets/Audios/casino_disco_1.mp3";
import track6 from "../assets/Audios/casino_disco_2.mp3";
import track7 from "../assets/Audios/casino_disco_3.mp3";

import "../styles/MusicRoom.css";

// Variables para el juego de click en símbolos
interface FloatingSymbol {
  id:       number;
  x:        number; 
  y:        number;  
  size:     number;  
  emoji:    string;
  rotation: number;
  lifetime: number;
  dying:    boolean;
}

const SYMBOLS = ["🐶", "🐱", "🐼", "🦁", "🐭", "🦖", "🐦‍🔥", "🐷", "🐸", "🐯", "🦆", "🐋"];
let symbolIdCounter = 0;

interface Track {
  name: string;
  file: string;
  emoji: string;
}

const tracks: Track[] = [
  { name: "Casino Lobby", file: musicaIntro, emoji: "🎰" },
  { name: "Game Track 1", file: track1, emoji: "🎸" },
  { name: "Game Track 2", file: track2, emoji: "🎹" },
    { name: "Game Track 3", file: track3, emoji: "🎺" },
  { name: "Game Track 4", file: track4, emoji: "🥁" },
    { name: "Disco Track 1", file: track5, emoji: "🪩" },
    { name: "Disco Track 2", file: track6, emoji: "🕺" },
    { name: "Disco Track 3", file: track7, emoji: "💃"},
    { name: "Farm Theme", file: farm, emoji: "🌾" }
];

export default function MusicRoom() {
  const navigate = useNavigate();
  const audioRef = useRef<HTMLAudioElement>(null);

  const [currentIndex, setCurrentIndex] = useState<number>(0);
  const [isPlaying, setIsPlaying] = useState<boolean>(false);
  const [volume, setVolume] = useState<number>(0.7);
  const [progress, setProgress] = useState<number>(0);
  const [duration, setDuration] = useState<number>(0);
  const [isMuted, setIsMuted] = useState<boolean>(false);

  const [symbols,   setSymbols] = useState<FloatingSymbol[]>([]);
  const [gameScore, setGameScore] = useState<number>(0);
  const [missed,    setMissed] = useState<number>(0);
  const spawnIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const currentTrack = tracks[currentIndex];

  useEffect(() => {
        const audio = audioRef.current;
        if (!audio) return;

        audio.load();

        if (isPlaying) {
        audio.play().catch(() => {});
        }
  }, [currentIndex]);

  // Sincronizar volumen
  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = volume;
    }
  }, [volume]);

  const spawnSymbol = useCallback(() => {
  const newSymbol: FloatingSymbol = {
    id:       symbolIdCounter++,
    x:        Math.random() * 85 + 5, 
    y:        Math.random() * 75 + 5,   
    size:     Math.random() * 30 + 35,  
    emoji:    SYMBOLS[Math.floor(Math.random() * SYMBOLS.length)],
    rotation: Math.random() * 40 - 20,  
    lifetime: Math.random() * 2000 + 2000, 
    dying:    false,
  };

  setSymbols(prev => [...prev, newSymbol]);

    // Auto-destruir si no lo clickan
    setTimeout(() => {
        // Marcar como dying para animación de salida
        setSymbols(prev =>
        prev.map(s => s.id === newSymbol.id ? { ...s, dying: true } : s)
        );
        // Contar como fallado y eliminar
        setTimeout(() => {
        setSymbols(prev => {
            const existed = prev.find(s => s.id === newSymbol.id);
            if (existed) setMissed(m => m + 1); // Solo si no fue clickado
            return prev.filter(s => s.id !== newSymbol.id);
        });
        }, 400); // Duración animación de salida
    }, newSymbol.lifetime);

    }, []);

    //numero aleatorio entre 700 y 1500 ms para el spawn de símbolos
    const randomSpawnTime = Math.random() * 1000 + 700;

    // Arrancar/parar spawn según isPlaying
    useEffect(() => {
    if (isPlaying) {
        spawnIntervalRef.current = setInterval(spawnSymbol, randomSpawnTime);
    } else {
        if (spawnIntervalRef.current) {
        clearInterval(spawnIntervalRef.current);
        spawnIntervalRef.current = null;
        }
        setSymbols([]); // Limpiar pantalla al pausar
    }

    return () => {
        if (spawnIntervalRef.current) clearInterval(spawnIntervalRef.current);
    };
    }, [isPlaying, spawnSymbol]);

    // Manejar click en símbolo

    const handleSymbolClick = (id: number) => {
    // Animación de salida
    setSymbols(prev =>
        prev.map(s => s.id === id ? { ...s, dying: true } : s)
    );
    // Sumar punto y eliminar
    setTimeout(() => {
        setSymbols(prev => prev.filter(s => s.id !== id));
        setGameScore(prev => prev + 10);
    }, 300);
    };

  // Play/Pause

  const handlePlay = () => {
    audioRef.current?.play();
    setIsPlaying(true);
  };

  const handlePause = () => {
    audioRef.current?.pause();
    setIsPlaying(false);
  };

  // Seleccionar pista desde la lista
  const handleSelectTrack = (index: number) => {
    setCurrentIndex(index);
    setIsPlaying(true);
    setProgress(0);
  };

  const handlePrev = () => {
    const prevIndex = (currentIndex - 1 + tracks.length) % tracks.length;
    handleSelectTrack(prevIndex);
  };

  const handleNext = () => {
    const nextIndex = (currentIndex + 1) % tracks.length;
    handleSelectTrack(nextIndex);
  };

  const handleTimeUpdate = () => {
    const audio = audioRef.current;
    if (!audio) return;
    setProgress(audio.currentTime);
  };

  const handleLoadedMetadata = () => {
    if (audioRef.current) {
      setDuration(audioRef.current.duration);
    }
  };

  const handleSeek = (e: React.ChangeEvent<HTMLInputElement>) => {
    const time = Number(e.target.value);
    if (audioRef.current) {
      audioRef.current.currentTime = time;
      setProgress(time);
    }
  };

  const handleEnded = () => {
    // Auto-avanza a la siguiente pista
    handleNext();
  };

  const handleToggleMute = () => {
    if (audioRef.current) {
      audioRef.current.muted = !isMuted;
      setIsMuted(!isMuted);
    }
  };

  // Formatea segundos → mm:ss
  const formatTime = (seconds: number): string => {
    if (isNaN(seconds)) return "0:00";
    const m = Math.floor(seconds / 60);
    const s = Math.floor(seconds % 60).toString().padStart(2, "0");
    return `${m}:${s}`;
  };


  return (
    <div className="disco-container">

      <button className="btn-back-music" onClick={() => navigate("/lobby")}>
        ← Back to Lobby
      </button>

      <h1 className="music-room-title">DISCO ROOM</h1>

      <div className="music-player-card">

        <h2 className="track-name">{currentTrack.name}</h2>
        <p className="track-index">{currentIndex + 1} / {tracks.length}</p>

        <div className="progress-section">
          <span className="time-label">{formatTime(progress)}</span>
          <input
            type="range"
            className="progress-bar"
            min={0}
            max={duration || 0}
            value={progress}
            onChange={handleSeek}
          />
          <span className="time-label">{formatTime(duration)}</span>
        </div>

        <div className="player-controls">
          <button className="btn-control" onClick={handlePrev}>⏮</button>

          {isPlaying ? (
            <button className="btn-control btn-play-pause" onClick={handlePause}>⏸</button>
          ) : (
            <button className="btn-control btn-play-pause" onClick={handlePlay}>▶</button>
          )}

          <button className="btn-control" onClick={handleNext}>⏭</button>
          <button className="btn-control" onClick={handleToggleMute}>
            {isMuted ? "🔇" : "🔊"}
          </button>
        </div>

        <div className="volume-section">
          <span>🔈</span>
          <input
            type="range"
            className="volume-bar"
            min={0}
            max={1}
            step={0.01}
            value={volume}
            onChange={(e) => setVolume(Number(e.target.value))}
          />
          <span>🔉</span>
        </div>

        <audio
          ref={audioRef}
          src={currentTrack.file}
          onTimeUpdate={handleTimeUpdate}
          onLoadedMetadata={handleLoadedMetadata}
          onEnded={handleEnded}
        />
      </div>

      <div className="track-list">
        <h3 className="track-list-title">🎶 All Tracks</h3>
        {tracks.map((track, index) => (
          <button
            key={track.name}
            className={`track-item ${index === currentIndex ? "active" : ""}`}
            onClick={() => handleSelectTrack(index)}
          >
            <span className="track-item-emoji">{track.emoji}</span>
            <span className="track-item-name">{track.name}</span>
            {index === currentIndex && isPlaying && (
              <span className="track-playing-indicator">▶ Playing</span>
            )}
          </button>
        ))}
      </div>


    {isPlaying && (
    <div className="symbols-overlay">
        {symbols.map(symbol => (
        <button
            key={symbol.id}
            className={`floating-symbol ${symbol.dying ? "dying" : "spawning"}`}
            style={{
            left:      `${symbol.x}%`,
            top:       `${symbol.y}%`,
            fontSize:  `${symbol.size}px`,
            transform: `rotate(${symbol.rotation}deg)`,
            }}
            onClick={() => handleSymbolClick(symbol.id)}
        >
            {symbol.emoji}
        </button>
        ))}
    </div>
    )}

    {isPlaying && (
    <div className="symbol-game-hud">
        <span>🎯 <strong>{gameScore}</strong> pts</span>
        <span>💨 Missed: <strong>{missed}</strong></span>
        <span>🎵 Click the symbols!</span>
    </div>
    )}
    </div>
  );
}