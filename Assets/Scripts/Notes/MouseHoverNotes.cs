using UnityEngine;

public class MouseHoverNotes : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;

    public GameObject HoverNote()
    {
        Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask_Objects))
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
