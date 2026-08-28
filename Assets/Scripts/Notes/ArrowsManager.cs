using UnityEngine;
using System.Collections.Generic;

public class ArrowsManager : MonoBehaviour
{
    Inputs Inputs;
    MouseWorldPosition MouseWorldPosition;
    Transform TransArrows;

    [SerializeField] GameObject Prefab_Arrow;
    public List<GameObject> List_ActiveArrows = new List<GameObject>();
    [SerializeField] List<GameObject> List_InactiveArrows = new List<GameObject>();

    [SerializeField] GameObject ArrowToDelete;
    public int ActualArrow;
    //public GameObject SelectedArrow;
    Notes Notes;
    Arrow Arrow;

    private void Awake()
    {
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        MouseWorldPosition = GameObject.Find("+---Control Center---+").GetComponent<MouseWorldPosition>();
        TransArrows = GameObject.Find("+---Arrows---+").transform;
        Notes = GameObject.Find("+---Notes---+").GetComponent<Notes>();
    }

    private void Update()
    {
        AddArrowPoint();

        RemoveArrowPoint();

        DeleteArrow();
    }

    public GameObject CreateLoadedArrow(List<Vector3> points, float scaleFaktor, Color color)
    {
        GameObject newArrow = null;

        if (List_InactiveArrows.Count == 0)
        {
            newArrow = GameObject.Instantiate(Prefab_Arrow, TransArrows);
            List_ActiveArrows.Add(newArrow);
        }
        else
        {
            newArrow = List_InactiveArrows[0];
            List_InactiveArrows.Remove(newArrow);
            List_ActiveArrows.Add(newArrow);
            newArrow.SetActive(true);
        }

        Arrow arrowComp = newArrow.GetComponent<Arrow>();
        arrowComp.SetScaleFaktor(scaleFaktor);
        arrowComp.SetUnselectedColor(color);

        // Punkte setzen
        foreach (var pos in points)
        {
            arrowComp.AddPosition(pos);
        }

        arrowComp.UnselectArrow();

        return newArrow;
    }    

    void AddArrowPoint()
    {
        //Arrow Hinzufügen
        if (Inputs.Mode_Arrow_Held && Inputs.Select_Down)
        {
            Vector3 Spawnposition = MouseWorldPosition.MousePosition;
            Spawnposition.y = 5f;

            if (List_ActiveArrows.Count - 1 < ActualArrow)
            {
                GameObject NewArrow = null;

                if (List_InactiveArrows.Count == 0)
                {
                    NewArrow = GameObject.Instantiate(Prefab_Arrow);
                    NewArrow.transform.SetParent(TransArrows);
                    List_ActiveArrows.Add(NewArrow);
                }
                else
                {
                    NewArrow = List_InactiveArrows[0];
                    if (List_InactiveArrows.Contains(NewArrow))
                    {
                        List_InactiveArrows.Remove(NewArrow);
                    }

                    if (!List_ActiveArrows.Contains(NewArrow))
                    {
                        List_ActiveArrows.Add(NewArrow);
                    }

                    NewArrow.SetActive(true);
                }
                NewArrow.transform.position = Spawnposition;
                Notes.SelectedNote = NewArrow;
            }

            if (List_ActiveArrows.Count >= ActualArrow + 1)
            {
                Arrow = List_ActiveArrows[ActualArrow].GetComponent<Arrow>();
                Arrow.AddPosition(Spawnposition);
                Notes.SelectedNote = List_ActiveArrows[ActualArrow];
            }
        }
    }

    void RemoveArrowPoint()
    {
        //Arrowpunkt entfernen
        if (Inputs.Mode_Arrow_Held && Inputs.Deselect_Down)
        {
            if (List_ActiveArrows.Count >= ActualArrow + 1)
            {
                Arrow = List_ActiveArrows[ActualArrow].GetComponent<Arrow>();
                if (Arrow.RemoveLastPosition())//Der letzte Punkt wurde entfernt
                {
                    DeleteArrow_Final();
                }
            }
        }
    }

    void DeleteArrow()
    {
        if (Notes.SelectedNote != null && Inputs.Delete_Down)
        {
            if(Notes.SelectedNote.GetComponent<Arrow>())
            {
                DeleteArrow_Final();
            }
        }
    }

    void DeleteArrow_Final()
    {
        ArrowToDelete = List_ActiveArrows[ActualArrow];

        if (List_ActiveArrows.Contains(ArrowToDelete))
        {
            List_ActiveArrows.Remove(ArrowToDelete);
        }

        ArrowToDelete.SetActive(false);

        if (!List_InactiveArrows.Contains(ArrowToDelete))
        {
            List_InactiveArrows.Add(ArrowToDelete);
        }

        ActualArrow--;

        if (ActualArrow < 0)
        {
            ActualArrow = 0;
            Notes.SelectedNote = null;
        }
        else
        {
            if (List_ActiveArrows.Count - 1 == ActualArrow)
            {
                Notes.SelectedNote = List_ActiveArrows[ActualArrow];
                Notes.SelectedNote.GetComponent<Arrow>().SelectArrow();
            }
            else
            {
                Notes.SelectedNote = null;
            }
        }
    }
}
