import { createContext, useState, useEffect } from 'react';

export const SceneContext = createContext();

export function SceneContextProvider({ children }) {

    const [isLoading, setIsLoading] = useState(true);
    const [splatData, setSplatData] = useState([]);
    const [buildingID, setBuildingID] = useState(1);

    /*  -- Load the JSON data
        If this implementation needs to be changed, make sure
        setSplatData() is used to populate splatData, and isLoading 
        is correctly updated when data loading completes or fails.*/
    useEffect(() => {
        const fetchData = async () => {
            try {
                await fetch('/modelData.json')
                    .then(res => res.json())
                    .then(data => {
                        setSplatData(data);
                    });
                setIsLoading(false);
            } catch (error) {
                console.error('Error fetching model data:', error);
                setIsLoading(false);
            }
        };

        fetchData();
    }, []);

    return (
        <SceneContext.Provider value={{
            isLoading,
            splatData,
            buildingID,
            setBuildingID
        }}>
            {children}
        </SceneContext.Provider>
    );
}