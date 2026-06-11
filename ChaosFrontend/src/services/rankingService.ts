const BASE = 'https://localhost:7101/api';

function getHeaders(): HeadersInit {
    const token = localStorage.getItem('token_casino');
    return {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
    };
}

export interface AnimalRanking {
    name: string;
    typeAnimal: string;
    age: number;
    weight: number;
    height: number;
    health: string;
    value: number;
    ownerId: string;
    rarity: boolean;
}

export interface UserData {
    id: string;
    name: string;
    email: string;
    wallet: number;
    isAlive: boolean;
}

export async function getTopAnimals(): Promise<AnimalRanking[]> {
    const res = await fetch(`${BASE}/Ranking/GetRankingAnimals`, {
        headers: getHeaders(),
    });
    if (!res.ok) throw new Error('Error al obtener el ranking');
    const json = await res.json();
    return json.data;
}

export async function getUserById(id: string): Promise<UserData | null> {
    try {
        const res = await fetch(`${BASE}/User/GetUserById?userId=${id}`, {
            headers: getHeaders(),
        });

        if (!res.ok) return null;

        const json = await res.json();
        console.log('USER RESPONSE:', json); // ← quitá esto cuando funcione
        return json.data ?? null;

    } catch (e) {
        console.error('Error fetching user:', e);
        return null;
    }
}

export async function getAnimalImages(): Promise<any[]> {
    const res = await fetch(`${BASE}/AnimalValueConfig/images`, {
        headers: getHeaders(),
    });
    if (!res.ok) throw new Error('Error al obtener imágenes');
    return res.json();
}