const difficulties = [
    {
        key: "facil",
        label: "🟢 Fácil",
        description: "0-2 tiradas posibles",
        color: "#4CAF50",
    },
    {
        key: "medio",
        label: "🟡 Medio",
        description: "0-3 tiradas posibles",
        color: "#FF9800",
    },
    {
        key: "dificil",
        label: "🔴 Difícil",
        description: "0-4 tiradas posibles",
        color: "#F44336",
    },
];

const DifficultySelector = ({ selected, onChange, disabled }) => {
    return (
        <div className="difficulty-selector">
            <h3 className="difficulty-title">Elige tu dificultad</h3>
            <div className="difficulty-options">
                {difficulties.map((d) => (
                    <button
                        key={d.key}
                        className={`difficulty-btn ${selected === d.key ? "active" : ""}`}
                        style={{
                            "--accent": d.color,
                            borderColor: selected === d.key ? d.color : "transparent",
                        }}
                        onClick={() => onChange(d.key)}
                        disabled={disabled}
                    >
                        <span className="diff-label">{d.label}</span>
                        <span className="diff-desc">{d.description}</span>
                    </button>
                ))}
            </div>
        </div>
    );
};

export default DifficultySelector;