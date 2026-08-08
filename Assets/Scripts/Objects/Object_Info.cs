using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object_Info : MonoBehaviour
{
    [Header("Stats")]
    public UnitStats ObjectStats;

    [Header("Faction")]
    public bool BDF;
    public bool PALA;

    [Header("Faction Colors")]
    [SerializeField] Color COL_BDF;
    [SerializeField] Color COL_PALA;
    [SerializeField] Color COL_Neutral;

    [Header("Map Icon")]
    [SerializeField] Image IMG_MapIcon;

    [Header("Weapon Ranges")]
    public List<GameObject> List_WeaponRanges = new List<GameObject>();

    public bool HoverTarget;
    public bool Selected;
    GameObject Indicator;
    float Range;
    Vector3 Scale;

    Transform TRANS_WeaponRanges;

    private void Awake()
    {
        TRANS_WeaponRanges = GameObject.Find("+---WeaponRanges---+").transform;
    }

    private void OnEnable()
    {
        SetMapIconColor();
        ChangeWeaponRangeParent();
    }

    void SetMapIconColor()
    {
        IMG_MapIcon.sprite = ObjectStats.SPR_MapIcon;

        if (BDF)
        {
            IMG_MapIcon.color = COL_BDF;
            return;
        }
        
        if(PALA)
        {
            IMG_MapIcon.color = COL_PALA;
            return;
        }

        IMG_MapIcon.color = COL_Neutral;
    }
    void ChangeWeaponRangeParent()
    {
        for (int i = 0; i < List_WeaponRanges.Count; i++)
        {
            List_WeaponRanges[i].transform.SetParent(TRANS_WeaponRanges);
        }
    }

    private void Update()
    {
        if(HoverTarget || Selected)
        {
            ShowWeaponRanges();
            SelectedColor();
        }
        if(!HoverTarget && !Selected)
        {
            HideWeaponRanges();
            UnselectedColor();
        }
    }
    void ShowWeaponRanges()
    {
        for (int i = 0; i < List_WeaponRanges.Count; i++)
        {
            List_WeaponRanges[i].SetActive(false);
        }

        for (int i = 0; i < ObjectStats.List_Weapons.Count; i++)
        {
            Indicator = List_WeaponRanges[i];
            Range = ObjectStats.List_Weapons[i].Range;
            Scale = new Vector3(Range * 2, Range * 2, 1f);

            Indicator.transform.localScale = Scale;
            Indicator.SetActive(true);
        }
    }
    void HideWeaponRanges()
    {
        for (int i = 0; i < List_WeaponRanges.Count; i++)
        {
            List_WeaponRanges[i].SetActive(false);
        }
    }

    void SelectedColor()
    {
        IMG_MapIcon.color = Color.green;
    }
    void UnselectedColor()
    {
        if (BDF)
        {
            IMG_MapIcon.color = COL_BDF;
            return;
        }

        if (PALA)
        {
            IMG_MapIcon.color = COL_PALA;
        }
    }
}
