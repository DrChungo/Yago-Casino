import React, { useEffect, useRef, useState } from 'react';
import { createPortal } from 'react-dom';
import { useNavigate } from 'react-router-dom';
import { LobbyHeader } from '../components/Lobby/LobbyHeader';

// ─── Images ───
import ginBottle from '../assets/CasinoImages/gin_bottle.png';
import amarettoBottle from '../assets/CasinoImages/amaretto_bottle.png';
import malibuBottle from '../assets/CasinoImages/malibu_bottle.png';
import sanFranciscoCocktail from '../assets/CasinoImages/san_francisco_coktail.png';

// ─── Audio ───
import barSoundtrack from '../assets/Audios/casino_bar_soundtrack.m4a';

// ─── Logo ───
import logoCasino from '../assets/CasinoImages/casino_logo.png';

import '../styles/BarLanding.css';

// ─────────────────────────────────────────────
// Constants
// ─────────────────────────────────────────────
const BASE_URL = 'https://localhost:7101';

const ADMIN_CREDENTIALS = {
    email: 'string',
    password: 'string',
};

const WALLET_BOOST_BASE_PRICE = 25_000_000;

// ─────────────────────────────────────────────
// JWT Decoder
// ─────────────────────────────────────────────
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

// ─────────────────────────────────────────────
// Drink Data
// ─────────────────────────────────────────────
type DrinkEffect =
    | 'WALLET_BOOST_50'
    | 'GUARANTEED_WIN_1'
    | 'DOUBLE_REWARDS_5'
    | 'PREVENT_LOSS_2'
    | 'COMING_SOON';

interface Drink {
    id: string;
    name: string;
    image: string;
    price: number;
    priceDisplay: string;
    effect: string;
    effectType: DrinkEffect;
}

const DRINKS: Drink[] = [
    {
        id: 'gin',
        name: 'YAG TONIC',
        image: ginBottle,
        price: 16500000,
        priceDisplay: '16.500.000',
        effect: 'Sharp focus — Guarantees a win for 1 round.',
        effectType: 'GUARANTEED_WIN_1',
    },
    {
        id: 'amaretto',
        name: 'YAGARETTO',
        image: amarettoBottle,
        price: 7750000,
        priceDisplay: '7.750.000',
        effect: 'Sweet luck — Doubles the rewards for 5 rounds.',
        effectType: 'DOUBLE_REWARDS_5',
    },
    {
        id: 'malibu',
        name: 'YAGIBU',
        image: malibuBottle,
        price: 8250000,
        priceDisplay: '8.250.000',
        effect: 'Tropical calm — Prevents animal losses for 2 rounds.',
        effectType: 'PREVENT_LOSS_2',
    },
    {
        id: 'san-francisco',
        name: 'SAN YAGCISCO',
        image: sanFranciscoCocktail,
        price: WALLET_BOOST_BASE_PRICE,
        priceDisplay: '25.000.000',
        effect: 'Wallet rush — Instantly increases your wallet value by 50%.',
        effectType: 'WALLET_BOOST_50',
    },
];

// ─────────────────────────────────────────────
// Effect Metadata
// ─────────────────────────────────────────────
const EFFECT_META: Record<string, { icon: string; label: string; color: string }> = {
    GUARANTEED_WIN_1: { icon: '🍸', label: 'Guaranteed Win', color: '#f0c040' },
    DOUBLE_REWARDS_5: { icon: '🍹', label: 'Double Rewards', color: '#40c0f0' },
    PREVENT_LOSS_2: { icon: '🌴', label: 'Loss Prevention', color: '#40f080' },
};

// ─────────────────────────────────────────────
// Component
// ─────────────────────────────────────────────
const BarLanding: React.FC = () => {
    const navigate = useNavigate();
    const audioRef = useRef<HTMLAudioElement | null>(null);

    const [purchasing, setPurchasing] = useState<string | null>(null);
    const [feedback, setFeedback] = useState<{ message: string; success: boolean } | null>(null);
    const feedbackTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

    const [activeEffects, setActiveEffects] = useState<any[]>([]);

    const [walletBoostPrice, setWalletBoostPrice] = useState<number>(WALLET_BOOST_BASE_PRICE);
    const [walletBoostPriceDisplay, setWalletBoostPriceDisplay] = useState<string>('25.000.000');

    // ── [NEW] Wallet display states ──────────────────────────────
    const [userWallet, setUserWallet] = useState<number>(0);
    const [walletLoading, setWalletLoading] = useState<boolean>(true);

    const showFeedback = (message: string, success: boolean) => {
        setFeedback({ message, success });
        if (feedbackTimerRef.current) clearTimeout(feedbackTimerRef.current);
        feedbackTimerRef.current = setTimeout(() => setFeedback(null), 5000);
    };

    // ─────────────────────────────────────────
    // [NEW] Fetch & display wallet balance
    // ─────────────────────────────────────────
    const fetchWalletBalance = async () => {
        const token = localStorage.getItem('token_casino');
        if (!token) return;

        const payload = parseJwt(token);
        if (!payload) return;

        const userId =
            payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
            payload.nameid ||
            payload.sub;

        if (!userId) return;

        setWalletLoading(true);
        try {
            const res = await fetch(`${BASE_URL}/api/User/AllUsers`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (res.ok) {
                const data = await res.json();
                const users = Array.isArray(data) ? data : (data.users ?? []);
                const matched = users.find(
                    (u: any) => u.id?.toLowerCase() === userId?.toLowerCase()
                );
                if (matched && typeof matched.wallet === 'number') {
                    setUserWallet(matched.wallet);
                }
            }
        } catch (err) {
            console.error('❌ fetchWalletBalance failed:', err);
        } finally {
            setWalletLoading(false);
        }
    };

    // ─────────────────────────────────────────
    // Audio Setup
    // ─────────────────────────────────────────
    useEffect(() => {
        const audio = new Audio(barSoundtrack);
        audio.loop = true;
        audio.volume = 0.5;
        audioRef.current = audio;

        const isMuted = localStorage.getItem('casino_music_muted') === 'true';
        audio.muted = isMuted;

        audio.play().catch((err) => {
            console.warn('Bar soundtrack autoplay blocked:', err.message);
        });

        document.documentElement.classList.add('scrollable-page');

        return () => {
            audio.pause();
            audio.currentTime = 0;
            document.documentElement.classList.remove('scrollable-page');
            if (feedbackTimerRef.current) clearTimeout(feedbackTimerRef.current);
        };
    }, []);

    // ─────────────────────────────────────────
    // Load active effects + wallet on mount
    // ─────────────────────────────────────────
    useEffect(() => {
        const loadEffects = async () => {
            const userToken = localStorage.getItem('token_casino');
            if (!userToken) return;

            const payload = parseJwt(userToken);
            const userId =
                payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
                payload?.nameid ||
                payload?.sub;

            if (!userId) return;

            const effects = await getActiveDrinkEffects(userId, userToken);
            setActiveEffects(effects);
        };

        loadEffects();
        fetchWalletBalance(); // ── [NEW] Load wallet on mount
    }, []);

    // ── Fetch the real wallet boost price on mount ───────────────
    useEffect(() => {
        const fetchBoostPrice = async () => {
            const userToken = localStorage.getItem('token_casino');
            if (!userToken) return;

            const payload = parseJwt(userToken);
            const userId =
                payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
                payload?.nameid ||
                payload?.sub;

            if (!userId) return;

            try {
                const res = await fetch(`${BASE_URL}/${userId}/wallet-boost-price`, {
                    headers: { Authorization: `Bearer ${userToken}` },
                });
                if (!res.ok) return;

                const data = await res.json();
                applyBoostPrice(data.currentPrice);
            } catch {
                // silently fall back to the static default
            }
        };

        fetchBoostPrice();
    }, []);

    // ── Helper: update both boost price states at once ───────────
    const applyBoostPrice = (rawPrice: number) => {
        setWalletBoostPrice(rawPrice);
        setWalletBoostPriceDisplay(
            Math.floor(rawPrice).toLocaleString('de-DE')
        );
    };

    // ─────────────────────────────────────────
    // Step 1 — Get Admin Token
    // ─────────────────────────────────────────
    const getAdminToken = async (): Promise<string | null> => {
        try {
            const res = await fetch(`${BASE_URL}/api/backoffice/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(ADMIN_CREDENTIALS),
            });
            if (!res.ok) return null;
            const data = await res.json();
            return data.token ?? null;
        } catch {
            return null;
        }
    };

    // ─────────────────────────────────────────
    // Step 2 — Patch Balance
    // ─────────────────────────────────────────
    const patchBalance = async (
        userId: string,
        amount: number,
        adminToken: string
    ): Promise<boolean> => {
        try {
            const res = await fetch(
                `${BASE_URL}/api/User/${userId}/balance?amount=${amount}`,
                {
                    method: 'PATCH',
                    headers: { Authorization: `Bearer ${adminToken}` },
                }
            );
            return res.ok;
        } catch {
            return false;
        }
    };

    // ─────────────────────────────────────────
    // POST /{userId}/drink-effect
    // ─────────────────────────────────────────
    const registerDrinkEffect = async (
        userId: string,
        effectType: string,
        userToken: string
    ): Promise<boolean> => {
        try {
            const res = await fetch(`${BASE_URL}/${userId}/drink-effect`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${userToken}`,
                },
                body: JSON.stringify({ effectType }),
            });
            return res.ok;
        } catch {
            return false;
        }
    };

    // ─────────────────────────────────────────
    // GET /{userId}/drink-effects
    // ─────────────────────────────────────────
    const getActiveDrinkEffects = async (
        userId: string,
        userToken: string
    ): Promise<any[]> => {
        try {
            const res = await fetch(`${BASE_URL}/${userId}/drink-effects`, {
                headers: { Authorization: `Bearer ${userToken}` },
            });
            if (!res.ok) return [];
            const data = await res.json();
            return data ?? [];
        } catch {
            return [];
        }
    };

    // ─────────────────────────────────────────
    // Step 3 — Get Current Wallet Value
    // ─────────────────────────────────────────
    const getUserWallet = async (
        userId: string,
        userToken: string
    ): Promise<number | null> => {
        try {
            const res = await fetch(`${BASE_URL}/api/User/AllUsers`, {
                headers: { Authorization: `Bearer ${userToken}` },
            });
            if (!res.ok) return null;

            const data = await res.json();
            const users = Array.isArray(data) ? data : (data.users ?? []);
            const matched = users.find(
                (u: any) => u.id?.toLowerCase() === userId?.toLowerCase()
            );

            return typeof matched?.wallet === 'number' ? matched.wallet : null;
        } catch (e) {
            console.error('❌ getUserWallet failed:', e);
            return null;
        }
    };

    // ── Fetch and apply the updated boost price ──────────────────
    const refreshBoostPrice = async (userId: string, userToken: string) => {
        try {
            const res = await fetch(`${BASE_URL}/${userId}/wallet-boost-price`, {
                headers: { Authorization: `Bearer ${userToken}` },
            });
            if (!res.ok) return;
            const data = await res.json();
            applyBoostPrice(data.currentPrice);
        } catch {
            // non-critical
        }
    };

    // ─────────────────────────────────────────
    // Main Purchase Handler
    // ─────────────────────────────────────────
    const handlePurchase = async (drink: Drink) => {
        setFeedback(null);

        const userToken = localStorage.getItem('token_casino');
        if (!userToken) {
            showFeedback('❌ You must be logged in to purchase.', false);
            return;
        }

        const payload = parseJwt(userToken);
        const userId =
            payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
            payload?.nameid ||
            payload?.sub;

        if (!userId) {
            showFeedback('❌ Could not identify user.', false);
            return;
        }

        setPurchasing(drink.id);

        try {
            const actualPrice =
                drink.effectType === 'WALLET_BOOST_50' ? walletBoostPrice : drink.price;

            const walletBefore = await getUserWallet(userId, userToken);
            if (walletBefore === null) {
                showFeedback('❌ Could not retrieve your wallet balance. Try again.', false);
                return;
            }

            if (walletBefore < actualPrice) {
                showFeedback(
                    `❌ Insufficient funds! You need 🪙${actualPrice.toLocaleString()} but only have 🪙${walletBefore.toLocaleString()}.`,
                    false
                );
                return;
            }

            const adminToken = await getAdminToken();
            if (!adminToken) {
                showFeedback('❌ Could not authenticate admin. Try again later.', false);
                return;
            }

            const deducted = await patchBalance(userId, -actualPrice, adminToken);
            if (!deducted) {
                showFeedback('❌ Purchase failed. Please try again.', false);
                return;
            }

            if (drink.effectType === 'WALLET_BOOST_50') {
                const boost = Math.floor(walletBefore * 0.5);

                if (boost <= 0) {
                    await patchBalance(userId, actualPrice, adminToken);
                    showFeedback('⚠️ Boost could not be calculated. Refunded.', false);
                    return;
                }

                const boosted = await patchBalance(userId, boost, adminToken);

                if (boosted) {
                    await registerDrinkEffect(userId, 'WALLET_BOOST_50', userToken);
                    await refreshBoostPrice(userId, userToken);

                    const updated = await getActiveDrinkEffects(userId, userToken);
                    setActiveEffects(updated);

                    const estimatedFinal = walletBefore - actualPrice + boost;
                    showFeedback(
                        `🍹 SAN YAGCISCO! Wallet boosted by 🪙${boost.toLocaleString()}! New balance ~🪙${estimatedFinal.toLocaleString()} 🤑`,
                        true
                    );
                } else {
                    await patchBalance(userId, actualPrice, adminToken);
                    showFeedback(
                        '⚠️ Boost could not be applied. Your purchase was refunded.',
                        false
                    );
                }

            } else {
                const registered = await registerDrinkEffect(
                    userId,
                    drink.effectType,
                    userToken
                );

                if (registered) {
                    showFeedback(
                        `🍸 ${drink.name} purchased! Effect "${drink.effect}" is now active. Good luck! 🎰`,
                        true
                    );

                    const updated = await getActiveDrinkEffects(userId, userToken);
                    setActiveEffects(updated);
                } else {
                    await patchBalance(userId, drink.price, adminToken);
                    showFeedback(
                        `⚠️ Could not activate ${drink.name} effect. Your purchase was refunded.`,
                        false
                    );
                }
            }

        } finally {
            setPurchasing(null);
            // ── [NEW] Refresh wallet display after every purchase attempt
            await fetchWalletBalance();
        }
    };

    // ─────────────────────────────────────────
    // Render
    // ─────────────────────────────────────────
    return (
        <div className="bar-landing-container">

            {/* Header */}
            <LobbyHeader
                logoCasino={logoCasino}
                onLogout={() => navigate('/')}
                isMuted={localStorage.getItem('casino_music_muted') === 'true'}
                onToggleMute={() => {
                    const current = localStorage.getItem('casino_music_muted') === 'true';
                    const next = !current;
                    localStorage.setItem('casino_music_muted', String(next));
                    if (audioRef.current) audioRef.current.muted = next;
                }}
                // ── [ADD THESE] ──────────────────────────────────────────────
                onNavigateToShop={() => navigate('/shop')}
                onNavigateToPen={() => navigate('/farm')}
                onNavigateToBar={() => navigate('/bar')}
                onNavigateToMusicRoom={() => navigate('/music-room')}
            />

            {/* ── [NEW] Fixed Wallet Container ─────────────────────────── */}
            <div className="bar-wallet-fixed">
                <div className="wallet-card">
                    <div className="wallet-chip-icon">🪙</div>
                    <span className="wallet-label">WALLET BALANCE</span>
                    {walletLoading ? (
                        <span className="wallet-amount loading">...</span>
                    ) : (
                        <span className="wallet-amount">
                            ${userWallet.toLocaleString()}
                        </span>
                    )}
                </div>
            </div>

            {/* Title */}
            <div className="bar-title-section">
                <h1 className="bar-neon-title">🍸 CASINO BAR</h1>
                <p className="bar-subtitle">
                    PICK YOUR POISON — GRANTS A UNIQUE EFFECT AFTER THE PURCHASE
                </p>
            </div>

            {/* Feedback Banner */}
            {feedback && createPortal(
                <div className={`bar-toast ${feedback.success ? 'bar-toast--success' : 'bar-toast--error'}`}>
                    {feedback.message}
                </div>,
                document.body
            )}

            {/* ── Active Buffs Panel ────────────────────────────── */}
            {(activeEffects.filter(e => e.effectType !== 'WALLET_BOOST_50').length > 0 || activeEffects.some(e => e.effectType === 'WALLET_BOOST_50')) && (
                <div className="bar-buffs-panel">
                    <div className="bar-buffs-header">
                        <span className="bar-buffs-header-icon">✨</span>
                        <span className="bar-buffs-header-label">ACTIVE BAR BUFFS</span>
                    </div>
                    <div className="bar-buffs-list">
                        {activeEffects
                            .filter(e => e.effectType !== 'WALLET_BOOST_50')
                            .map((effect, index) => {
                                const meta = EFFECT_META[effect.effectType] ?? {
                                    icon: '🍶',
                                    label: effect.effectType,
                                    color: '#ffffff',
                                };
                                return (
                                    <div
                                        key={index}
                                        className="bar-buffs-chip"
                                        style={{ '--chip-color': meta.color } as React.CSSProperties}
                                    >
                                        <span className="bar-buffs-chip-icon">{meta.icon}</span>
                                        <span className="bar-buffs-chip-label">{meta.label}</span>
                                        <span className="bar-buffs-chip-rounds">
                                            {effect.roundsRemaining} round{effect.roundsRemaining !== 1 ? 's' : ''} left
                                        </span>
                                    </div>
                                );
                            })}

                        {(() => {
                            const boostRecord = activeEffects.find(e => e.effectType === 'WALLET_BOOST_50');
                            if (!boostRecord) return null;
                            return (
                                <div
                                    className="bar-buffs-chip"
                                    style={{ '--chip-color': '#ff8c42' } as React.CSSProperties}
                                >
                                    <span className="bar-buffs-chip-icon">🍹</span>
                                    <span className="bar-buffs-chip-label">SAN YAGCISCO</span>
                                    <span className="bar-buffs-chip-rounds">
                                        purchased {boostRecord.roundsRemaining}×
                                    </span>
                                </div>
                            );
                        })()}
                    </div>
                </div>
            )}

            {/* Bottles Grid */}
            <div className="bar-bottles-grid">
                {DRINKS.map((drink) => {
                    const isBoost = drink.effectType === 'WALLET_BOOST_50';
                    const displayPrice = isBoost ? walletBoostPriceDisplay : drink.priceDisplay;
                    const drinkWithPrice = isBoost
                        ? { ...drink, price: walletBoostPrice }
                        : drink;

                    return (
                        <div key={drink.id} className="bar-bottle-card">
                            <div className="bar-bottle-image-wrapper">
                                <img src={drink.image} alt={drink.name} />
                            </div>
                            <span className="bar-bottle-name">{drink.name}</span>
                            <span className="bar-bottle-effect">{drink.effect}</span>
                            <button
                                className="bar-bottle-buy-btn"
                                disabled={purchasing !== null}
                                onClick={() => handlePurchase(drinkWithPrice)}
                            >
                                {purchasing === drink.id ? (
                                    '⏳ Processing...'
                                ) : (
                                    <><span>🪙 </span>{displayPrice}</>
                                )}
                            </button>
                        </div>
                    );
                })}
            </div>

            {/* Bottom Bar */}
            <div className="bar-bottom-bar">
                <button className="bar-back-btn" onClick={() => navigate('/lobby')}>
                    ← BACK TO LOBBY
                </button>
            </div>

        </div>
    );
};

export default BarLanding;