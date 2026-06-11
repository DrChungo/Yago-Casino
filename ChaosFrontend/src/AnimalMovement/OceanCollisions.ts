export const limite = {
    minX: 0,
    maxX: 1000,
    minY: 0,
    maxY: 1000
}

// Esta función es la que usaremos luego para evitar que el animal salga
export const clamp = (valor: number, min: number, max: number): number => {
    return Math.max(min, Math.min(valor, max));
};

export const chequearColisionRectangulos = (rect1: any, rect2: any): boolean => {
    return (
        rect1.x < rect2.x + rect2.ancho &&
        rect1.x + rect1.ancho > rect2.x &&
        rect1.y < rect2.y + rect2.alto &&
        rect1.y + rect1.alto > rect2.y
    );
};