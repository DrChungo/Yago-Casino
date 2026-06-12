export const limite = {
    minX: 120,  
    maxX: 890,  
    minY: 235,  
    maxY: 500   
};


export const limitesCielo = {
    minX: 0,
    maxX: 1000,
    minY: 0,
    maxY: 167
}

export const obstaculos = [
    { id: 'caseta', x: 670, y: 200, ancho: 200, alto: 165 },
    { id: 'caseta-paja', x: 255, y: 200, ancho: 80, alto: 115 },
    { id: 'eno1', x: 585, y: 200, ancho: 80, alto: 105 },
    { id: 'eno2', x: 500, y: 200, ancho: 75, alto: 80 },
    { id: 'agua', x: 370, y: 252, ancho: 70, alto: 40 },
    { id: 'agua', x: 449, y: 213, ancho: 15, alto: 70 },
    { id: 'eno3', x: 837, y: 415, ancho: 35, alto: 80 },
    { id: 'eno4', x: 798, y: 470, ancho: 40, alto: 50 },
    { id: 'enoYagua', x: 130, y: 330, ancho: 50, alto: 185 },
    { id: 'agua1', x: 182, y: 457, ancho: 100, alto: 50 },
    { id: 'jarrones', x: 640, y: 310, ancho: 30, alto: 50 },
    { id: 'GUERTO', x: 140, y: 250, ancho: 84, alto: 68 },
];

// Esta funciï¿½n es la que usaremos luego para evitar que el animal salga
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