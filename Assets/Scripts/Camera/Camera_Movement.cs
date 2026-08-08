using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    [SerializeField] float Projection_Size;
    [SerializeField] float MoveSpeed;
    [SerializeField] float ZoomStep;

    [SerializeField] float Map_Max_X;
    [SerializeField] float Map_Max_Y;

    [SerializeField] float Min_Projection_Size;
    [SerializeField] float Max_Projection_Size;    

    GameObject OBJ_Map;
    GameObject OBJ_Camera;
    Inputs Inputs;
    private void Awake()
    {
        OBJ_Camera = gameObject;
        Projection_Size = OBJ_Camera.GetComponent<Camera>().orthographicSize;
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        OBJ_Map = GameObject.Find("+---MAP---+");
    }

    private void Start()
    {
        Map_Max_X = OBJ_Map.transform.localScale.x;
        Map_Max_Y = OBJ_Map.transform.localScale.y;

        Min_Projection_Size = 500f;
        if(Map_Max_X >= Map_Max_Y)
        {
            Max_Projection_Size = Map_Max_X / 2f;
        }
        else
        {
            Max_Projection_Size = Map_Max_Y / 2f;
        }
    }

    private void Update()
    {
        Pan();
        Zoom();
    }

    void Pan()
    {
        if (Inputs.Pan_Held)
        {
            Vector3 Destination = OBJ_Camera.transform.position;

            MoveSpeed = Projection_Size / 200f;
            Destination -= Inputs.MouseDelta.x * (OBJ_Camera.transform.right * MoveSpeed);
            Destination -= Inputs.MouseDelta.y * (OBJ_Camera.transform.up * MoveSpeed);

            OBJ_Camera.transform.position = Destination;
        }
    }

    void Zoom()
    {
        float ScrollInput = Inputs.ZoomDelta;       

        if(ScrollInput != 0)
        {
            if (ScrollInput > 0)
            {
                if (Projection_Size > Min_Projection_Size)
                {
                    Projection_Size -= ZoomStep;
                }
            }

            if (ScrollInput < 0)
            {
                if (Projection_Size < Max_Projection_Size)
                {
                    Projection_Size += ZoomStep;
                }
            }

            if(Projection_Size < Min_Projection_Size)
            {
                Projection_Size = Min_Projection_Size;
            }

            if(Projection_Size > Max_Projection_Size)
            {
                Projection_Size = Max_Projection_Size;
            }

            OBJ_Camera.GetComponent<Camera>().orthographicSize = Projection_Size;
        }
    }
}
