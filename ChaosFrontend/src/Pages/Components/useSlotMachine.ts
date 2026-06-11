import { useRef, useState, useCallback } from "react";

interface SlotSymbol {
    id: string;
    symbolCode: string;
}

interface SlotConfig {
    numberOfReels: number;
    numberOfRows: number;
}

export function useSlotMachine(
    symbols: SlotSymbol[] | null,
    slotGameConf: SlotConfig | null
) {
    // Ahora es number[][] → [reelIndex][rowIndex]
    const [reelAngles, setReelAngles] = useState<number[][]>([]);
    const [isSpinning, setIsSpinning] = useState(false);

    // Refs también en 2D
    const currentAngles = useRef<number[][]>([]);

    // Map con clave "reel-row" para identificar cada cilindro
    const reelIntervals = useRef<Map<string, ReturnType<typeof setInterval>>>(new Map());
    const reelFrames = useRef<Map<string, number>>(new Map());

    const stoppedCount = useRef<number>(0);
    const expectedStops = useRef<number>(0);

    // Clave única por cilindro
    const key = (reelIndex: number, rowIndex: number) => `${reelIndex}-${rowIndex}`;

    const stopCylinder = useCallback((reelIndex: number, rowIndex: number) => {
        const k = key(reelIndex, rowIndex);

        const interval = reelIntervals.current.get(k);
        if (interval !== undefined) {
            clearInterval(interval);
            reelIntervals.current.delete(k);
        }

        const frame = reelFrames.current.get(k);
        if (frame !== undefined) {
            cancelAnimationFrame(frame);
            reelFrames.current.delete(k);
        }
    }, []);

    const stopAllCylinders = useCallback(() => {
        reelIntervals.current.forEach((interval) => clearInterval(interval));
        reelIntervals.current.clear();
        reelFrames.current.forEach((frame) => cancelAnimationFrame(frame));
        reelFrames.current.clear();
    }, []);

    const calcFinalAngle = useCallback((
        targetSymbolCode: string,
        currentAngle: number
    ): number => {
        if (!symbols || symbols.length === 0) return currentAngle;

        const n = symbols.length;
        const anglePerSymbol = 360 / n;
        const symbolIndex = symbols.findIndex(s => s.symbolCode === targetSymbolCode);
        if (symbolIndex === -1) return currentAngle;

        const naturalAngle = -(anglePerSymbol * symbolIndex);
        const normalizedCurrent = ((currentAngle % 360) + 360) % 360;
        const naturalNormalized = ((naturalAngle % 360) + 360) % 360;

        let diff = naturalNormalized - normalizedCurrent;
        if (diff > 0) diff -= 360;

        return currentAngle + diff - (360 * 5);
    }, [symbols]);

    const animateCylinderToAngle = useCallback((
        reelIndex: number,
        rowIndex: number,
        fromAngle: number,
        toAngle: number,
        onComplete: () => void
    ) => {
        const duration = 800;
        const startTime = performance.now();
        const k = key(reelIndex, rowIndex);

        const animate = (now: number) => {
            const elapsed = now - startTime;
            const progress = Math.min(elapsed / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);
            const angle = fromAngle + (toAngle - fromAngle) * eased;

            currentAngles.current[reelIndex][rowIndex] = angle;

            setReelAngles(prev => {
                const updated = prev.map(r => [...r]);
                updated[reelIndex][rowIndex] = angle;
                return updated;
            });

            if (progress < 1) {
                reelFrames.current.set(k, requestAnimationFrame(animate));
            } else {
                currentAngles.current[reelIndex][rowIndex] = toAngle;
                setReelAngles(prev => {
                    const updated = prev.map(r => [...r]);
                    updated[reelIndex][rowIndex] = toAngle;
                    return updated;
                });
                reelFrames.current.delete(k);
                onComplete();
            }
        };

        reelFrames.current.set(k, requestAnimationFrame(animate));
    }, []);

    const startSpinning = useCallback((numberOfReels: number, numberOfRows: number) => {
        if (!symbols || symbols.length === 0) return;

        stopAllCylinders();
        stoppedCount.current = 0;
        expectedStops.current = 0;

        // Inicializa la matriz 2D si cambia la configuración
        const needsReset =
            currentAngles.current.length !== numberOfReels ||
            currentAngles.current[0]?.length !== numberOfRows;

        if (needsReset) {
            currentAngles.current = Array.from({ length: numberOfReels }, () =>
                new Array(numberOfRows).fill(0)
            );
            setReelAngles(Array.from({ length: numberOfReels }, () =>
                new Array(numberOfRows).fill(0)
            ));
        }

        setIsSpinning(true);

        // Arranca un intervalo por cada cilindro [reel][row]
        Array.from({ length: numberOfReels }).forEach((_, reelIndex) => {
            Array.from({ length: numberOfRows }).forEach((_, rowIndex) => {
                const k = key(reelIndex, rowIndex);

                // Pequeño offset visual entre filas para que no sean idénticas
                const rowOffset = rowIndex * 3;

                const interval = setInterval(() => {
                    currentAngles.current[reelIndex][rowIndex] -= (10 + rowOffset);
                    setReelAngles(prev => {
                        const updated = prev.map(r => [...r]);
                        updated[reelIndex][rowIndex] = currentAngles.current[reelIndex][rowIndex];
                        return updated;
                    });
                }, 16);

                reelIntervals.current.set(k, interval);
            });
        });

    }, [symbols, stopAllCylinders]);

    const stopSpinning = useCallback((resultReels: string[][]) => {
        // resultReels[reelIndex][rowIndex] = symbolCode
        if (!slotGameConf || !symbols) return;

        const numberOfRows = resultReels[0].length;
        const totalCylinders = resultReels.length * numberOfRows;

        stoppedCount.current = 0;
        expectedStops.current = totalCylinders;

        resultReels.forEach((reel, reelIndex) => {
            reel.forEach((targetSymbol, rowIndex) => {

                // Escalonado: primero por reel, luego pequeño delay entre filas
                const delay = reelIndex * 300 + rowIndex * 80;

                setTimeout(() => {
                    stopCylinder(reelIndex, rowIndex);

                    const fromAngle = currentAngles.current[reelIndex][rowIndex];
                    const finalAngle = calcFinalAngle(targetSymbol, fromAngle);

                    animateCylinderToAngle(
                        reelIndex,
                        rowIndex,
                        fromAngle,
                        finalAngle,
                        () => {
                            stoppedCount.current += 1;
                            if (stoppedCount.current >= expectedStops.current) {
                                setIsSpinning(false);
                            }
                        }
                    );
                }, delay);
            });
        });

    }, [calcFinalAngle, animateCylinderToAngle, stopCylinder, slotGameConf, symbols]);

    return {
        reelAngles,
        isSpinning,
        startSpinning,
        stopSpinning,
    };
}