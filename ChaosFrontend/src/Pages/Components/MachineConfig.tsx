import { useEffect, useState } from "react";
import "../../styles/MachineConfig.css";
import LoadingHamster from "./LoadingHamster";
export default function MachineConfig({ selectedMachine }: { selectedMachine: (config: any) => void }) {
    const Api_URL = import.meta.env.VITE_BASE_URL;
    const token = localStorage.getItem('token_casino');
     const [machineConfigs, setMachineConfigs] = useState<any[]>([]);
    const [selectedId, setSelectedId] = useState<number | null>(null);
    const fetchConfig = async () => {
        try{
        const response = await fetch(`${Api_URL}/api/configs/slot-game`,{
            method: 'GET',
            headers:{
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`


            }

        })
                    const data = await response.json();
                    setMachineConfigs(data);
                  
    }     catch(error){
        console.error("Error fetching machine configuration:", error);
    }
    }

    useEffect(() => {
        fetchConfig();
    }, [])

        const handleSelect = (config: any) => {
        setSelectedId(config.id);
        selectedMachine(config);
        }
    return(<>
        <section id="configuration">
        <h1>Select Machine</h1>
       
        {machineConfigs.length > 0 ? machineConfigs.map((config) => (
            <div key={config.id} onClick={() => handleSelect(config)}
                            
                            className={selectedId === config.id ? "selectedConfig" : ""}>
                <h3>{config.machineName}</h3>
                <p> Reels: {config.numberOfReels}</p>
                <p> Rows: {config.numberOfRows}</p>
                <p> tries: {config.payLines}</p>
            </div>
        )
            
        ): <LoadingHamster/>}

        
        </section>
    </>)
}