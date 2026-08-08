using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GroupInfo : MonoBehaviour
{
    [SerializeField] TMP_Text TXT_AmountSelected;
    [SerializeField] TMP_Text TXT_CostTotal;
    [SerializeField] Image IMG_Background;

    [SerializeField] List<UnitStats> List_SelectedObjects_Here = new List<UnitStats>();

    public void ShowGroupInfo(List<UnitStats> List_SelectedObjects)
    {
        if (List_SelectedObjects_Here != List_SelectedObjects)
        {
            if (List_SelectedObjects.Count > 1)
            {
                List_SelectedObjects_Here.Clear();
                List_SelectedObjects_Here.AddRange(List_SelectedObjects);
                IMG_Background.enabled = true;
            }
            else
            {
                List_SelectedObjects_Here.Clear();
                IMG_Background.enabled = false;
            }

            if (List_SelectedObjects_Here.Count > 0)
            {
                float ValueTotal = 0;
                for (int i = 0; i < List_SelectedObjects_Here.Count; i++)
                {
                    ValueTotal += List_SelectedObjects_Here[i].Cost;
                }

                string AmountSelected = $"{List_SelectedObjects_Here.Count} objects selected";
                string TotalValue = $"Total value: ${ValueTotal:F2}m";

                TXT_AmountSelected.text = AmountSelected;
                TXT_CostTotal.text = TotalValue;

                float preferredWidth_A = TXT_CostTotal.GetPreferredValues(TotalValue).x;
                float preferredWidth_B = TXT_AmountSelected.GetPreferredValues(AmountSelected).x;

                float Length = 0;
                if(preferredWidth_A > preferredWidth_B)
                {
                    Length = preferredWidth_A;
                }
                else
                {
                    Length = preferredWidth_B;
                }

                float padding = 20f;

                Vector2 sizeDelta = IMG_Background.rectTransform.sizeDelta;
                sizeDelta.x = preferredWidth_A + padding;
                IMG_Background.rectTransform.sizeDelta = sizeDelta;
            }
            else
            {
                TXT_AmountSelected.text = "";
                TXT_CostTotal.text = "";
            }
        }
    }
}