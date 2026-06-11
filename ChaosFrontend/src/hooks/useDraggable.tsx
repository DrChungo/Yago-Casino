import { useRef, useState } from "react";

export function useDraggable(initialX?: number, initialY?: number) {
    const [pos, setPos] = useState({
        x: initialX ?? 0,
        y: initialY ?? 0,
    });
    const [dragging, setDragging] = useState(false);
    const offset = useRef({ x: 0, y: 0 })
    const hasMoved = useRef(false);



    const onPointerDown = (e: React.PointerEvent) => {
        e.currentTarget.setPointerCapture(e.pointerId);
        hasMoved.current = false;
        offset.current = {
            x: e.clientX - pos.x,
            y: e.clientY - pos.y,
        };
        setDragging(true);
    }

    const onPointerMove = (e: React.PointerEvent) => {
        if (!dragging) return;
        hasMoved.current = true;
        setPos({
            x: e.clientX - offset.current.x,
            y: e.clientY - offset.current.y,
        })
    }
    const onPointerUp = () => setDragging(false);

    return { pos, dragging, onPointerDown, onPointerMove, onPointerUp }

}