using System.Collections.Generic;
using UnityEngine;

public class AreasManager : MonoBehaviour
{
    [SerializeField] GameObject Prefab_Area;
    public List<GameObject> List_ActiveAreas = new List<GameObject>();
    [SerializeField] List<GameObject> List_InactiveAreas = new List<GameObject>();

    [SerializeField] GameObject AreaToDelete;

    public int ActualArea;

    Inputs Inputs;
    MouseWorldPosition MouseWorldPosition;
    Transform TransArea;
    Area Area;
    Notes Notes;
    private void Awake()
    {
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        MouseWorldPosition = GameObject.Find("+---Control Center---+").GetComponent<MouseWorldPosition>();
        TransArea = GameObject.Find("+---Areas---+").transform;
        Notes = GameObject.Find("+---Notes---+").GetComponent<Notes>();
    }

    private void Update()
    {
        AddAreaPoint();

        RemoveAreaPoint();

        DeleteArrow();
    }

    void AddAreaPoint()
    {
        //Arrow Hinzufügen
        if (Inputs.Mode_Areas_Held && Inputs.Select_Down)
        {
            Vector3 Spawnposition = MouseWorldPosition.MousePosition;
            Spawnposition.y = 4f;

            if (List_ActiveAreas.Count - 1 < ActualArea)
            {
                GameObject NewArea = null;

                if (List_InactiveAreas.Count == 0)
                {
                    NewArea = GameObject.Instantiate(Prefab_Area);
                    NewArea.transform.SetParent(TransArea);
                    List_ActiveAreas.Add(NewArea);
                }
                else
                {
                    NewArea = List_InactiveAreas[0];
                    if (List_InactiveAreas.Contains(NewArea))
                    {
                        List_InactiveAreas.Remove(NewArea);
                    }

                    if (!List_ActiveAreas.Contains(NewArea))
                    {
                        List_ActiveAreas.Add(NewArea);
                    }

                    NewArea.SetActive(true);
                }
                NewArea.transform.position = Spawnposition;
                Notes.SelectedNote = NewArea;
            }

            if (List_ActiveAreas.Count >= ActualArea + 1)
            {
                Area = List_ActiveAreas[ActualArea].GetComponent<Area>();
                Area.AddCorner(Spawnposition);
                Notes.SelectedNote = List_ActiveAreas[ActualArea];
            }
        }
    }

    void RemoveAreaPoint()
    {
        //Arrowpunkt entfernen
        if (Inputs.Mode_Areas_Held && Inputs.Deselect_Down)
        {
            if (List_ActiveAreas.Count >= ActualArea + 1)
            {
                Area = List_ActiveAreas[ActualArea].GetComponent<Area>();
                if (Area.RemoveLastCorner())//Der letzte Punkt wurde entfernt
                {
                    DeleteArea_Final();
                }
            }
        }
    }

    void DeleteArrow()
    {
        if (Notes.SelectedNote != null && Inputs.Delete_Down)
        {
            if (Notes.SelectedNote.GetComponent<Area>())
            {
                DeleteArea_Final();
            }
        }
    }

    void DeleteArea_Final()
    {
        AreaToDelete = List_ActiveAreas[ActualArea];

        if (List_ActiveAreas.Contains(AreaToDelete))
        {
            List_ActiveAreas.Remove(AreaToDelete);
        }

        AreaToDelete.SetActive(false);

        if (!List_InactiveAreas.Contains(AreaToDelete))
        {
            List_InactiveAreas.Add(AreaToDelete);
        }

        ActualArea--;

        if (ActualArea < 0)
        {
            ActualArea = 0;
            Notes.SelectedNote = null;
        }
        else
        {
            if (List_ActiveAreas.Count - 1 == ActualArea)
            {
                Notes.SelectedNote = List_ActiveAreas[ActualArea];
                Notes.SelectedNote.GetComponent<Area>().SelectArea();
            }
            else
            {
                Notes.SelectedNote = null;
            }
        }
    }

    public GameObject CreateLoadedArea(List<Vector3> points, Color color)
    {
        GameObject newArea = null;

        if (List_InactiveAreas.Count == 0)
        {
            newArea = GameObject.Instantiate(Prefab_Area, TransArea);
            List_ActiveAreas.Add(newArea);
        }
        else
        {
            newArea = List_InactiveAreas[0];
            List_InactiveAreas.Remove(newArea);
            List_ActiveAreas.Add(newArea);
            newArea.SetActive(true);
        }

        Area areaComp = newArea.GetComponent<Area>();
        areaComp.SetUnselectedColor(color);

        foreach (var pos in points)
        {
            areaComp.AddCorner(pos);
        }

        areaComp.UnselectArea();

        return newArea;
    }
}
