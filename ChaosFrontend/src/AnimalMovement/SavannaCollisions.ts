export const limite = {

    minX: 40,
    maxX: 930,
    minY: 170,
    maxY: 530,
};
export const limitesCielo = {
    minX: 0,
    maxX: 998,
    minY: 0,
    maxY: 105

}

export const obstaculos =
    [
        { id: 'caseta', x: 40, y: 170, ancho: 250, alto: 135 },
        { id: 'pozo', x: 291, y: 170, ancho: 102, alto: 115 },
        { id: 'pilar', x: 40, y: 307, ancho: 25, alto: 75 },
        { id: 'eno', x: 520, y: 170, ancho: 133, alto: 90 },
        { id: 'eno1', x: 655, y: 170, ancho: 100, alto: 120 },
        { id: 'arbol', x: 810, y: 170, ancho: 25, alto: 105 },
        { id: 'arbol1', x: 835, y: 395, ancho: 94, alto: 135 },
        { id: 'hojasArbol', x: 735, y: 348, ancho: 194, alto: 45 },
        { id: 'ramaArbol', x: 785, y: 395, ancho: 50, alto: 23 },
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