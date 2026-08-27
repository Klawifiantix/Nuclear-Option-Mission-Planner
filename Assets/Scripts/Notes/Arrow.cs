using UnityEngine;
using System.Collections.Generic;

public class Arrow : MonoBehaviour
{
    [SerializeField] LineRenderer LR;
    [SerializeField] Material MAT_ArrowHead;
    Camera_Movement Camera_Movement;

    public float Scale_Faktor;
    [SerializeField] float ArrowHead_Size;
    [SerializeField] Vector3 ArrowHeadScale;
    [SerializeField] float LR_Width;
    public List<Vector3> List_LR_Points = new List<Vector3>();

    [SerializeField] Color COL_Selected;
    [SerializeField] Color COL_Unselected;
    public float COL_Unselected_R;
    public float COL_Unselected_G;
    public float COL_Unselected_B;

    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
        MAT_ArrowHead = GetComponent<MeshRenderer>().materials[0];
        Camera_Movement = GameObject.Find("Main Camera").GetComponent<Camera_Movement>();

        Scale_Faktor = 1;
    }

    private void OnEnable()
    {
        LR.positionCount = 0;
        List_LR_Points.Clear();
        transform.rotation = Quaternion.identity;

        COL_Unselected = Color.yellow;
        COL_Unselected_R = COL_Unselected.r;
        COL_Unselected_G = COL_Unselected.g;
        COL_Unselected_B = COL_Unselected.b;
    }

    private void LateUpdate()
    {
        ArrowHead_Size = (Camera_Movement.Projection_Size / 10f) * Scale_Faktor;
        if(ArrowHead_Size > 500 * Scale_Faktor)
        {
            ArrowHead_Size = 500 * Scale_Faktor;
        }

        ArrowHeadScale = new Vector3(ArrowHead_Size, 1, ArrowHead_Size);
        transform.localScale = ArrowHeadScale;

        LR_Width = (Camera_Movement.Projection_Size / 2f) * Scale_Faktor;
        if(LR_Width > 2500 * Scale_Faktor)
        {
            LR_Width = 2500f * Scale_Faktor;
        }
        LR.startWidth = LR_Width;
        LR.endWidth = LR_Width;
    }

    public void AddPosition(Vector3 Position)
    {
        Vector3 ArrowHeadPosition = Position;
        ArrowHeadPosition.y = 5;
        transform.position = ArrowHeadPosition;

        List_LR_Points.Add(ArrowHeadPosition);

        if(List_LR_Points.Count > 1)
        {
            Vector3 LastPoint = List_LR_Points[List_LR_Points.Count - 1];
            Vector3 StartPoint = List_LR_Points[List_LR_Points.Count - 2];
            Vector3 Direction = LastPoint - StartPoint;
            transform.forward = Direction.normalized;
        }

        LR.positionCount = List_LR_Points.Count;

        Vector3[] Points = List_LR_Points.ToArray();
        LR.SetPositions(Points);

        SelectArrow();
    }

    public bool RemoveLastPosition()
    {
        if(List_LR_Points.Count > 1)
        {
            transform.position = List_LR_Points[List_LR_Points.Count - 2];
        }
        
        if(List_LR_Points.Count > 2)
        {
            Vector3 LastPoint = List_LR_Points[List_LR_Points.Count - 2];
            Vector3 StartPoint = List_LR_Points[List_LR_Points.Count - 3];
            Vector3 Direction = LastPoint - StartPoint;
            transform.forward = Direction.normalized;
        }

        List_LR_Points.RemoveAt(List_LR_Points.Count - 1);

        LR.positionCount = List_LR_Points.Count;

        Vector3[] Points = List_LR_Points.ToArray();
        LR.SetPositions(Points);

        SelectArrow();

        if (List_LR_Points.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SelectArrow()
    {
        COL_Selected.a = 1;

        MAT_ArrowHead.SetColor("_Color", COL_Selected);

        COL_Selected.a = MAT_ArrowHead.GetFloat("_Alpha");
        LR.startColor = COL_Selected;
        LR.endColor = COL_Selected;
    }

    public void UnselectArrow()
    {
        COL_Unselected.a = 1;

        MAT_ArrowHead.SetColor("_Color", COL_Unselected);

        COL_Unselected.a = MAT_ArrowHead.GetFloat("_Alpha");
        LR.startColor = COL_Unselected;
        LR.endColor = COL_Unselected;
    }

    public void SetUnselectedColor(Color NewColor)
    {
        COL_Unselected_R = NewColor.r;
        COL_Unselected_G = NewColor.g;
        COL_Unselected_B = NewColor.b;

        COL_Unselected = NewColor;
        UnselectArrow();
    }

    public void SetScaleFaktor (float NewFaktor)
    {
        Scale_Faktor = NewFaktor;
    }
}
