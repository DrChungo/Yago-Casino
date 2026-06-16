import { useEffect, useRef, useState } from "react";

export function useDraggable(
    initialX?: number,
    initialY?: number,
    elementWidth = 280,   // ✅ ancho estimado del elemento
    elementHeight = 200   // ✅ alto estimado del elemento
) {
    const ratioRef = useRef({
        x: (initialX ?? 0) / window.innerWidth,
        y: (initialY ?? 0) / window.innerHeight,
    });

    // ✅ Función reutilizable para limitar la posición dentro de la pantalla
    const clamp = (x: number, y: number) => ({
        x: Math.max(0, Math.min(x, window.innerWidth - elementWidth)),
        y: Math.max(0, Math.min(y, window.innerHeight - elementHeight)),
    });

    const [pos, setPos] = useState(() =>
        clamp(initialX ?? 0, initialY ?? 0)
    );

    const [dragging, setDragging] = useState(false);
    const offset = useRef({ x: 0, y: 0 });
    const hasMoved = useRef(false);

    useEffect(() => {
        const handleResize = () => {
            const newPos = clamp(
                ratioRef.current.x * window.innerWidth,
                ratioRef.current.y * window.innerHeight
            );
            setPos(newPos);
        };

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    const onPointerDown = (e: React.PointerEvent) => {
        e.currentTarget.setPointerCapture(e.pointerId);
        hasMoved.current = false;
        offset.current = {
            x: e.clientX - pos.x,
            y: e.clientY - pos.y,
        };
        setDragging(true);
    };

    const onPointerMove = (e: React.PointerEvent) => {
        if (!dragging) return;
        hasMoved.current = true;

        const newPos = clamp(
            e.clientX - offset.current.x,
            e.clientY - offset.current.y
        );

        // ✅ Actualizamos el ratio con la posición ya limitada
        ratioRef.current = {
            x: newPos.x / window.innerWidth,
            y: newPos.y / window.innerHeight,
        };

        setPos(newPos);
    };

    const onPointerUp = () => setDragging(false);

    return { pos, dragging, onPointerDown, onPointerMove, onPointerUp };
}