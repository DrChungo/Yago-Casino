import { useEffect, useState } from "react";
import "../../styles/MachineConfig.css";
export default function MachineConfig({ selectedMachine }: { selectedMachine: (config: any) => void }) {
    const Api_URL = import.meta.env.VITE_BASE_URL;
    const token = localStorage.getItem('token_casino');
     const [machineConfigs, setMachineConfigs] = useState<any[]>([]);
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
    return(<>
        <section id="configuration">
        <h1>Machine</h1> 
        <h1>Configuration</h1>
        {machineConfigs.length > 0 ? machineConfigs.map((config) => (
            <div key={config.id} onClick={() => selectedMachine(config)}>
                <h3>{config.machineName}</h3>
                <p> Reels: {config.numberOfReels}</p>
                <p> Rows: {config.numberOfRows}</p>
                <p> tries: {config.payLines}</p>
            </div>
        )
            
        ): <p>Loading machine configuration...</p>}

        
        </section>
    </>)
}