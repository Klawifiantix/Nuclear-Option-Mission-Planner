using UnityEngine;
using System.Collections.Generic;

public class SelectSingleObject : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;

    
    Inputs Inputs;

    public List<Object_Info> List_OI = new List<Object_Info>();

    private void Awake()
    {
        Inputs = GetComponent<Inputs>();
    }

    private void Update()
    {
        if(Inputs.Select_Down)
        {
            Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask_Objects))
            {
                List_OI.Add(hit.collider.gameObject.GetComponent<Object_Info>());
                List_OI[List_OI.Count -1].Selected = true;
            }
            else
            {
                for (int i = 0; i < List_OI.Count; i++)
                {
                    List_OI[i].Selected = false;
                }

                List_OI.Clear();
            }
        }
    }
}
