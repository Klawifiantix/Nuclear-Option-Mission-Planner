using UnityEngine;

public class MouseHoverWaypointScale : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;

    [SerializeField] GameObject HoverObject;
    [SerializeField] GameObject HoverObject_Old;

    Waypoint Waypoint_Temp;

    private void Update()
    {
        HoverObject = null;

        Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask_Objects))
        {
            HoverObject = hit.collider.gameObject;
        }

        if (HoverObject_Old != null)
        {
            Waypoint_Temp = HoverObject_Old.GetComponent<Waypoint>();
            Waypoint_Temp.HoverTarget = false;
        }

        HoverObject_Old = HoverObject;

        if (HoverObject != null)
        {
            Waypoint_Temp = HoverObject.GetComponent<Waypoint>();
            Waypoint_Temp.HoverTarget = true;
        }
    }
}
