// src/components/ActiveDrinkEffects.tsx

import React, { useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import '../../styles/ActiveDrinkEffect.css';

const BASE_URL = 'https://localhost:7101';

const POLL_INTERVAL_MS = 5000; // ← re-fetch every 5 seconds

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
    } catch {
        return null;
    }
};

const EFFECT_META: Record<string, { icon: string; label: string; color: string }> = {
    GUARANTEED_WIN_1: { icon: '🍸', label: 'Guaranteed Win', color: '#f0c040' },
    DOUBLE_REWARDS_5: { icon: '🍹', label: 'Double Rewards', color: '#40c0f0' },
    PREVENT_LOSS_2: { icon: '🌴', label: 'Loss Prevention', color: '#40f080' },
};

interface DrinkEffect {
    effectType: string;
    roundsRemaining: number;
}

interface ActiveDrinkEffectsProps {
    disabled?: boolean;
    corner?: boolean;
}

const ActiveDrinkEffects: React.FC<ActiveDrinkEffectsProps> = ({
    disabled = false,
    corner = false,
}) => {
    const [effects, setEffects] = useState<DrinkEffect[]>([]);
    const [loading, setLoading] = useState(true);
    const [visible, setVisible] = useState(true);
    const [mounted, setMounted] = useState(true);

    // ── Fetch + Poll active effects ─────────────────────────────
    useEffect(() => {
        let cancelled = false; // ← prevents state update on unmounted component

        const fetchEffects = async () => {
            try {
                const token = localStorage.getItem('token_casino');
                if (!token) return;

                const payload = parseJwt(token);
                const userId =
                    payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
                    payload?.nameid ||
                    payload?.sub;

                if (!userId) return;

                const res = await fetch(`${BASE_URL}/${userId}/drink-effects`, {
                    headers: { Authorization: `Bearer ${token}` },
                });

                if (!res.ok) return;

                const data = await res.json();

                if (!cancelled) {
                    setEffects(Array.isArray(data) ? data : []);
                    setLoading(false);
                }
            } catch {
                if (!cancelled) setLoading(false);
            }
        };

        // ── Initial fetch immediately ───────────────────────────
        fetchEffects();

        // ── Then poll every POLL_INTERVAL_MS ───────────────────
        const interval = setInterval(fetchEffects, POLL_INTERVAL_MS);

        // ── Cleanup on unmount ──────────────────────────────────
        return () => {
            cancelled = true;
            clearInterval(interval);
        };
    }, []);

    // ── Auto-dismiss timer for disabled games ───────────────────
    useEffect(() => {
        if (!disabled) return;

        const fadeTimer = setTimeout(() => setVisible(false), 3000);
        const unmountTimer = setTimeout(() => setMounted(false), 3500);

        return () => {
            clearTimeout(fadeTimer);
            clearTimeout(unmountTimer);
        };
    }, [disabled]);

    if (loading || !mounted) return null;

    // ── [CHANGED] Strip the boost counter — games never show it ─
    const visibleEffects = effects.filter(e => e.effectType !== 'WALLET_BOOST_50');

    const activeClass = corner
        ? 'adf-panel adf-panel--active adf-panel--corner'
        : 'adf-panel adf-panel--active';

    const panel = disabled ? (
        <div className={`adf-panel adf-panel--disabled ${!visible ? 'adf-panel--hiding' : ''}`}>
            <span className="adf-disabled-icon">🚫🍹</span>
            <span className="adf-disabled-text">
                Bar drink effects are <strong>disabled</strong> in this game
            </span>
        </div>
    ) : visibleEffects.length === 0 ? null : (
        <div className={activeClass}>
            <div className="adf-header">
                <span className="adf-header-icon">✨</span>
                <span className="adf-header-label">ACTIVE BAR BUFFS</span>
            </div>
            <div className="adf-list">
                {visibleEffects.map((effect, i) => {
                    const meta = EFFECT_META[effect.effectType] ?? {
                        icon: '🍶',
                        label: effect.effectType,
                        color: '#ffffff',
                    };
                    return (
                        <div
                            key={i}
                            className="adf-chip"
                            style={{ '--chip-color': meta.color } as React.CSSProperties}
                        >
                            <span className="adf-chip-icon">{meta.icon}</span>
                            <span className="adf-chip-label">{meta.label}</span>
                            <span className="adf-chip-rounds">
                                {effect.roundsRemaining} round{effect.roundsRemaining !== 1 ? 's' : ''} left
                            </span>
                        </div>
                    );
                })}
            </div>
        </div>
    );

    if (!panel) return null;

    return createPortal(panel, document.body);
};

export default ActiveDrinkEffects;