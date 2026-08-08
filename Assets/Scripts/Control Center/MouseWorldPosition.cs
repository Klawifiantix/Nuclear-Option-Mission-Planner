using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;
    public Vector3 MousePosition;

    private void Update()
    {
        Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask_Objects))
        {
            MousePosition = hit.point;
        }
    }
}
