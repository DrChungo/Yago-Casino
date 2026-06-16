import { useEffect, useState } from "react";
import '../styles/AnimalsUser.css'
import { useNavigate } from "react-router-dom";
import { useDraggable } from "../hooks/useDraggable";

export default function AnimalsUser({ betAnimal }: { betAnimal: (animal: any) => void }) {
    const token = localStorage.getItem('token_casino');
    const Api_URL = import.meta.env.VITE_BASE_URL;
    const [animals, setAnimals] = useState<any[]>([]);
    const [load, setLoad] = useState(false);
    const navigate = useNavigate();
    const [deployAnimals, setDeployAnimals] = useState(false);
    const [animalSelectedId, setAnimalSelectedId] = useState<number | null>(null);

    const toggleAnimalDeploy = () => {
        setDeployAnimals(prev => !prev)

    }

    const { pos, dragging, onPointerDown, onPointerMove, onPointerUp } = useDraggable(
        window.innerWidth - 300,
        200,
        240,
        400
    );
    const fetchAnimals = async () => {

        if (!token) {
            navigate("/");
            return;
        }
        const response = await fetch(`${Api_URL}/api/Animal/GetAnimalByOwnerId`, {

            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });
        const data = await response.json();
        filterAnimalsDeath(data.data);
        setLoad(true);

    }
    function filterAnimalsDeath(data: any[]) {
        const filteredAnimals = data.filter(animal => animal.isAvailable === true);
        setAnimals(filteredAnimals);
    }



    useEffect(() => {
        fetchAnimals();
    }, [])

    return (
        <section
            className={`ListAnimals ${dragging ? "ListAnimals--dragging" : ""}`}
            style={{
                position: "fixed",
                left: `${pos.x}px`,
                top: `${pos.y}px`,
                userSelect: "none",
            }}
            onPointerMove={onPointerMove}
            onPointerUp={onPointerUp}
        >
            <h1
                className="drag-handle"
                style={{ cursor: dragging ? "grabbing" : "grab" }}
                onPointerDown={onPointerDown}
            >
                <span style={{ fontSize: "1.7rem" }}>{dragging ? "✊" : "🖐"}</span>
                Your Animals
                <span
                    style={{ cursor: "pointer" }}
                    onPointerDown={(e) => e.stopPropagation()}
                    onClick={() => toggleAnimalDeploy()}>{deployAnimals ? "🡻" : "🡹"}
                </span>


            </h1>

            <div className="animals-scroll" style={deployAnimals ? { maxHeight: "0" } : {}}>
                {load
                    ? animals.map((animal) => (
                        <div key={animal.id} id={animalSelectedId === animal.id ? "animal-selected" : ""}
                            onClick={() => {
                                betAnimal(animal)
                                setAnimalSelectedId(animal.id);
                            }}>
                            <h3>{animal.name}</h3>
                            <p>Type: {animal.typeAnimal}</p>
                            <p>Age: {animal.age}y/o</p>
                            <p>Weight: {animal.weight}kg</p>
                            <p>Height: {animal.height}cm</p>
                            <p>Price: {animal.value}€</p>
                            <p>State: {animal.isAvailable ? "alive" : "death"}</p>
                        </div>
                    ))
                    : <p>Loading animals ...</p>
                }
            </div>
        </section>
    )
}