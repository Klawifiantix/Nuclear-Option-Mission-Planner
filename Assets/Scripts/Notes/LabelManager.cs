using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LabelManager : MonoBehaviour
{
    [SerializeField] GameObject Prefab_Label;
    public List<GameObject> List_ActiveLabels = new List<GameObject>();
    [SerializeField] List<GameObject> List_InactiveLabels = new List<GameObject>();

    [SerializeField] GameObject LabelToMove;
    [SerializeField] GameObject LabelToDelete;

    public int ActualLabel;

    MouseHoverNotes MouseHoverNotes;
    MouseWorldPosition MouseWorldPosition;

    Transform TransLabels;

    Inputs Inputs;
    Notes Notes;
    Label Label;
    private void Awake()
    {
        MouseHoverNotes = GameObject.Find("+---Control Center---+").GetComponent<MouseHoverNotes>();
        MouseWorldPosition = GameObject.Find("+---Control Center---+").GetComponent<MouseWorldPosition>();
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        Notes = GameObject.Find("+---Notes---+").GetComponent<Notes>();
        TransLabels = GameObject.Find("+---Labels---+").transform;
    }

    private void Update()
    {
        MoveLabel();

        AddLabel();
        DeleteLabel();
    }

    void MoveLabel()
    {
        if (Inputs.Select_Down)
        {
            GameObject HoverObject = MouseHoverNotes.HoverNote();
            if (HoverObject != null)
            {
                if (HoverObject.GetComponent<Label>())
                {
                    LabelToMove = HoverObject;
                    Notes.SelectedNote = HoverObject;
                }
            }
        }

        if (LabelToMove != null && Inputs.Select_Held)
        {
            Vector3 Position = MouseWorldPosition.MousePosition;
            Position.y = 200f;

            LabelToMove.transform.position = Position;
        }

        if (LabelToMove != null && Inputs.Select_Up)
        {
            LabelToMove = null;
        }
    }

    void AddLabel()
    {
        //Arrow Hinzufügen
        if (Inputs.Mode_Labels_Held && Inputs.Select_Down)
        {
            Vector3 Spawnposition = MouseWorldPosition.MousePosition;
            Spawnposition.y = 200f;

            if (List_ActiveLabels.Count - 1 < ActualLabel)
            {
                GameObject NewArea = null;

                if (List_InactiveLabels.Count == 0)
                {
                    NewArea = GameObject.Instantiate(Prefab_Label);
                    NewArea.transform.SetParent(TransLabels);
                    List_ActiveLabels.Add(NewArea);
                }
                else
                {
                    NewArea = List_InactiveLabels[0];
                    if (List_InactiveLabels.Contains(NewArea))
                    {
                        List_InactiveLabels.Remove(NewArea);
                    }

                    if (!List_ActiveLabels.Contains(NewArea))
                    {
                        List_ActiveLabels.Add(NewArea);
                    }

                    NewArea.SetActive(true);
                }
                NewArea.transform.position = Spawnposition;
                Notes.SelectedNote = NewArea;
            }

            if (List_ActiveLabels.Count >= ActualLabel + 1)
            {
                Label = List_ActiveLabels[ActualLabel].GetComponent<Label>();
                Notes.SelectedNote = List_ActiveLabels[ActualLabel];
            }
        }
    }

    void DeleteLabel()
    {
        if (Notes.SelectedNote != null && Inputs.Delete_Down)
        {
            if (Notes.SelectedNote.GetComponent<Label>())
            {
                DeleteLabel_Final();
            }
        }
    }

    void DeleteLabel_Final()
    {
        LabelToDelete = List_ActiveLabels[ActualLabel];

        if (List_ActiveLabels.Contains(LabelToDelete))
        {
            List_ActiveLabels.Remove(LabelToDelete);
        }

        LabelToDelete.SetActive(false);

        if (!List_InactiveLabels.Contains(LabelToDelete))
        {
            List_InactiveLabels.Add(LabelToDelete);
        }

        ActualLabel--;

        if (ActualLabel < 0)
        {
            ActualLabel = 0;
            Notes.SelectedNote = null;
        }
        else
        {
            if (List_ActiveLabels.Count - 1 == ActualLabel)
            {
                Notes.SelectedNote = List_ActiveLabels[ActualLabel];
                Notes.SelectedNote.GetComponent<Label>().SelectLabel();
            }
            else
            {
                Notes.SelectedNote = null;
            }
        }
    }

    public void CreateLoadedLabel(Vector3 position, float scaleFaktor, Color color, string textContent)
    {
        GameObject newLabel = Instantiate(Prefab_Label, position, Quaternion.identity);
        newLabel.transform.Rotate(0f, 180f, 0f);
        newLabel.transform.SetParent(TransLabels);

        Label labelComp = newLabel.GetComponent<Label>();

        if (labelComp != null)
        {
            labelComp.SetScale(scaleFaktor);
            labelComp.SetUnselectedColor(color);
            labelComp.SetText(textContent);
        }

        List_ActiveLabels.Add(newLabel);
    }
}
