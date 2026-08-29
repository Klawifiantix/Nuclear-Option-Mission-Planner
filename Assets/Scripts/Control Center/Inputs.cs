using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    [Header("CameraMovement")]
    [SerializeField] private InputActionReference pan;
    [SerializeField] private InputActionReference pointerDelta;
    [SerializeField] private InputActionReference zoom;

    [Header("Map")]
    [SerializeField] private InputActionReference ChangeStyle;

    [Header("Waypoints")]
    [SerializeField] private InputActionReference Mode_Waypoint;

    [Header("Clicks")]
    [SerializeField] private InputActionReference Select;
    [SerializeField] private InputActionReference Deselect;

    [Header("Arrows")]
    [SerializeField] private InputActionReference Mode_Arrow;

    [Header("Areas")]
    [SerializeField] private InputActionReference Mode_Areas;
    
    [Header("Labels")]
    [SerializeField] private InputActionReference Mode_Labels;

    [Header("Delete")]
    [SerializeField] private InputActionReference Delete;

    //CameraMovement
    public bool Pan_Held { get; private set; }
    public bool Pan_Down { get; private set; }
    public bool Pan_Up { get; private set; }

    public Vector2 MouseDelta { get; private set; }

    public float ZoomDelta { get; private set; }

    //Map
    public bool ChangeStyle_Held { get; private set; }
    public bool ChangeStyle_Down { get; private set; }
    public bool ChangeStyle_Up { get; private set; }

    //Waypoints
    public bool Mode_Waypoint_Held { get; private set; }
    public bool Mode_Waypoint_Down { get; private set; }
    public bool Mode_Waypoint_Up { get; private set; }

    //Clicks
    public bool Select_Held { get; private set; }
    public bool Select_Down { get; private set; }
    public bool Select_Up { get; private set; }

    public bool Deselect_Held { get; private set; }
    public bool Deselect_Down { get; private set; }
    public bool Deselect_Up { get; private set; }

    //Arrows
    public bool Mode_Arrow_Held { get; private set; }
    public bool Mode_Arrow_Down { get; private set; }
    public bool Mode_Arrow_Up { get; private set; }

    //Areas
    public bool Mode_Areas_Held { get; private set; }
    public bool Mode_Areas_Down { get; private set; }
    public bool Mode_Areas_Up { get; private set; }

    //Labels
    public bool Mode_Labels_Held { get; private set; }
    public bool Mode_Labels_Down { get; private set; }
    public bool Mode_Labels_Up { get; private set; }

    //Delete
    public bool Delete_Held { get; private set; }
    public bool Delete_Down { get; private set; }
    public bool Delete_Up { get; private set; }

    private void Awake()
    {
        if (
            PlayerPrefs.HasKey("InputOverrides")
        )
        {
            string overrides = PlayerPrefs.GetString("InputOverrides");
            if (
                pan != null
            )
            {
                pan.asset.LoadBindingOverridesFromJson(overrides);
            }
        }
    }

    private void OnEnable()
    {
        //Camera Movement
        if (pan != null)
        {
            pan.action.Enable();
        }
        if (pointerDelta != null)
        {
            pointerDelta.action.Enable();
        }
        if (zoom != null)
        {
            zoom.action.Enable();
        }

        //Map
        if(ChangeStyle != null)
        {
            ChangeStyle.action.Enable();
        }

        //Waypoints
        if(Mode_Waypoint != null)
        {
            Mode_Waypoint.action.Enable();
        }

        //Clicks
        if(Select != null)
        {
            Select.action.Enable();
        }

        if(Deselect != null)
        {
            Deselect.action.Enable();
        }

        //Arrows
        if(Mode_Arrow !=null)
        {
            Mode_Arrow.action.Enable();
        }

        //Areas
        if (Mode_Areas != null)
        {
            Mode_Areas.action.Enable();
        }

        //Labels
        if (Mode_Labels != null)
        {
            Mode_Labels.action.Enable();
        }

        //Delete
        if (Delete != null)
        {
            Delete.action.Enable();
        }
    }

    private void OnDisable()
    {
        //Camera Movement
        if (pan != null)
        {
            pan.action.Disable();
        }
        if (pointerDelta != null)
        {
            pointerDelta.action.Disable();
        }
        if (zoom != null)
        {
            zoom.action.Disable();
        }

        //Map
        if (ChangeStyle != null)
        {
            ChangeStyle.action.Disable();
        }

        //Waypoints
        if (Mode_Waypoint != null)
        {
            Mode_Waypoint.action.Disable();
        }

        //Clicks
        if (Select != null)
        {
            Select.action.Disable();
        }

        if (Deselect != null)
        {
            Deselect.action.Disable();
        }

        //Arrows
        if (Mode_Arrow != null)
        {
            Mode_Arrow.action.Disable();
        }

        //Areas
        if (Mode_Areas != null)
        {
            Mode_Areas.action.Disable();
        }

        //Labels
        if (Mode_Labels != null)
        {
            Mode_Labels.action.Disable();
        }

        //Delete
        if (Delete != null)
        {
            Delete.action.Disable();
        }
    }

    private void Update()
    {
        // Camera Movement
        if (pan != null)
        {
            Pan_Held = pan.action.IsPressed();
            Pan_Down = pan.action.WasPressedThisFrame();
            Pan_Up = pan.action.WasReleasedThisFrame();
        }
        else
        {
            Pan_Held = Pan_Down = Pan_Up = false;
        }

        MouseDelta = pointerDelta != null ? pointerDelta.action.ReadValue<Vector2>() : Vector2.zero;

        ZoomDelta = zoom != null ? zoom.action.ReadValue<float>() : 0f;

        //Map
        if (ChangeStyle != null)
        {
            ChangeStyle_Held = ChangeStyle.action.IsPressed();
            ChangeStyle_Down = ChangeStyle.action.WasPressedThisFrame();
            ChangeStyle_Up = ChangeStyle.action.WasReleasedThisFrame();
        }
        else
        {
            ChangeStyle_Held = ChangeStyle_Down = ChangeStyle_Up = false;
        }

        //Waypoint
        if (Mode_Waypoint != null)
        {
            Mode_Waypoint_Held = Mode_Waypoint.action.IsPressed();
            Mode_Waypoint_Down = Mode_Waypoint.action.WasPressedThisFrame();
            Mode_Waypoint_Up = Mode_Waypoint.action.WasReleasedThisFrame();
        }
        else
        {
            Mode_Waypoint_Held = Mode_Waypoint_Down = Mode_Waypoint_Up = false;
        }

        //Clicks
        if (Select != null)
        {
            Select_Held = Select.action.IsPressed();
            Select_Down = Select.action.WasPressedThisFrame();
            Select_Up = Select.action.WasReleasedThisFrame();
        }
        else
        {
            Select_Held = Select_Down = Select_Up = false;
        }

        if (Deselect != null)
        {
            Deselect_Held = Deselect.action.IsPressed();
            Deselect_Down = Deselect.action.WasPressedThisFrame();
            Deselect_Up = Deselect.action.WasReleasedThisFrame();
        }
        else
        {
            Deselect_Held = Deselect_Down = Deselect_Up = false;
        }

        //Arrows
        if (Mode_Arrow != null)
        {
            Mode_Arrow_Held = Mode_Arrow.action.IsPressed();
            Mode_Arrow_Down = Mode_Arrow.action.WasPressedThisFrame();
            Mode_Arrow_Up = Mode_Arrow.action.WasReleasedThisFrame();
        }
        else
        {
            Mode_Arrow_Held = Mode_Arrow_Down = Mode_Arrow_Up = false;
        }

        //Areas
        if (Mode_Areas != null)
        {
            Mode_Areas_Held = Mode_Areas.action.IsPressed();
            Mode_Areas_Down = Mode_Areas.action.WasPressedThisFrame();
            Mode_Areas_Up = Mode_Areas.action.WasReleasedThisFrame();
        }
        else
        {
            Mode_Areas_Held = Mode_Areas_Down = Mode_Areas_Up = false;
        }

        //Labels
        if (Mode_Labels != null)
        {
            Mode_Labels_Held = Mode_Labels.action.IsPressed();
            Mode_Labels_Down = Mode_Labels.action.WasPressedThisFrame();
            Mode_Labels_Up = Mode_Labels.action.WasReleasedThisFrame();
        }
        else
        {
            Mode_Labels_Held = Mode_Labels_Down = Mode_Labels_Up = false;
        }

        //Delete
        if (Delete != null)
        {
            Delete_Held = Delete.action.IsPressed();
            Delete_Down = Delete.action.WasPressedThisFrame();
            Delete_Up = Delete.action.WasReleasedThisFrame();
        }
        else
        {
            Delete_Held = Delete_Down = Delete_Up = false;
        }
    }
}
