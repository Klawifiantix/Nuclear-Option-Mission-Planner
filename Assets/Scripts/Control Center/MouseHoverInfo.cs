using UnityEngine;

public class MouseHoverInfo : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;

    UI_ObjectInfos UI_ObjectInfos;
    GameObject HoverObject;

    Object_Info OI_Temp;

    private void Awake()
    {
        UI_ObjectInfos = GameObject.Find("+---Canvas_HoverObjectInfos---+").GetComponent<UI_ObjectInfos>();
    }

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

        if(HoverObject != null)
        {
            OI_Temp = HoverObject.GetComponent<Object_Info>();

            string Name = OI_Temp.ObjectStats.STR_Name_InGame;
            float Cost = OI_Temp.ObjectStats.Cost;
            Sprite Avatar = OI_Temp.ObjectStats.SPR_Avatar;

            UI_ObjectInfos.ShowInfos(Name, Cost, Avatar);
        }
        else
        {
            Sprite Avatar = null;
            UI_ObjectInfos.ShowInfos("", 0, Avatar);
        }
    }
}