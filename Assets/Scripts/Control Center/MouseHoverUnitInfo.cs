using System.Collections.Generic;
using UnityEngine;

public class MouseHoverUnitInfo : MonoBehaviour
{
    [SerializeField] LayerMask Mask_Objects;

    UI_ObjectInfos UI_ObjectInfos;
    GameObject HoverObject;
    GameObject HoverObject_Old;
    List<WeaponStats> List_WeaponStats = new List<WeaponStats>();

    Object_Info OI_Temp;
    Object_Info OI_Temp_Old;
    private void Awake()
    {
        UI_ObjectInfos = GameObject.Find("+---Canvas_HoverObjectInfos---+").GetComponent<UI_ObjectInfos>();
    }

    private void Update()
    {
        HoverCheck();
    }

    void HoverCheck()
    {
        HoverObject = null;
        if (HoverObject_Old != null)
        {
            OI_Temp_Old = HoverObject_Old.GetComponent<Object_Info>();
            OI_Temp_Old.HoverTarget = false;
        }

        Vector2 mousePosition = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Mask_Objects))
        {
            HoverObject = hit.collider.gameObject;
        }

        if (HoverObject != null)
        {
            HoverObject_Old = HoverObject;

            OI_Temp = HoverObject.GetComponent<Object_Info>();
            OI_Temp.HoverTarget = true;

            string Name = OI_Temp.ObjectStats.STR_Name_InGame;
            float Cost = OI_Temp.ObjectStats.Cost;
            Sprite Avatar = OI_Temp.ObjectStats.SPR_Avatar;

            List_WeaponStats.Clear();
            if (OI_Temp.ObjectStats.List_Weapons.Count > 0)
            {
                for (int i = 0; i < OI_Temp.ObjectStats.List_Weapons.Count; i++)
                {
                    List_WeaponStats.Add(OI_Temp.ObjectStats.List_Weapons[i]);
                }
            }

            UI_ObjectInfos.ShowInfos(Name, Cost, Avatar, List_WeaponStats);
        }
        else
        {
            List_WeaponStats.Clear();
            Sprite Avatar = null;
            UI_ObjectInfos.ShowInfos("", 0, Avatar, List_WeaponStats);
        }
    }

}