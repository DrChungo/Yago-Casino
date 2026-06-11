export const limite = {

    minX: 100,
    maxX: 900,
    minY: 165,
    maxY: 515,
};
export const limitesCielo = {
    minX: 0,
    maxX: 998,
    minY: 0,
    maxY: 140

}

export const obstaculos =
    [
        { id: 'caseta', x: 100, y: 165, ancho: 230, alto: 180 },
        { id: 'pozo', x: 331, y: 165, ancho: 63, alto: 145 },
        { id: 'eno1', x: 836, y: 165, ancho: 63, alto: 220 },
        { id: 'eno2', x: 755, y: 165, ancho: 80, alto: 115 },
        { id: 'comedero1', x: 755, y: 465, ancho: 77, alto: 50 },
        { id: 'comedero2', x: 287, y: 469, ancho: 76, alto: 46 },
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