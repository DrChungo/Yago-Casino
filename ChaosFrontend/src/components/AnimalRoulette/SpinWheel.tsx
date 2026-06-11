import { useEffect, useRef, useImperativeHandle, forwardRef } from "react";

const COLORS = [
    "#FF4D8D", "#7B2FBE", "#FFD700", "#FF6B35", "#4CAF50", "#2196F3",
    "#FF4D8D", "#7B2FBE", "#FFD700", "#FF6B35", "#4CAF50", "#2196F3",
];

const POINTER_ANGLE = -Math.PI / 2; // arriba = 270° en canvas
const EMOJI_FONT_STACK = `"Segoe UI Emoji", "Apple Color Emoji", "Noto Color Emoji", Arial, sans-serif`;

const SpinWheel = forwardRef(({ segments, size = 320, onSpinEnd }: any, ref: any) => {
    const canvasRef = useRef<HTMLCanvasElement>(null);
    const rotationRef = useRef(0);
    const animFrameRef = useRef<number | null>(null);
    const isSpinningRef = useRef(false);
    const onSpinEndRef = useRef(onSpinEnd);

    const segmentsRef = useRef(segments);
    segmentsRef.current = segments;

    useEffect(() => { onSpinEndRef.current = onSpinEnd; }, [onSpinEnd]);

    // ─── Draw ────────────────────────────────────────────────────────────────
    const draw = (rotation: number) => {
        const canvas = canvasRef.current;
        if (!canvas) return;
        const ctx = canvas.getContext("2d")!;
        const cx = size / 2;
        const cy = size / 2;
        const radius = size / 2 - 18;
        const innerRadius = 28;
        const segs = segmentsRef.current;
        const segAngle = (2 * Math.PI) / segs.length;

        ctx.clearRect(0, 0, size, size);

        // ── Outer gold ring ──────────────────────────────────────────────────
        ctx.beginPath();
        ctx.arc(cx, cy, radius + 8, 0, 2 * Math.PI);
        const goldGrad = ctx.createRadialGradient(cx, cy, radius, cx, cy, radius + 10);
        goldGrad.addColorStop(0, "#FFC200");
        goldGrad.addColorStop(1, "#FF9500");
        ctx.fillStyle = goldGrad;
        ctx.fill();

        // ── Bulbs ────────────────────────────────────────────────────────────
        const bulbCount = 16;
        for (let i = 0; i < bulbCount; i++) {
            const angle = (i / bulbCount) * 2 * Math.PI;
            const bx = cx + (radius + 4) * Math.cos(angle);
            const by = cy + (radius + 4) * Math.sin(angle);
            ctx.beginPath();
            ctx.arc(bx, by, 5, 0, 2 * Math.PI);
            ctx.fillStyle = i % 2 === 0 ? "#FFFDE7" : "#FFD740";
            ctx.shadowColor = "#FFFDE7";
            ctx.shadowBlur = 6;
            ctx.fill();
            ctx.shadowBlur = 0;
        }

        // ── Segments ─────────────────────────────────────────────────────────
        segs.forEach((seg: any, i: number) => {
            const startAngle = rotation + i * segAngle;
            const endAngle = startAngle + segAngle;
            const middleAngle = startAngle + segAngle / 2;

            ctx.beginPath();
            ctx.moveTo(cx, cy);
            ctx.arc(cx, cy, radius, startAngle, endAngle);
            ctx.closePath();
            ctx.fillStyle = COLORS[i % COLORS.length];
            ctx.fill();
            ctx.strokeStyle = "#fff";
            ctx.lineWidth = 2;
            ctx.stroke();

            const textRadius = radius * 0.65;
            const textX = cx + textRadius * Math.cos(middleAngle);
            const textY = cy + textRadius * Math.sin(middleAngle);

            ctx.save();
            ctx.translate(textX, textY);
            ctx.rotate(middleAngle + Math.PI / 2);
            ctx.textAlign = "center";
            ctx.textBaseline = "middle";
            ctx.fillStyle = "#fff";
            ctx.font = `bold ${seg.label.length > 6 ? 10 : 13}px Arial`;
            ctx.shadowColor = "rgba(0,0,0,0.6)";
            ctx.shadowBlur = 3;
            ctx.fillText(seg.label, 0, 0);
            ctx.restore();
        });

        // ── Inner gold border ────────────────────────────────────────────────
        ctx.beginPath();
        ctx.arc(cx, cy, innerRadius + 6, 0, 2 * Math.PI);
        ctx.fillStyle = "#FFD700";
        ctx.fill();

        // ── Center circle ────────────────────────────────────────────────────
        // ── Center circle ────────────────────────────────────────────────
        ctx.beginPath();
        ctx.arc(cx, cy, innerRadius, 0, 2 * Math.PI);
        const centerGrad = ctx.createRadialGradient(cx - 5, cy - 5, 2, cx, cy, innerRadius);
        centerGrad.addColorStop(0, "#5C35CC");
        centerGrad.addColorStop(1, "#3A1F8A");
        ctx.fillStyle = centerGrad;
        ctx.fill();

        // ── Emoji León ───────────────────────────────────────────────────
        ctx.font = `${innerRadius * 1.2}px serif`;
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillText("🦁", cx, cy +3);

        // ── Pointer ──────────────────────────────────────────────────────────
        const pointerTipX = cx + (radius + 2) * Math.cos(POINTER_ANGLE);
        const pointerTipY = cy + (radius + 2) * Math.sin(POINTER_ANGLE);
        const perpX = -Math.sin(POINTER_ANGLE);
        const perpY = Math.cos(POINTER_ANGLE);
        const outX = Math.cos(POINTER_ANGLE);
        const outY = Math.sin(POINTER_ANGLE);
        const baseHalf = 13;
        const height = 24;

        ctx.beginPath();
        ctx.moveTo(pointerTipX, pointerTipY);
        ctx.lineTo(
            pointerTipX - perpX * baseHalf + outX * height,
            pointerTipY - perpY * baseHalf + outY * height
        );
        ctx.lineTo(
            pointerTipX + perpX * baseHalf + outX * height,
            pointerTipY + perpY * baseHalf + outY * height
        );
        ctx.closePath();
        const pointerGrad = ctx.createLinearGradient(
            pointerTipX, pointerTipY,
            pointerTipX + outX * height,
            pointerTipY + outY * height
        );
        pointerGrad.addColorStop(0, "#9C27B0");
        pointerGrad.addColorStop(1, "#5C35CC");
        ctx.fillStyle = pointerGrad;
        ctx.fill();
        ctx.strokeStyle = "#fff";
        ctx.lineWidth = 1.5;
        ctx.stroke();
        ctx.beginPath();
        ctx.arc(
            pointerTipX + outX * height,
            pointerTipY + outY * height,
            6, 0, 2 * Math.PI
        );
        ctx.fillStyle = "#fff";
        ctx.fill();
    };

    // ─── Init ────────────────────────────────────────────────────────────────
    useEffect(() => {
        isSpinningRef.current = false;
        rotationRef.current = 0;
        if (animFrameRef.current) {
            cancelAnimationFrame(animFrameRef.current);
            animFrameRef.current = null;
        }
        draw(0);
        return () => {
            if (animFrameRef.current) cancelAnimationFrame(animFrameRef.current);
        };
    }, []);

    useEffect(() => {
        if (!isSpinningRef.current) draw(rotationRef.current);
    }, [segments]);

    // ─── Spin ────────────────────────────────────────────────────────────────
    useImperativeHandle(ref, () => ({
        spin: (targetIndex: number) => {
            if (animFrameRef.current) {
                cancelAnimationFrame(animFrameRef.current);
                animFrameRef.current = null;
                isSpinningRef.current = false;
            }
            if (isSpinningRef.current) return;
            isSpinningRef.current = true;

            const segs = segmentsRef.current;
            const segAngle = (2 * Math.PI) / segs.length;
            const minSpins = 5;
            const extraSpins = minSpins + Math.random() * 3; // 5..8 vueltas

            // ✅ FÓRMULA CORRECTA
            // Queremos que al terminar:
            //   rotFinal + targetIndex*segAngle + segAngle/2 = POINTER_ANGLE + k*2π
            //
            // El valor base del destino (sin vueltas extra):
            const baseTarget = POINTER_ANGLE - targetIndex * segAngle - segAngle / 2;

            // rotFinal debe ser >= startRotation + minSpins*2π
            // Calculamos k mínimo tal que baseTarget + k*2π >= startRotation + minSpins*2π
            const startRotation = rotationRef.current;
            const minFinal = startRotation + minSpins * 2 * Math.PI;

            // k = ceil((minFinal - baseTarget) / 2π)
            const k = Math.ceil((minFinal - baseTarget) / (2 * Math.PI));

            // Destino exacto con k vueltas
            const exactFinalRotation = baseTarget + k * 2 * Math.PI;

            // Añadimos fracción aleatoria de vuelta extra para variedad visual
            const randomExtra = Math.random() * 3 * 2 * Math.PI; // 0..3 vueltas extra
            const finalRotation = exactFinalRotation + randomExtra;
            // Pero randomExtra rompe la alineación → redondeamos al múltiplo de 2π más cercano
            // No: la alineación ya está garantizada por exactFinalRotation.
            // randomExtra debe ser múltiplo de 2π para no romperla:
            const extraFullSpins = Math.floor(Math.random() * 3); // 0, 1 o 2 vueltas extra
            const totalFinal = exactFinalRotation + extraFullSpins * 2 * Math.PI;

            const duration = 4000 + Math.random() * 1500;
            const startTime = performance.now();
            const easeOut = (t: number) => 1 - Math.pow(1 - t, 4);

            // Verificación previa
            const expectedNorm = ((totalFinal % (2 * Math.PI)) + 2 * Math.PI) % (2 * Math.PI);
            const expectedCenter = ((expectedNorm + targetIndex * segAngle + segAngle / 2)
                * 180 / Math.PI) % 360;
            const pointerDeg = ((POINTER_ANGLE * 180 / Math.PI) + 360) % 360;

            console.log(
                `🎡 seg[${targetIndex}]="${segs[targetIndex]?.label}"`,
                `| segs=${segs.length} | segAngle=${(segAngle * 180 / Math.PI).toFixed(1)}°`,
                `| k=${k} | extraFullSpins=${extraFullSpins}`,
                `| totalFinal=${(totalFinal * 180 / Math.PI).toFixed(1)}°`,
                `| expectedNorm=${(expectedNorm * 180 / Math.PI).toFixed(1)}°`,
                `| expectedCenter=${expectedCenter.toFixed(1)}°`,
                `| pointer=${pointerDeg.toFixed(1)}°`,
                `| pre-check=${Math.abs(expectedCenter - pointerDeg) < 0.01 ? '✅' : '❌'}`
            );

            const animate = (now: number) => {
                const elapsed = now - startTime;
                const progress = Math.min(elapsed / duration, 1);
                const eased = easeOut(progress);

                rotationRef.current = startRotation + (totalFinal - startRotation) * eased;
                draw(rotationRef.current);

                if (progress < 1) {
                    animFrameRef.current = requestAnimationFrame(animate);
                } else {
                    // ✅ Snap al destino exacto
                    rotationRef.current = totalFinal;
                    draw(rotationRef.current);

                    // Verificación final
                    const finalNorm = (
                        (rotationRef.current % (2 * Math.PI)) + 2 * Math.PI
                    ) % (2 * Math.PI);
                    const centerDeg = (
                        (finalNorm + targetIndex * segAngle + segAngle / 2) * 180 / Math.PI
                    ) % 360;

                    console.log(
                        `🏁 FIN: rotNorm=${(finalNorm * 180 / Math.PI).toFixed(2)}°`,
                        `| centro=${centerDeg.toFixed(2)}°`,
                        `| pointer=${pointerDeg.toFixed(2)}°`,
                        `| coincide=${Math.abs(centerDeg - pointerDeg) < 0.01 ? '✅' : '❌'}`
                    );

                    isSpinningRef.current = false;
                    animFrameRef.current = null;
                    onSpinEndRef.current?.(targetIndex);
                }
            };

            animFrameRef.current = requestAnimationFrame(animate);
        },
    }), []);

    return (
        <canvas
            ref={canvasRef}
            width={size}
            height={size}
            style={{ display: "block", margin: "0 auto" }}
        />
    );
});

SpinWheel.displayName = "SpinWheel";
export default SpinWheel;