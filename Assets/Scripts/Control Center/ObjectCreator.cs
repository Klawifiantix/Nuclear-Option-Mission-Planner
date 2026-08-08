using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] GameObject OBJ_ObjectPrefab;

    [SerializeField] Transform TRANS_Building;
    [SerializeField] Transform TRANS_Vehicle;
    [SerializeField] Transform TRANS_Ships;
    [SerializeField] Transform TRANS_Aircraft;

    KnownObjects KnownObjects;
    PlacedObjects PlacedObjects;

    private void Awake()
    {
        KnownObjects = GetComponent<KnownObjects>();
        PlacedObjects = GameObject.Find("+---Objects---+").GetComponent<PlacedObjects>();
    }

    public void CreateObject(string Faction, bool Building, bool Vehicle, bool Ship, bool Aircraft, string ObjectName, Vector3 Position, Quaternion Rotation)
    {
        Position.y = 10f;

        Vector3 eulerRotation = Rotation.eulerAngles;
        Quaternion yOnlyRotation = Quaternion.Euler(0f, eulerRotation.y, 0f);

        GameObject NewObject = GameObject.Instantiate(OBJ_ObjectPrefab);
        NewObject.transform.position = Position;
        NewObject.transform.rotation = yOnlyRotation;

        Object_Info OI = NewObject.GetComponent<Object_Info>();

        if (Faction == "Boscali")
        {
            OI.BDF = true;
        }
        if (Faction == "Primeva")
        {
            OI.PALA = true;
        }

        if (Building)
        {
            NewObject.transform.SetParent(TRANS_Building);
            OI.ObjectStats = KnownObjects.BuildingStats(ObjectName);

            PlacedObjects.List_Buildings.Add(NewObject);
        }
        if (Vehicle)
        {
            NewObject.transform.rotation = Quaternion.identity;

            NewObject.transform.SetParent(TRANS_Vehicle);
            OI.ObjectStats = KnownObjects.VehicleStats(ObjectName);

            PlacedObjects.List_Vehicle.Add(NewObject);
        }
        if (Ship)
        {
            NewObject.transform.rotation = Quaternion.identity;

            NewObject.transform.SetParent(TRANS_Ships);
            OI.ObjectStats = KnownObjects.ShipsStats(ObjectName);

            PlacedObjects.List_Ships.Add(NewObject);
        }
        if (Aircraft)
        {
            NewObject.transform.SetParent(TRANS_Aircraft);
            OI.ObjectStats = KnownObjects.AircraftStats(ObjectName);

            PlacedObjects.List_Aircraft.Add(NewObject);
        }

        NewObject.transform.name = OI.ObjectStats.STR_Name_InGame;

        NewObject.SetActive(true);
    }
}
