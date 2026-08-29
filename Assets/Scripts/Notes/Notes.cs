using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Notes : MonoBehaviour
{
    GameObject HoverNote;
    public GameObject SelectedNote;

    [SerializeField] UI_Notes UI_Notes;

    Inputs Inputs;
    MouseHoverNotes MouseHoverNotes;

    ArrowsManager ArrowsManager;
    AreasManager AreasManager;
    LabelManager LabelManager;

    private void Awake()
    {
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        MouseHoverNotes = GameObject.Find("+---Control Center---+").GetComponent<MouseHoverNotes>();
        ArrowsManager = GetComponent<ArrowsManager>();
        AreasManager = GetComponent<AreasManager>();
        LabelManager = GetComponent<LabelManager>();
    }
    private void Update()
    {
        ToggleUiControls();

        DeselectNote();
    }

    void ToggleUiControls()
    {
        if (SelectedNote != null)
        {
            if(SelectedNote.GetComponent<Arrow>())
            {
                UI_Notes.DisplayArrowControls();
            }
            if(SelectedNote.GetComponent<Area>())
            {
                UI_Notes.DisplayAreaControls();
            }
            if (SelectedNote.GetComponent<Label>())
            {
                UI_Notes.DisplayLabelControls();
            }
        }
        else
        {
            UI_Notes.HideAll();
        }
    }

    void DeselectNote()
    {
        //Ohne Arrow Mode
        //Abwählen eines ausgewählten objekts
        if (!Inputs.Mode_Arrow_Held && !Inputs.Mode_Areas_Held && !Inputs.Mode_Labels_Held && Inputs.Select_Down)
        {
            HoverNote = MouseHoverNotes.HoverNote();

            if (HoverNote != null)//Wenn die Maus sich  über einer Note befindet. Wird die alte Note abgewählt und die neue ausgewählt.
            {
                if (SelectedNote != null)
                {
                    if(SelectedNote.GetComponent<Arrow>())
                    {
                        SelectedNote.GetComponent<Arrow>().UnselectArrow();
                    }
                    if (SelectedNote.GetComponent<Area>())
                    {
                        SelectedNote.GetComponent<Area>().UnselectArea();
                    }
                    if (SelectedNote.GetComponent<Label>())
                    {
                        SelectedNote.GetComponent<Label>().UnselectLabel();
                    }
                }

                if(HoverNote.GetComponent<Arrow>())
                {
                    SelectedNote = HoverNote;
                    SelectedNote.GetComponent<Arrow>().SelectArrow();
                    ArrowsManager.ActualArrow = ArrowsManager.List_ActiveArrows.IndexOf(SelectedNote);
                }

                if (HoverNote.GetComponent<Area>())
                {
                    SelectedNote = HoverNote;
                    SelectedNote.GetComponent<Area>().SelectArea();
                    AreasManager.ActualArea = AreasManager.List_ActiveAreas.IndexOf(SelectedNote);
                }

                if (HoverNote.GetComponent<Label>())
                {
                    SelectedNote = HoverNote;
                    SelectedNote.GetComponent<Label>().SelectLabel();
                    LabelManager.ActualLabel = LabelManager.List_ActiveLabels.IndexOf(SelectedNote);
                }
            }
            else//Wenn  nicht über einer Note gehovert wird.
            {
                //Damit der Pfeil nicht abgewählt wird, wenn das UI benutzt wird, wird geprüft ob der Mauszeiger über einem Clickable UI Element ist. Dann wird der Pfeil nimcht abgewählt.
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Mouse.current.position.ReadValue();

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)//Wenn ein UI Element gefunden wurde.
                {
                    GameObject hitObject = results[0].gameObject;
                    string tag = hitObject.transform.tag;
                    if (tag != "Clickable")//Wenn es nicht anklickbar ist, wird der Pfeil abgewählt.
                    {
                        if (SelectedNote != null)
                        {
                            if (SelectedNote.GetComponent<Arrow>())
                            {
                                SelectedNote.GetComponent<Arrow>().UnselectArrow();
                                ArrowsManager.ActualArrow = ArrowsManager.List_ActiveArrows.Count;
                            }
                            if (SelectedNote.GetComponent<Area>())
                            {
                                SelectedNote.GetComponent<Area>().UnselectArea();
                                AreasManager.ActualArea = AreasManager.List_ActiveAreas.Count;
                            }
                            if (SelectedNote.GetComponent<Label>())
                            {
                                SelectedNote.GetComponent<Label>().UnselectLabel();
                                LabelManager.ActualLabel = LabelManager.List_ActiveLabels.Count;
                            }
                        }

                        SelectedNote = null;
                    }
                }
                else//Wenn kein UI Element gefunden wirde, wird der Pfeil abgewählt.
                {
                    if (SelectedNote != null)
                    {
                        if (SelectedNote.GetComponent<Arrow>())
                        {
                            SelectedNote.GetComponent<Arrow>().UnselectArrow();
                            ArrowsManager.ActualArrow = ArrowsManager.List_ActiveArrows.Count;
                        }
                        if (SelectedNote.GetComponent<Area>())
                        {
                            SelectedNote.GetComponent<Area>().UnselectArea();
                            AreasManager.ActualArea = AreasManager.List_ActiveAreas.Count;
                        }
                        if (SelectedNote.GetComponent<Label>())
                        {
                            SelectedNote.GetComponent<Label>().UnselectLabel();
                            LabelManager.ActualLabel = LabelManager.List_ActiveLabels.Count;
                        }
                    }

                    SelectedNote = null;
                }
            }
        }
    }
}
