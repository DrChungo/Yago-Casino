import animalConfigsJson from './animalConfigs.json';

export interface AnimalConfig {
    tipo: string;
    habitat: string;       // 'Jurassic' | 'Jurassic / Sky' | 'Ocean' | etc
    anchoHitbox: number;
    altoHitbox: number;
    escala: number;
}

// Se inicializa con el JSON estático de fallback
export let animalConfigs: AnimalConfig[] = animalConfigsJson as AnimalConfig[];

// Permite actualizar las configuraciones dinámicamente en tiempo de ejecución (ej. cargando de la API)
export function setRuntimeAnimalConfigs(configs: AnimalConfig[]) {
    animalConfigs = configs;
}

// Helper para buscar config por tipo
export function getAnimalConfig(tipo: string): AnimalConfig | undefined {
    const raw = animalConfigs.find(c => {
        const t = c.tipo || (c as any).Tipo;
        return t?.toLowerCase() === tipo.toLowerCase();
    });
    if (!raw) return undefined;
    return {
        tipo: raw.tipo || (raw as any).Tipo || '',
        habitat: raw.habitat || (raw as any).Habitat || '',
        anchoHitbox: raw.anchoHitbox ?? (raw as any).AnchoHitbox ?? 40,
        altoHitbox: raw.altoHitbox ?? (raw as any).AltoHitbox ?? 40,
        escala: raw.escala ?? (raw as any).Escala ?? 1,
    };
}