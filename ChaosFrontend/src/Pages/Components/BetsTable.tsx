import '../../styles/BetsTable.css'
interface BetNamesType {
    selectedNumbers: { number: string; animalName: string }[];
    redNumbers: string;
    blackNumbers: string;
    firstDozen: string;
    secondDozen: string;
    thirdDozen: string;
    firstHalf: string;
    secondHalf: string;
    firstRow: string;
    secondRow: string;
    thirdRow: string;
    evenNumbers: string;
    oddNumbers: string;
}

interface BetsType {
    selectedNumbers: any[];
    redNumbers: string;
    blackNumbers: string;
    firstDozen: string;
    secondDozen: string;
    thirdDozen: string;
    firstHalf: string;
    secondHalf: string;
    firstRow: string;
    secondRow: string;
    thirdRow: string;
    evenNumbers: string;
    oddNumbers: string;
}

interface Props {
    bets: BetsType;
    betNames: BetNamesType;
}

const BET_LABELS: { key: keyof Omit<BetsType, "selectedNumbers">; label: string }[] = [
    { key: "firstDozen",   label: "1st 12"    },
    { key: "secondDozen",  label: "2nd 12"    },
    { key: "thirdDozen",   label: "3rd 12"    },
    { key: "firstHalf",    label: "1 to 18"   },
    { key: "secondHalf",   label: "19 to 36"  },
    { key: "evenNumbers",  label: "Even"      },
    { key: "oddNumbers",   label: "Odd"       },
    { key: "redNumbers",   label: "🟥 Red"    },
    { key: "blackNumbers", label: "⬛ Black"  },
    { key: "firstRow",     label: "1st Row"   },
    { key: "secondRow",    label: "2nd Row"   },
    { key: "thirdRow",     label: "3rd Row"   },
];

export default function BetsTable({ bets, betNames }: Props) {
    const hasAnyBet =
        bets.selectedNumbers.length > 0 ||
        BET_LABELS.some(({ key }) => bets[key] !== "00000000-0000-0000-0000-000000000000");

    if (!hasAnyBet) return null; 

    return (
        <div className="bets-table-container">
            <h3>Current Bets</h3>
            <table className="bets-table">
                <thead>
                    <tr>
                        <th>Category</th>
                        <th>Animal</th>
                    </tr>
                </thead>
                <tbody>
                   
                    {BET_LABELS.map(({ key, label }) =>
                        bets[key] !== "00000000-0000-0000-0000-000000000000" ? (
                            <tr key={key}>
                                <td>{label}</td>
                                <td>🐾 {betNames[key] || "Unknown"}</td>
                            </tr>
                        ) : null
                    )}

                   
                    {betNames.selectedNumbers.map(({ number, animalName }) => (
                        <tr key={`num-${number}`}>
                            <td>Number {number}</td>
                            <td>🐾 {animalName}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            
        </div>
    );
}