// ===========================
// LÍMITE SUELO — polígono libre
// ===========================
export const limitePoligono = [
    { x: 110, y: 460 },  // esquina 0
    { x: 630, y: 170 },  // esquina 1
    { x: 910, y: 400 },  // esquina 2
    { x: 500, y: 690 },  // esquina 3
];

export function obtenerEsquinasPoligono() {
    return limitePoligono;
}

// ✅ Ray casting — funciona con cualquier orientación del polígono
export function puntoEnPoligono(px: number, py: number): boolean {
    const poligono = limitePoligono;
    const n = poligono.length;
    let dentro = false;

    for (let i = 0, j = n - 1; i < n; j = i++) {
        const xi = poligono[i].x;
        const yi = poligono[i].y;
        const xj = poligono[j].x;
        const yj = poligono[j].y;

        const intersecta =
            yi > py !== yj > py &&
            px < ((xj - xi) * (py - yi)) / (yj - yi) + xi;

        if (intersecta) dentro = !dentro;
    }

    return dentro;
}

export function resolverLimitePoligono(animal: {
    x: number; y: number;
    vx: number; vy: number;
    hitboxAncho: number; hitboxAlto: number;
}): boolean {
    const cx = animal.x + animal.hitboxAncho / 2;
    const cy = animal.y + animal.hitboxAlto / 2;

    if (!puntoEnPoligono(cx, cy)) {
        // Deshacer el movimiento
        animal.x -= animal.vx;
        animal.y -= animal.vy;

        // Frenar completamente
        animal.vx = 0;
        animal.vy = 0;

        return true;
    }
    return false;
}

// ===========================
// LÍMITE CIELO
// ===========================
export const limitesCielo = {
    minX: 406,
    maxX: 1000,
    minY: 0,
    maxY: 150,
};

// ===========================
// OBSTÁCULOS
// ===========================
export interface Obstaculo {
    id: string;
    x: number;
    y: number;
    ancho: number;
    alto: number;
}

export const obstaculos: Obstaculo[] = [
    { id: 'arbol1', x: 110, y: 293, ancho: 300, alto: 305 },
    { id: 'limiteAbajo', x: 395, y: 599, ancho: 254, alto: 10 },
    { id: 'planta1', x: 558, y: 170, ancho: 220, alto: 120 },
    { id: 'planta2', x: 710, y: 220, ancho: 130, alto: 110 },
    { id: 'planta3', x: 825, y: 320, ancho: 130, alto: 80 },
];

// ===========================
// COLISIÓN ENTRE RECTÁNGULOS
// ===========================
export function chequearColisionRectangulos(
    a: { x: number; y: number; ancho: number; alto: number },
    b: { x: number; y: number; ancho: number; alto: number }
): boolean {
    return (
        a.x < b.x + b.ancho &&
        a.x + a.ancho > b.x &&
        a.y < b.y + b.alto &&
        a.y + a.alto > b.y
    );
}