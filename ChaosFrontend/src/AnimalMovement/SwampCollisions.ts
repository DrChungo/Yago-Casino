export const limite = {

    minX: 80,
    maxX: 930,
    minY: 130,
    maxY: 495,
};
export const limitesCielo = {
    minX: 0,
    maxX: 998,
    minY: 0,
    maxY: 105

}

export const obstaculos =
    [
        { id: 'caseta', x: 80, y: 130, ancho: 240, alto: 155 },
        { id: 'pozo', x: 321, y: 130, ancho: 74, alto: 135 },
        { id: 'pilar', x: 740, y: 130, ancho: 190, alto: 105 },
        { id: 'pilar1', x: 861, y: 236, ancho: 69, alto: 105 },
        { id: 'pilar2', x: 864, y: 390, ancho: 65, alto: 105 },
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