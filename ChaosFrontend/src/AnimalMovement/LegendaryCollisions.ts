export const limite = {

    minX: 80,
    maxX: 860,
    minY: 180,
    maxY: 495,
};
export const limitesCielo = {
    minX: 0,
    maxX: 998,
    minY: 0,
    maxY: 145

}

export const obstaculos =
    [
        { id: 'caseta', x: 80, y: 180, ancho: 220, alto: 155 },
        { id: 'pozo', x: 301, y: 180, ancho: 70, alto: 120 },
        { id: 'eno', x: 717, y: 180, ancho: 142, alto: 90 },
        { id: 'eno1', x: 789, y: 272, ancho: 70, alto: 95 },
        { id: 'agua', x: 714, y: 440, ancho: 70, alto: 55 },
        { id: 'agua1', x: 274, y: 440, ancho: 70, alto: 55 },
        { id: 'casita', x: 80, y: 337, ancho: 90, alto: 158 },
    ];

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