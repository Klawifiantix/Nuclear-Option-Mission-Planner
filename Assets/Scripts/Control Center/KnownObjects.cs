using UnityEngine;
using System.Collections.Generic;

public class KnownObjects : MonoBehaviour
{
    [SerializeField] List<UnitStats> List_Buildings = new List<UnitStats>();
    [SerializeField] List<UnitStats> List_Vehicles = new List<UnitStats>();
    [SerializeField] List<UnitStats> List_Ships = new List<UnitStats>();
    [SerializeField] List<UnitStats> List_Aircraft = new List<UnitStats>();

    public UnitStats BuildingStats(string Name)
    {
        for (int i = 0; i < List_Buildings.Count; i++)
        {
            string NameDev = List_Buildings[i].STR_Name_Development;
            if(NameDev == Name)
            {
                return List_Buildings[i];
            }
        }

        Debug.Log($"Keine Building Stats für {Name} gefunden");
        return null;
    }

    public UnitStats VehicleStats(string Name)
    {
        for (int i = 0; i < List_Vehicles.Count; i++)
        {
            string NameDev = List_Vehicles[i].STR_Name_Development;
            if (NameDev == Name)
            {
                return List_Vehicles[i];
            }
        }
        Debug.Log($"Keine Vehicle Stats für {Name} gefunden");
        return null;
    }

    public UnitStats ShipsStats(string Name)
    {
        for (int i = 0; i < List_Ships.Count; i++)
        {
            string NameDev = List_Ships[i].STR_Name_Development;
            if (NameDev == Name)
            {
                return List_Ships[i];
            }
        }

        Debug.Log($"Keine Ship Stats für {Name} gefunden");
        return null;
    }

    public UnitStats AircraftStats(string Name)
    {
        for (int i = 0; i < List_Aircraft.Count; i++)
        {
            string NameDev = List_Aircraft[i].STR_Name_Development;
            if (NameDev == Name)
            {
                return List_Aircraft[i];
            }
        }

        Debug.Log($"Keine Aircraft Stats für {Name} gefunden");
        return null;
    }
}
