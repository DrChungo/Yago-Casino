export const limite = {

    minX: 70,
    maxX: 930,
    minY: 170,
    maxY: 520,
};
export const limitesCielo = {
    minX: 0,
    maxX: 998,
    minY: 0,
    maxY: 145

}

export const obstaculos =
    [
        { id: 'caseta', x: 70, y: 170, ancho: 240, alto: 350 },
        { id: 'pozo', x: 311, y: 170, ancho: 74, alto: 105 },
        { id: 'frutita', x: 501, y: 170, ancho: 240, alto: 115 },
        { id: 'pozo', x: 311, y: 170, ancho: 74, alto: 105 },
        { id: 'limite', x: 868, y: 170, ancho: 61, alto: 70 },
        { id: 'limite1', x: 883, y: 241, ancho: 46, alto: 70 },
        { id: 'limite2', x: 901, y: 312, ancho: 28, alto: 70 },
        { id: 'limite3', x: 919, y: 384, ancho: 10, alto: 50 },

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