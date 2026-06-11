export interface AnimalConfig {
    tipo: string;
    habitat: string;       // 'Jurassic' | 'Jurassic / Sky' | 'Ocean' | etc
    anchoHitbox: number;
    altoHitbox: number;
    escala: number;
}

export const animalConfigs: AnimalConfig[] = [
    { tipo: 'dog', habitat: 'Urban', anchoHitbox: 21.5, altoHitbox: 33, escala: 3 },
    { tipo: 'hamster', habitat: 'Urban', anchoHitbox: 11, altoHitbox: 10.5, escala: 1.5 },
    { tipo: 'cat', habitat: 'Urban', anchoHitbox: 19, altoHitbox: 23, escala: 1.7 },
    { tipo: 'ornitorinco', habitat: 'Urban', anchoHitbox: 24, altoHitbox: 27, escala: 2.3 },
    { tipo: 'snail', habitat: 'Urban', anchoHitbox: 8, altoHitbox: 6.5, escala: 2.3 },
    { tipo: 'spider', habitat: 'Urban', anchoHitbox: 13.5, altoHitbox: 10, escala: 1.5 },
    { tipo: 'lovebird', habitat: 'Urban / Sky', anchoHitbox: 25, altoHitbox: 24, escala: 1.2 },
    { tipo: 'ant', habitat: 'Urban', anchoHitbox: 5, altoHitbox: 4, escala: 1.5 },
    { tipo: 'cockroach', habitat: 'Urban', anchoHitbox: 5, altoHitbox: 4, escala: 1.5 },
    { tipo: 'horse', habitat: 'Farm', anchoHitbox: 79, altoHitbox: 89, escala: 1.2 },
    { tipo: 'sheep', habitat: 'Farm', anchoHitbox: 40, altoHitbox: 40, escala: 1.1 },
    { tipo: 'pig', habitat: 'Farm', anchoHitbox: 53.5, altoHitbox: 35, escala: 3.2 },
    { tipo: 'cow', habitat: 'Farm', anchoHitbox: 50, altoHitbox: 50, escala: 1.1 },
    { tipo: 'duck', habitat: 'Farm', anchoHitbox: 25, altoHitbox: 23, escala: 1.2 },
    { tipo: 'rabbit', habitat: 'Farm', anchoHitbox: 16, altoHitbox: 13, escala: 2.1 },
    { tipo: 'fly', habitat: 'Sky', anchoHitbox: 6, altoHitbox: 4, escala: 2 },
    { tipo: 'shark', habitat: 'Ocean', anchoHitbox: 53, altoHitbox: 59.5, escala: 2.2 },
    { tipo: 'whale', habitat: 'Ocean', anchoHitbox: 200, altoHitbox: 120.5, escala: 2.5 },
    { tipo: 'penguin', habitat: 'Ocean', anchoHitbox: 12, altoHitbox: 16, escala: 3 },
    { tipo: 'dolphin', habitat: 'Ocean', anchoHitbox: 51.5, altoHitbox: 45, escala: 2.4 },
    { tipo: 'turtle marine', habitat: 'Ocean', anchoHitbox: 47, altoHitbox: 31.5, escala: 1.3 },
    { tipo: 'crab', habitat: 'Ocean', anchoHitbox: 15, altoHitbox: 10, escala: 3 },
    { tipo: 'melanocetus', habitat: 'Ocean', anchoHitbox: 20, altoHitbox: 12, escala: 3 },
    { tipo: 'godzilla', habitat: 'Ocean', anchoHitbox: 137.5, altoHitbox: 152, escala: 2.3 },
    { tipo: 'octopus', habitat: 'Ocean', anchoHitbox: 23.5, altoHitbox: 17.5, escala: 3 },
    { tipo: 'pterodactylus', habitat: 'Jurassic / Sky', anchoHitbox: 103, altoHitbox: 52, escala: 1.2 },
    { tipo: 't rex', habitat: 'Jurassic', anchoHitbox: 125.5, altoHitbox: 140, escala: 1.2 },
    { tipo: 'brachiosaurus', habitat: 'Jurassic', anchoHitbox: 200, altoHitbox: 200, escala: 2 },
    { tipo: 'velociraptor', habitat: 'Jurassic', anchoHitbox: 50, altoHitbox: 50, escala: 2 },
    { tipo: 'dodo', habitat: 'Jurassic', anchoHitbox: 23.5, altoHitbox: 27, escala: 2.2 },
    { tipo: 'pholidota', habitat: 'Savanna', anchoHitbox: 28.5, altoHitbox: 28, escala: 1.9 },
    { tipo: 'gorilla', habitat: 'Jungle', anchoHitbox: 65, altoHitbox: 72.5, escala: 1.9 },
    { tipo: 'chimpanzee', habitat: 'Jungle', anchoHitbox: 29, altoHitbox: 46, escala: 3.1 },
    { tipo: 'panda', habitat: 'Jungle', anchoHitbox: 66, altoHitbox: 50, escala: 2.5 },
    { tipo: 'cuckoo', habitat: 'Jungle / Sky', anchoHitbox: 24.5, altoHitbox: 22, escala: 3.2 },
    { tipo: 'hummingbird', habitat: 'Jungle / Sky', anchoHitbox: 34, altoHitbox: 24, escala: 2.5 },
    { tipo: 'crocodile', habitat: 'Swamp', anchoHitbox: 76, altoHitbox: 49, escala: 1.6 },
    { tipo: 'frog', habitat: 'Swamp', anchoHitbox: 10, altoHitbox: 8, escala: 2 },
    { tipo: 'capybara', habitat: 'Swamp', anchoHitbox: 40, altoHitbox: 38.5, escala: 1.7 },
    { tipo: 'beaver', habitat: 'Swamp', anchoHitbox: 40, altoHitbox: 38.5, escala: 1.7 },
    { tipo: 'otter', habitat: 'Swamp', anchoHitbox: 43.5, altoHitbox: 26.5, escala: 1.7 },
    { tipo: 'sandworm', habitat: 'Desert', anchoHitbox: 197.5, altoHitbox: 97, escala: 1.8 },
    { tipo: 'camel', habitat: 'Desert', anchoHitbox: 52, altoHitbox: 33.5, escala: 3 },
    { tipo: 'kangaroo', habitat: 'Desert', anchoHitbox: 78, altoHitbox: 65.5, escala: 3.5 },
    { tipo: 'phoenix', habitat: 'Legend / Sky', anchoHitbox: 70.5, altoHitbox: 72, escala: 2.1 },
];

// Helper para buscar config por tipo
export function getAnimalConfig(tipo: string): AnimalConfig | undefined {
    return animalConfigs.find(c => c.tipo.toLowerCase() === tipo.toLowerCase());
}