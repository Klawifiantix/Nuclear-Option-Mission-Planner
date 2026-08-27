using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ArrowsManager : MonoBehaviour
{
    Inputs Inputs;
    MouseWorldPosition MouseWorldPosition;
    MouseHoverNotes MouseHoverNotes;
    Transform TransArrows;
    [SerializeField] GameObject OBJ_InfoPanel;

    [SerializeField] GameObject Prefab_Arrow;
    [SerializeField] List<GameObject> List_ActiveArrows = new List<GameObject>();
    [SerializeField] List<GameObject> List_InactiveArrows = new List<GameObject>();

    [SerializeField] int ActualArrow;
    public GameObject SelectedArrow;

    Arrow Arrow;
    private void Awake()
    {
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        MouseWorldPosition = GameObject.Find("+---Control Center---+").GetComponent<MouseWorldPosition>();
        MouseHoverNotes = GameObject.Find("+---Control Center---+").GetComponent<MouseHoverNotes>();
        TransArrows = GameObject.Find("+---Arrows---+").transform;
    }

    private void Update()
    {
        //Ohne Arrow Mode
        if(!Inputs.Mode_Arrow_Held && Inputs.Select_Down)
        {
            if(MouseHoverNotes.HoverNote() != null)
            {
                if (SelectedArrow != null)
                {
                    SelectedArrow.GetComponent<Arrow>().UnselectArrow();
                }

                SelectedArrow = MouseHoverNotes.HoverNote();
                SelectedArrow.GetComponent<Arrow>().SelectArrow();
                ActualArrow = List_ActiveArrows.IndexOf(SelectedArrow);
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Es wurde auf ein UI-Element geklickt.");
                }
                else
                {
                    if (SelectedArrow != null)
                    {
                        SelectedArrow.GetComponent<Arrow>().UnselectArrow();
                    }

                    SelectedArrow = null;
                    ActualArrow = List_ActiveArrows.Count; ;
                }
            }
        }

        //Arrow Hinzufügen
        if (Inputs.Mode_Arrow_Held && Inputs.Select_Down)
        {
            Vector3 Spawnposition = MouseWorldPosition.MousePosition;
            Spawnposition.y = 5f;

            if (List_ActiveArrows.Count -1 < ActualArrow)
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
                SelectedArrow = NewArrow;
            }

            if(List_ActiveArrows.Count >= ActualArrow + 1)
            {
                Arrow = List_ActiveArrows[ActualArrow].GetComponent<Arrow>();
                Arrow.AddPosition(Spawnposition);
                SelectedArrow = List_ActiveArrows[ActualArrow];
            }
        }

        //Arrowpunkt entfernen
        if(Inputs.Mode_Arrow_Held && Inputs.Deselect_Down)
        {
            if(List_ActiveArrows.Count >= ActualArrow + 1)
            {
                Arrow = List_ActiveArrows[ActualArrow].GetComponent<Arrow>();
                if(Arrow.RemoveLastPosition())//Der letzte Punkt wurde entfernt
                {
                    DeleteArrow();
                }
            }
        }

        if(SelectedArrow != null && Inputs.Delete_Down)
        {
            DeleteArrow();
        }

        if(SelectedArrow != null)
        {
            OBJ_InfoPanel.SetActive(true);
        }
        else
        {
            OBJ_InfoPanel.SetActive(false);
        }
    }

    void DeleteArrow()
    {
        GameObject ArrowToDelete = List_ActiveArrows[ActualArrow];

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
            SelectedArrow = null;
        }
        else
        {
            if (List_ActiveArrows.Count - 1 == ActualArrow)
            {
                SelectedArrow = List_ActiveArrows[ActualArrow];
                SelectedArrow.GetComponent<Arrow>().SelectArrow();
            }
            else
            {
                SelectedArrow = null;
            }
        }
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
}
