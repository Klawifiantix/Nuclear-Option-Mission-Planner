using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_ObjectInfos : MonoBehaviour
{
    [SerializeField] TMP_Text TXT_Name;
    [SerializeField] TMP_Text TXT_Cost;
    [SerializeField] List<TMP_Text> List_TXT_WeaponNames = new List<TMP_Text>();
    [SerializeField] Image IMG_Text_Background;
    [SerializeField] Image IMG_Avatar;
    [SerializeField] Image IMG_Avatar_Background;

    public void ShowInfos(string Name, float Cost, Sprite Avatar, List<WeaponStats> List_WeaponStats)
    {
        TXT_Name.text = Name;

        string ActualCost = $"Value: ${Cost}m";

        if (Cost != 0)
        {
            TXT_Cost.text = ActualCost;
        }
        else
        {
            TXT_Cost.text = "";
        }
        
        for (int i = 0; i < List_TXT_WeaponNames.Count; i++)
        {
            List_TXT_WeaponNames[i].text = "";
        }

        if (List_WeaponStats.Count > 0)
        {
            for (int i = 0; i < List_WeaponStats.Count; i++)
            {
                List_TXT_WeaponNames[i].text = List_WeaponStats[i].name;
            }
        }

        if (Avatar != null)
        {
            IMG_Text_Background.enabled = true;
            IMG_Avatar_Background.enabled = true;
            IMG_Avatar.enabled = true;
            IMG_Avatar.sprite = Avatar;

            float Length = TXT_Name.GetPreferredValues(Name).x;

            float preferredWidth_B = TXT_Cost.GetPreferredValues(ActualCost).x;
            if (preferredWidth_B > Length)
            {
                Length = preferredWidth_B;
            }

            for (int i = 0; i < List_WeaponStats.Count; i++)
            {
                if (i < List_TXT_WeaponNames.Count)
                {
                    float weaponTextWidth = List_TXT_WeaponNames[i].GetPreferredValues(List_WeaponStats[i].name).x;
                    if (weaponTextWidth > Length)
                    {
                        Length = weaponTextWidth;
                    }
                }
            }

            float padding = 20f;

            Vector2 sizeDelta = IMG_Text_Background.rectTransform.sizeDelta;
            sizeDelta.x = Length + padding;

            float baseHeight = 105f; // Hier die Basis-Höhe eintragen
            if (List_WeaponStats.Count > 0)
            {
                sizeDelta.y = baseHeight + 10f + (List_WeaponStats.Count * 50f);
            }
            else
            {
                sizeDelta.y = baseHeight;
            }

            IMG_Text_Background.rectTransform.sizeDelta = sizeDelta;
        }
        else
        {
            IMG_Text_Background.enabled = false;
            IMG_Avatar_Background.enabled = false;
            IMG_Avatar.enabled = false;
        }
    }
}