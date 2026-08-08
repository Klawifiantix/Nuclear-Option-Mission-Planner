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
    [SerializeField] private InputActionReference LShift;

    [Header("Clicks")]
    [SerializeField] private InputActionReference Select;
    [SerializeField] private InputActionReference Deselect;

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
    public bool LShift_Held { get; private set; }
    public bool LShift_Down { get; private set; }
    public bool LShift_Up { get; private set; }

    //Clicks
    public bool Select_Held { get; private set; }
    public bool Select_Down { get; private set; }
    public bool Select_Up { get; private set; }

    public bool Deselect_Held { get; private set; }
    public bool Deselect_Down { get; private set; }
    public bool Deselect_Up { get; private set; }

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
        if(LShift != null)
        {
            LShift.action.Enable();
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
        if (LShift != null)
        {
            LShift.action.Disable();
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
        if (LShift != null)
        {
            LShift_Held = LShift.action.IsPressed();
            LShift_Down = LShift.action.WasPressedThisFrame();
            LShift_Up = LShift.action.WasReleasedThisFrame();
        }
        else
        {
            LShift_Held = LShift_Down = LShift_Up = false;
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
    }
}
