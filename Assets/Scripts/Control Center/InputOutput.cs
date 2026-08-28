using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#region Window File Dialog Structs & Imports
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public string filter = null;
    public string customFilter = null;
    public int maxCustomFilter = 0;
    public int filterIndex = 0;
    public string file = null;
    public int maxFile = 0;
    public string fileTitle = null;
    public int maxFileTitle = 0;
    public string initialDir = null;
    public string title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public string defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public string templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class WinDll
{
    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
}
#endregion

#region JSON Data Structures
[Serializable]
public class MapKeyData
{
    public string Path;
}

[Serializable]
public class UnitData
{
    public string type;
    public string faction;
    public Vector3 globalPosition;
    public Quaternion rotation;
}

[Serializable]
public class WaypointData
{
    public Vector3 position;
}

[Serializable]
public class SortieData
{
    public int sortieIndex;
    public float travelSpeed;
    public WaypointData[] waypoints;
    public int[] assignedAircraftIndices;
}

[Serializable]
public class ArrowData
{
    public float scaleFaktor;
    public float colorR;
    public float colorG;
    public float colorB;
    public WaypointData[] points;
}

[Serializable]
public class AreaData
{
    public float colorR;
    public float colorG;
    public float colorB;
    public WaypointData[] points;
}

[Serializable]
public class CameraData
{
    public Vector3 position;
    public Quaternion rotation;
    public float orthographicSize;
    public float ProjectionSize;
}

[Serializable]
public class SaveDataContainer
{
    public MapKeyData MapKey;
    public UnitData[] buildings;
    public UnitData[] vehicles;
    public UnitData[] ships;
    public UnitData[] aircraft;
    public SortieData[] sorties;
    public ArrowData[] arrows;
    public AreaData[] areas;
    public CameraData cameraData;
}
#endregion

public class InputOutput : MonoBehaviour
{
    [SerializeField] private TMP_Text BTN_LoadText;
    [SerializeField] Camera MainCamera;
    [SerializeField] Transform TRANS_Camera;
    [SerializeField] Camera_Movement Camera_Movement;

    private ObjectCreator ObjectCreator;
    private PlacedObjects PlacedObjects;
    private SortieManager SortieManager;
    private SortieWing SortieWing;
    private Map_Setup Map_Setup;
    private ArrowsManager ArrowsManager;
    private AreasManager AreasManager;

    [SerializeField] bool isMissionLoaded = false;

    bool LoadedTempData;
    int FramesToWait = 0;
    int ActualFrame;

    private void Awake()
    {
        ObjectCreator = GetComponent<ObjectCreator>();

        GameObject mapContainer = GameObject.Find("+---MAP---+");
        if (mapContainer != null)
        {
            Map_Setup = mapContainer.GetComponent<Map_Setup>();
        }

        GameObject objectsContainer = GameObject.Find("+---Objects---+");
        if (objectsContainer != null)
        {
            PlacedObjects = objectsContainer.GetComponent<PlacedObjects>();
        }

        GameObject sortieContainer = GameObject.Find("+---Sortie_Manager---+");
        if (sortieContainer != null)
        {
            SortieManager = sortieContainer.GetComponent<SortieManager>();
            SortieWing = sortieContainer.GetComponent<SortieWing>();
        }

        GameObject notesContainer = GameObject.Find("+---Notes---+");
        if (notesContainer != null)
        {
            ArrowsManager = notesContainer.GetComponent<ArrowsManager>();
            AreasManager = notesContainer.GetComponent<AreasManager>();
        }
    }

    private void Start()
    {
        LoadedTempData = false;
    }

    private void Update()
    {
        if (!LoadedTempData)
        {
            if (ActualFrame < FramesToWait)
            {
                ActualFrame++;
            }
            else
            {
                ActualFrame = FramesToWait;
                LoadData(true);
                LoadedTempData = true;
            }
        }
    }

    

    public void SaveData(bool Temporary)
    {
        string filePath = "";

        if (Temporary)
        {
            filePath = Path.Combine(Application.persistentDataPath, "temp_mission_save.json");
        }
        else
        {
            filePath = SaveFileDialog();
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("SaveData abgebrochen: Kein Speicherpfad ausgewählt.");
            return;
        }

        try
        {
            SaveDataContainer dataContainer = new SaveDataContainer();

            dataContainer.MapKey = new MapKeyData();
            if (Map_Setup != null)
            {
                int currentMapIndex = Map_Setup.GetCurrentMapIndex();
                dataContainer.MapKey.Path = GetPathFromMapIndex(currentMapIndex);
            }

            if (PlacedObjects != null)
            {
                dataContainer.buildings = ExtractUnitData(PlacedObjects.List_Buildings);
                dataContainer.vehicles = ExtractUnitData(PlacedObjects.List_Vehicle);
                dataContainer.ships = ExtractUnitData(PlacedObjects.List_Ships);
                dataContainer.aircraft = ExtractUnitData(PlacedObjects.List_Aircraft);
            }

            dataContainer.sorties = ExtractSortieData();
            dataContainer.arrows = ExtractArrowData();
            dataContainer.areas = ExtractAreaData();

            if (MainCamera != null && Temporary)
            {
                dataContainer.cameraData = new CameraData
                {
                    position = MainCamera.transform.position,
                    rotation = MainCamera.transform.rotation,
                    orthographicSize = MainCamera.orthographicSize,
                    ProjectionSize = Camera_Movement.Projection_Size,
                };
            }

            string jsonContent = JsonUtility.ToJson(dataContainer, true);
            File.WriteAllText(filePath, jsonContent);

            Debug.Log($"Daten erfolgreich in {filePath} gespeichert.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fehler beim Speichern der Datei: {ex.Message}");
        }
    }

    private UnitData[] ExtractUnitData(List<GameObject> objectList)
    {
        if (objectList == null || objectList.Count == 0)
        {
            return new UnitData[0];
        }

        List<UnitData> unitDataList = new List<UnitData>();

        foreach (GameObject obj in objectList)
        {
            if (obj != null)
            {
                Object_Info oi = obj.GetComponent<Object_Info>();
                if (oi != null && oi.ObjectStats != null)
                {
                    UnitData data = new UnitData();
                    data.type = oi.ObjectStats.STR_Name_Development;

                    if (oi.BDF)
                    {
                        data.faction = "Boscali";
                    }
                    else if (oi.PALA)
                    {
                        data.faction = "Primeva";
                    }
                    else
                    {
                        data.faction = "";
                    }

                    data.globalPosition = obj.transform.position;
                    data.rotation = obj.transform.rotation;

                    unitDataList.Add(data);
                }
            }
        }

        return unitDataList.ToArray();
    }

    private SortieData[] ExtractSortieData()
    {
        if (SortieManager == null)
        {
            return new SortieData[0];
        }

        System.Reflection.FieldInfo fieldInfoInfo = typeof(SortieManager).GetField("List_Sorties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        List<Sortie> listSorties = fieldInfoInfo != null ? fieldInfoInfo.GetValue(SortieManager) as List<Sortie> : null;

        if (listSorties == null || listSorties.Count == 0)
        {
            return new SortieData[0];
        }

        System.Reflection.FieldInfo mapIconsField = typeof(SortieWing).GetField("List_Sprites_MapIcons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        List<Sprite> listMapIcons = mapIconsField != null ? mapIconsField.GetValue(SortieWing) as List<Sprite> : null;

        System.Reflection.FieldInfo sortiesSpritesField = typeof(SortieWing).GetField("List_Sprites_Sorties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        List<List<Sprite>> listSpritesSorties = sortiesSpritesField != null ? sortiesSpritesField.GetValue(SortieWing) as List<List<Sprite>> : null;

        List<SortieData> sortieDataList = new List<SortieData>();

        for (int i = 0; i < listSorties.Count; i++)
        {
            Sortie currentSortie = listSorties[i];
            if (currentSortie != null && currentSortie.List_Waypoints != null && currentSortie.List_Waypoints.Count > 0)
            {
                SortieData sData = new SortieData();
                sData.sortieIndex = i;
                sData.travelSpeed = currentSortie.TravelSpeed;

                List<WaypointData> wpList = new List<WaypointData>();
                foreach (GameObject wpObj in currentSortie.List_Waypoints)
                {
                    if (wpObj != null)
                    {
                        WaypointData wpData = new WaypointData();
                        wpData.position = wpObj.transform.position;
                        wpList.Add(wpData);
                    }
                }

                sData.waypoints = wpList.ToArray();

                List<int> iconIndices = new List<int>();
                if (listSpritesSorties != null && i < listSpritesSorties.Count && listSpritesSorties[i] != null && listMapIcons != null)
                {
                    foreach (Sprite sprite in listSpritesSorties[i])
                    {
                        if (sprite != null)
                        {
                            int iconIdx = listMapIcons.IndexOf(sprite);
                            if (iconIdx >= 0)
                            {
                                iconIndices.Add(iconIdx);
                            }
                        }
                    }
                }

                sData.assignedAircraftIndices = iconIndices.ToArray();
                sortieDataList.Add(sData);
            }
        }

        return sortieDataList.ToArray();
    }

    private ArrowData[] ExtractArrowData()
    {
        if (ArrowsManager == null)
        {
            return new ArrowData[0];
        }

        List<GameObject> activeArrows = ArrowsManager.List_ActiveArrows;

        if (activeArrows == null || activeArrows.Count == 0)
        {
            return new ArrowData[0];
        }

        List<ArrowData> arrowDataList = new List<ArrowData>();

        foreach (GameObject arrowObj in activeArrows)
        {
            if (arrowObj != null)
            {
                Arrow arrowComp = arrowObj.GetComponent<Arrow>();
                if (arrowComp != null && arrowComp.List_LR_Points != null && arrowComp.List_LR_Points.Count > 0)
                {
                    ArrowData aData = new ArrowData();
                    aData.scaleFaktor = arrowComp.Scale_Faktor;
                    aData.colorR = arrowComp.COL_Unselected_R;
                    aData.colorG = arrowComp.COL_Unselected_G;
                    aData.colorB = arrowComp.COL_Unselected_B;

                    List<WaypointData> pointList = new List<WaypointData>();
                    foreach (Vector3 pos in arrowComp.List_LR_Points)
                    {
                        WaypointData pData = new WaypointData();
                        pData.position = pos;
                        pointList.Add(pData);
                    }

                    aData.points = pointList.ToArray();
                    arrowDataList.Add(aData);
                }
            }
        }

        return arrowDataList.ToArray();
    }

    private AreaData[] ExtractAreaData()
    {
        if (AreasManager == null)
        {
            return new AreaData[0];
        }

        List<GameObject> activeAreas = AreasManager.List_ActiveAreas;

        if (activeAreas == null || activeAreas.Count == 0)
        {
            return new AreaData[0];
        }

        List<AreaData> areaDataList = new List<AreaData>();

        foreach (GameObject areaObj in activeAreas)
        {
            if (areaObj != null)
            {
                Area areaComp = areaObj.GetComponent<Area>();
                if (areaComp != null)
                {
                    System.Reflection.FieldInfo cornersField = typeof(Area).GetField("List_AreaCorners", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    List<Vector3> corners = cornersField != null ? cornersField.GetValue(areaComp) as List<Vector3> : null;

                    if (corners != null && corners.Count > 0)
                    {
                        AreaData aData = new AreaData();
                        aData.colorR = areaComp.COL_Unselected_R;
                        aData.colorG = areaComp.COL_Unselected_G;
                        aData.colorB = areaComp.COL_Unselected_B;

                        List<WaypointData> pointList = new List<WaypointData>();
                        foreach (Vector3 pos in corners)
                        {
                            WaypointData pData = new WaypointData();
                            pData.position = pos;
                            pointList.Add(pData);
                        }

                        aData.points = pointList.ToArray();
                        areaDataList.Add(aData);
                    }
                }
            }
        }

        return areaDataList.ToArray();
    }

    public void LoadData(bool Temporary)
    {
        string filePath = "";

        if (Temporary)
        {
            filePath = Path.Combine(Application.persistentDataPath, "temp_mission_save.json");
        }
        else
        {
            filePath = OpenFileDialog();
        }

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogWarning("LoadData abgebrochen: Keine gültige Datei ausgewählt.");
            return;
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            SaveDataContainer dataContainer = JsonUtility.FromJson<SaveDataContainer>(jsonContent);

            if (dataContainer == null)
            {
                Debug.LogError("Fehler beim Parsen der JSON-Datei.");
                return;
            }

            if (dataContainer.MapKey != null && Map_Setup != null)
            {
                int mapIndex = GetMapIndexFromPath(dataContainer.MapKey.Path);
                Map_Setup.InitializeMap(mapIndex);
            }

            if (ObjectCreator == null)
            {
                Debug.LogError("ObjectCreator-Referenz fehlt.");
                return;
            }

            if (dataContainer.buildings != null)
            {
                foreach (UnitData unit in dataContainer.buildings)
                {
                    if (unit != null)
                    {
                        ObjectCreator.CreateObject(unit.faction, true, false, false, false, unit.type, unit.globalPosition, unit.rotation);
                    }
                }
            }

            if (dataContainer.vehicles != null)
            {
                foreach (UnitData unit in dataContainer.vehicles)
                {
                    if (unit != null)
                    {
                        ObjectCreator.CreateObject(unit.faction, false, true, false, false, unit.type, unit.globalPosition, unit.rotation);
                    }
                }
            }

            if (dataContainer.ships != null)
            {
                foreach (UnitData unit in dataContainer.ships)
                {
                    if (unit != null)
                    {
                        ObjectCreator.CreateObject(unit.faction, false, false, true, false, unit.type, unit.globalPosition, unit.rotation);
                    }
                }
            }

            if (dataContainer.aircraft != null)
            {
                foreach (UnitData unit in dataContainer.aircraft)
                {
                    if (unit != null)
                    {
                        ObjectCreator.CreateObject(unit.faction, false, false, false, true, unit.type, unit.globalPosition, unit.rotation);
                    }
                }
            }

            if (dataContainer.sorties != null)
            {
                LoadSortieData(dataContainer.sorties);
            }

            if (dataContainer.arrows != null && ArrowsManager != null)
            {
                LoadArrowsData(dataContainer.arrows);
            }

            if (dataContainer.areas != null && AreasManager != null)
            {
                LoadAreasData(dataContainer.areas);
            }

            if (dataContainer.cameraData != null && MainCamera != null && Temporary)
            {
                MainCamera.transform.position = dataContainer.cameraData.position;
                MainCamera.transform.rotation = dataContainer.cameraData.rotation;
                MainCamera.orthographicSize = dataContainer.cameraData.orthographicSize;

                if (Camera_Movement != null)
                {
                    Camera_Movement.Projection_Size = dataContainer.cameraData.ProjectionSize;
                }
            }

            isMissionLoaded = true;

            if (BTN_LoadText != null)
            {
                BTN_LoadText.text = "New";
            }

            Debug.Log($"Daten erfolgreich aus {filePath} geladen.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fehler beim Laden der Datei: {ex.Message}");
        }

        isMissionLoaded = true;
    }

    private void LoadSortieData(SortieData[] loadedSorties)
    {
        System.Reflection.FieldInfo prefabWpField = typeof(SortieManager).GetField("Prefab_Waypoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        GameObject prefabWaypoint = prefabWpField != null ? prefabWpField.GetValue(SortieManager) as GameObject : null;

        System.Reflection.FieldInfo metricField = typeof(SortieManager).GetField("Metric", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bool isMetric = metricField != null ? (bool)metricField.GetValue(SortieManager) : true;

        System.Reflection.FieldInfo listSortiesField = typeof(SortieManager).GetField("List_Sorties", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        List<Sortie> listSorties = listSortiesField != null ? listSortiesField.GetValue(SortieManager) as List<Sortie> : null;

        if (prefabWaypoint == null || listSorties == null)
        {
            Debug.LogError("Prefab_Waypoint oder List_Sorties konnte aus SortieManager nicht ausgelesen werden.");
            return;
        }

        foreach (SortieData sortieData in loadedSorties)
        {
            if (sortieData == null || sortieData.waypoints == null || sortieData.waypoints.Length == 0)
            {
                continue;
            }

            while (listSorties.Count <= sortieData.sortieIndex)
            {
                SortieManager.ChangeSortie(listSorties.Count);

                System.Reflection.MethodInfo createSortieMethod = typeof(SortieManager).GetMethod("CreateNewSortie", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (createSortieMethod != null)
                {
                    createSortieMethod.Invoke(SortieManager, null);
                }
            }

            Sortie currentSortie = listSorties[sortieData.sortieIndex];
            if (currentSortie != null)
            {
                currentSortie.SetTravelSpeed(sortieData.travelSpeed);

                foreach (WaypointData wpData in sortieData.waypoints)
                {
                    GameObject newWaypoint = Instantiate(prefabWaypoint);
                    newWaypoint.transform.position = wpData.position;
                    currentSortie.AddWaypoint(newWaypoint, isMetric);
                }

                if (SortieWing != null && sortieData.assignedAircraftIndices != null)
                {
                    SortieManager.ChangeSortie(sortieData.sortieIndex);

                    System.Reflection.FieldInfo actualSplineField = typeof(SortieManager).GetField("ActualSplineContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    UnityEngine.Splines.SplineContainer splineContainer = actualSplineField != null ? actualSplineField.GetValue(SortieManager) as UnityEngine.Splines.SplineContainer : null;

                    SortieWing.EnableWingDisplay(sortieData.sortieIndex, splineContainer, sortieData.travelSpeed);

                    foreach (int iconIndex in sortieData.assignedAircraftIndices)
                    {
                        SortieWing.AddMapIcon(iconIndex);
                    }

                    SortieWing.DisableWingDisplay();
                }
            }
        }

        SortieManager.ChangeSortie(0);
        SortieManager.ResetSortiesAtStart();
    }

    private void LoadArrowsData(ArrowData[] loadedArrows)
    {
        foreach (ArrowData aData in loadedArrows)
        {
            if (aData == null || aData.points == null || aData.points.Length == 0)
            {
                continue;
            }

            List<Vector3> pointList = new List<Vector3>();
            foreach (WaypointData pData in aData.points)
            {
                pointList.Add(pData.position);
            }

            Color arrowColor = new Color(aData.colorR, aData.colorG, aData.colorB);

            ArrowsManager.CreateLoadedArrow(pointList, aData.scaleFaktor, arrowColor);
        }
    }

    private void LoadAreasData(AreaData[] loadedAreas)
    {
        foreach (AreaData aData in loadedAreas)
        {
            if (aData == null || aData.points == null || aData.points.Length == 0)
            {
                continue;
            }

            List<Vector3> pointList = new List<Vector3>();
            foreach (WaypointData pData in aData.points)
            {
                pointList.Add(pData.position);
            }

            Color areaColor = new Color(aData.colorR, aData.colorG, aData.colorB);

            AreasManager.CreateLoadedArea(pointList, areaColor);
        }
    }

    #region Helper Methods (Map Path & File Dialogs)
    private string GetPathFromMapIndex(int index)
    {
        return $"Maps/Map_{index}";
    }

    private int GetMapIndexFromPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return 0;
        }

        string[] parts = path.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[parts.Length - 1], out int idx))
        {
            return idx;
        }
        return 0;
    }

    private string SaveFileDialog()
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "JSON Files\0*.json\0All Files\0*.*\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.persistentDataPath;
        ofn.title = "Save Mission";
        ofn.defExt = "json";
        ofn.flags = 0x00000002 | 0x00000004 | 0x00080000;

        if (WinDll.GetSaveFileName(ofn))
        {
            return ofn.file;
        }
        return null;
    }

    private string OpenFileDialog()
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "JSON Files\0*.json\0All Files\0*.*\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Application.persistentDataPath;
        ofn.title = "Load Mission";
        ofn.defExt = "json";
        ofn.flags = 0x00000008 | 0x00080000;

        if (WinDll.GetOpenFileName(ofn))
        {
            return ofn.file;
        }
        return null;
    }

    public void BTN_OpticalDetection()
    {
        SaveData(true);
        SceneManager.LoadScene("OpticalDetection");
    }

    private void OnApplicationQuit()
    {
        DeleteTempFile();
    }

    private void DeleteTempFile()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "temp_mission_save.json")))
        {
            try
            {
                File.Delete(Path.Combine(Application.persistentDataPath, "temp_mission_save.json"));
                Debug.Log("Temporäre Datei gelöscht.");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

    public void OnLoadOrNewButtonPressed()
    {
        if (!isMissionLoaded)
        {
            LoadData(false);
        }
        else
        {
            DeleteTempFile();
            RestartScene();
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}