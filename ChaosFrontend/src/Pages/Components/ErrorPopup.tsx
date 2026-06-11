// components/ErrorPopup.tsx

interface ErrorPopupProps {
    message: string;
    onClose: () => void;
}

export default function ErrorPopup({ message, onClose }: ErrorPopupProps) {
    return (
        <div
            onClick={onClose}
            style={{
                position: "fixed",
                inset: 0,
                backgroundColor: "rgba(0, 0, 0, 0.85)",
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                zIndex: 9999,
                cursor: "pointer",
                backdropFilter: "blur(4px)",
            }}
        >
            <div
                onClick={(e) => e.stopPropagation()}
                style={{
                    background: "linear-gradient(135deg, #1a1a1a, #2d0000)",
                    border: "2px solid #ff4d4d",
                    borderRadius: "1.5rem",
                    padding: "2.5rem 3rem",
                    textAlign: "center",
                    maxWidth: "400px",
                    width: "90%",
                    boxShadow: "0 0 40px rgba(255, 77, 77, 0.5)",
                    animation: "popupIn 0.3s ease",
                }}
            >
                <div style={{ fontSize: "4rem", marginBottom: "1rem" }}>🚨</div>
                <p style={{
                    color: "#ff4d4d",
                    fontSize: "1.3rem",
                    fontWeight: "bold",
                    marginBottom: "1.5rem",
                    lineHeight: "1.5",
                }}>
                    {message}
                </p>
                <button
                    onClick={onClose}
                    style={{
                        background: "#ff4d4d",
                        color: "white",
                        border: "none",
                        borderRadius: "0.75rem",
                        padding: "0.75rem 2rem",
                        fontSize: "1rem",
                        fontWeight: "bold",
                        cursor: "pointer",
                        transition: "background 0.2s",
                    }}
                    onMouseEnter={e => (e.currentTarget.style.background = "#cc0000")}
                    onMouseLeave={e => (e.currentTarget.style.background = "#ff4d4d")}
                >
                    Got it!
                </button>
            </div>

            <style>{`
                @keyframes popupIn {
                    from { transform: scale(0.7); opacity: 0; }
                    to   { transform: scale(1);   opacity: 1; }
                }
            `}</style>
        </div>
    );
}