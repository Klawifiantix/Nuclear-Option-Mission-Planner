using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionBox : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;

    private Vector2 startPosition;
    private Vector2 currentPosition;
    private bool isSelecting = false;

    public List<UnitStats> List_SelectedUnitStats = new List<UnitStats>();
    public List<GameObject> List_SelectedObjects = new List<GameObject>();
    UI_GroupInfo UI_GroupInfo;
    SelectSingleObject SelectSingleObject;
    private void Awake()
    {
        UI_GroupInfo = GameObject.Find("+---Canvas_GroupSelection---+").GetComponent<UI_GroupInfo>();
        SelectSingleObject = GetComponent<SelectSingleObject>();
    }

    private void Update()
    {
        UI_GroupInfo.ShowGroupInfo(List_SelectedUnitStats);

        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isSelecting = true;
            startPosition = Mouse.current.position.ReadValue();
            currentPosition = startPosition;
        }

        if (isSelecting)
        {
            currentPosition = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isSelecting)
            {
                SelectObjectsInBox();
                isSelecting = false;
            }
        }
    }

    private void OnGUI()
    {
        if (isSelecting)
        {
            Rect rect = GetScreenRect(startPosition, currentPosition);
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            DrawScreenRectBorder(rect, 2, Color.green);
        }
    }

    private void SelectObjectsInBox()
    {
        List_SelectedUnitStats.Clear();
        List_SelectedObjects.Clear();

        Rect selectionRect = GetScreenRect(startPosition, currentPosition);
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null)
            {
                continue;
            }

            if (((1 << obj.layer) & Mask_Objects) != 0)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);

                if (screenPos.z > 0)
                {
                    screenPos.y = Screen.height - screenPos.y;

                    if (selectionRect.Contains(screenPos))
                    {
                        //Debug.Log(obj.transform.name);
                        UnitStats US_Object = obj.GetComponent<Object_Info>().ObjectStats;
                        Object_Info OI = obj.GetComponent<Object_Info>();
                        OI.Selected = true;

                        List_SelectedUnitStats.Add(US_Object);
                        List_SelectedObjects.Add(obj);
                        SelectSingleObject.List_OI.Add(OI);
                    }
                }
            }
        }
    }

    private Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;

        Vector2 topLeft = Vector2.Min(screenPosition1, screenPosition2);
        Vector2 bottomRight = Vector2.Max(screenPosition1, screenPosition2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    private void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}