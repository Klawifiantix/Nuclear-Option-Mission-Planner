using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class OpticalLogic : MonoBehaviour
{
    [SerializeField] List<OpticalDetection> List_OpticalDetection_Airframes = new List<OpticalDetection>();
    [SerializeField] List<OpticalDetection> List_OpticalDetection_Vehicles = new List<OpticalDetection>();
    [SerializeField] List<OpticalDetection> List_OpticalDetection_Ships = new List<OpticalDetection>();
    [SerializeField] List<OpticalDetection> List_OpticalDetection_Buildings = new List<OpticalDetection>();
    [SerializeField] List<OpticalDetection> List_OpticalDetection_Actual_0 = new List<OpticalDetection>();
    [SerializeField] List<OpticalDetection> List_OpticalDetection_Actual_1 = new List<OpticalDetection>();
    [SerializeField] List<string> List_Names_0 = new List<string>();
    [SerializeField] List<string> List_Names_1 = new List<string>();

    [SerializeField] TMP_Dropdown DD_Units_0;
    [SerializeField] TMP_Text TXT_DD_Label_0;

    [SerializeField] TMP_Dropdown DD_Units_1;
    [SerializeField] TMP_Text TXT_DD_Label_1;

    [SerializeField] TMP_Text TXT_VisibleRange_0;
    [SerializeField] TMP_Text TXT_VisualRange_0;
    [SerializeField] TMP_Text TXT_VisualMagnification_0;

    [SerializeField] TMP_Text TXT_VisibleRange_1;
    [SerializeField] TMP_Text TXT_VisualRange_1;
    [SerializeField] TMP_Text TXT_VisualMagnification_1;

    [SerializeField] Image IMG_Avatar_0;
    [SerializeField] Image IMG_Avatar_1;

    [SerializeField] int index;

    [SerializeField] float visibleRange_temp;
    [SerializeField] float visualRange_temp;

    [SerializeField] string Name_0;
    [SerializeField] float visibleRange_0;
    [SerializeField] float visualRange_0;
    [SerializeField] float visualMagnification_0;

    [SerializeField] string Name_1;
    [SerializeField] float visibleRange_1;
    [SerializeField] float visualRange_1;
    [SerializeField] float visualMagnification_1;

    [SerializeField] float DetectionRange_0;
    [SerializeField] float DetectionRange_1;

    [SerializeField] float DetectionRange_0_Temp;
    [SerializeField] float DetectionRange_1_Temp;

    [SerializeField] TMP_Text TXT_Winner;
    [SerializeField] TMP_Text TXT_Looser;

    [SerializeField] bool Imperial;
    string measure;

    //[SerializeField] GameObject OBJ_WinnerBackground;
    [SerializeField] int ActualType_0;
    [SerializeField] int ActualType_1;

    private void Awake()
    {
        ClearAll();
    }

    private void Start()
    {
        SetupDropDownMenues(2);
    }

    public void DropDown_0()
    {
        index = DD_Units_0.value -1;
        if (index < 0)
        {
            return;
        }
        visibleRange_0 = List_OpticalDetection_Actual_0[index].Range_Visible;
        visualRange_0 = List_OpticalDetection_Actual_0[index].Range_Visual;
        visualMagnification_0 = List_OpticalDetection_Actual_0[index].VisualMagnification;

        if (visibleRange_0 >= 1000)
        {
            visibleRange_0 /= 1000;
        }
        if(visualRange_0 >= 1000)
        {
            visualRange_0 /= 1000;
        }

        Name_0 = List_OpticalDetection_Actual_0[index].name;

        measure = "km";
        visibleRange_temp = visibleRange_0;
        visualRange_temp = visualRange_0;

        if(Imperial)
        {
            measure = "nm";
            visibleRange_temp /= 1.852f;
            visualRange_temp /= 1.852f;
        }

        TXT_VisibleRange_0.text = $"{visibleRange_temp:F1}{measure}";
        TXT_VisualRange_0.text = $"{visualRange_temp:F1}{measure}";
        TXT_VisualMagnification_0.text = $"x{visualMagnification_0}";

        IMG_Avatar_0.enabled = true;
        IMG_Avatar_0.sprite = List_OpticalDetection_Actual_0[index].SPR_Avatar;

        CalculateOpticalRanges();
    }
    public void DropDown_1()
    {
        index = DD_Units_1.value -1;
        if(index < 0)
        {
            return;
        }
        visibleRange_1 = List_OpticalDetection_Actual_1[index].Range_Visible;
        visualRange_1 = List_OpticalDetection_Actual_1[index].Range_Visual;
        visualMagnification_1 = List_OpticalDetection_Actual_1[index].VisualMagnification;

        if (visibleRange_1 >= 1000)
        {
            visibleRange_1 /= 1000;
        }
        if (visualRange_1 >= 1000)
        {
            visualRange_1 /= 1000;
        }

        Name_1 = List_OpticalDetection_Actual_1[index].name;

        measure = "km";
        visibleRange_temp = visibleRange_1;
        visualRange_temp = visualRange_1;

        if (Imperial)
        {
            measure = "nm";
            visibleRange_temp /= 1.852f;
            visualRange_temp /= 1.852f;
        }

        TXT_VisibleRange_1.text = $"{visibleRange_temp:F1}{measure}";
        TXT_VisualRange_1.text = $"{visualRange_temp:F1}{measure}";
        TXT_VisualMagnification_1.text = $"x{visualMagnification_1}";

        IMG_Avatar_1.enabled = true;
        IMG_Avatar_1.sprite = List_OpticalDetection_Actual_1[index].SPR_Avatar;

        CalculateOpticalRanges();
    }

    void ClearAll()
    {
        TXT_VisibleRange_0.text = "";
        TXT_VisualRange_0.text = "";
        TXT_VisualMagnification_0.text = "";
        IMG_Avatar_0.enabled = false;

        TXT_VisibleRange_1.text = "";
        TXT_VisualRange_1.text = "";
        TXT_VisualMagnification_1.text = "";
        IMG_Avatar_1.enabled = false;

        TXT_Winner.text = "";
        TXT_Looser.text = "";
    }

    void CalculateOpticalRanges()
    {
        if(Name_0 == "" || Name_1 == "")
        {
            return;
        }
        
        DetectionRange_0 = visibleRange_1 * visualMagnification_0;
        DetectionRange_1 = visibleRange_0 * visualMagnification_1;

        measure = "km";
        DetectionRange_0_Temp = DetectionRange_0;
        DetectionRange_1_Temp = DetectionRange_1;
        if (Imperial)
        {
            measure = "nm";
            DetectionRange_0_Temp /= 1.852f;
            DetectionRange_1_Temp /= 1.852f;
        }

        //OBJ_WinnerBackground.SetActive(true);

        if (DetectionRange_0 > DetectionRange_1)
        {
            //OBJ_WinnerBackground.GetComponent<RectTransform>().position = new Vector3(360, 636, 0);
            TXT_Winner.text = $"The {Name_0} can visually regognize the {Name_1} from a distance of {DetectionRange_0_Temp:F1}{measure}";
            TXT_Looser.text = $"The {Name_1} can visually regognize the {Name_0} from a distance of {DetectionRange_1_Temp:F1}{measure}";
        }
        else
        {
            //OBJ_WinnerBackground.GetComponent<RectTransform>().position = new Vector3(1560, 636, 0);
            TXT_Winner.text = $"The {Name_1} can visually regognize the {Name_0} from a distance of {DetectionRange_1_Temp:F1}{measure}";
            TXT_Looser.text = $"The {Name_0} can visually regognize the {Name_1} from a distance of {DetectionRange_0_Temp:F1}{measure}";
        }
    }

    public void SwitchMeasurement()
    {
        Imperial = !Imperial;

        if(Name_0 != "")
        {
            measure = "km";
            visibleRange_temp = visibleRange_0;
            visualRange_temp = visualRange_0;

            if (Imperial)
            {
                measure = "nm";
                visibleRange_temp /= 1.852f;
                visualRange_temp /= 1.852f;
            }

            TXT_VisibleRange_0.text = $"{visibleRange_temp:F1}{measure}";
            TXT_VisualRange_0.text = $"{visualRange_temp:F1}{measure}";
        }
        
        if(Name_1 != "")
        {
            measure = "km";
            visibleRange_temp = visibleRange_1;
            visualRange_temp = visualRange_1;

            if (Imperial)
            {
                measure = "nm";
                visibleRange_temp /= 1.852f;
                visualRange_temp /= 1.852f;
            }

            TXT_VisibleRange_1.text = $"{visibleRange_temp:F1}{measure}";
            TXT_VisualRange_1.text = $"{visualRange_temp:F1}{measure}";
        }
        
        if(Name_0 == "" || Name_1 == "")
        {
            return;
        }

        //Winner/Looser
        measure = "km";
        DetectionRange_0_Temp = DetectionRange_0;
        DetectionRange_1_Temp = DetectionRange_1;
        if (Imperial)
        {
            measure = "nm";
            DetectionRange_0_Temp /= 1.852f;
            DetectionRange_1_Temp /= 1.852f;
        }

        if (DetectionRange_0 > DetectionRange_1)
        {
            //OBJ_WinnerBackground.GetComponent<RectTransform>().position = new Vector3(360, 636, 0);
            TXT_Winner.text = $"The {Name_0} can visually regognize the {Name_1} from a distance of {DetectionRange_0_Temp:F1}{measure}";
            TXT_Looser.text = $"The {Name_1} can visually regognize the {Name_0} from a distance of {DetectionRange_1_Temp:F1}{measure}";
        }
        else
        {
            //OBJ_WinnerBackground.GetComponent<RectTransform>().position = new Vector3(1560, 636, 0);
            TXT_Winner.text = $"The {Name_1} can visually regognize the {Name_0} from a distance of {DetectionRange_1_Temp:F1}{measure}";
            TXT_Looser.text = $"The {Name_0} can visually regognize the {Name_1} from a distance of {DetectionRange_0_Temp:F1}{measure}";
        }
    }

    public void SetType_0(int Type)
    {
        ActualType_0 = Type;
        SetupDropDownMenues(0);
    }
    public void SetType_1(int Type)
    {
        ActualType_1 = Type;
        SetupDropDownMenues(1);
    }

    void SetupDropDownMenues(int DD_Index)
    {
        if(DD_Index == 0)
        {
            Setupspecific(List_OpticalDetection_Actual_0, ActualType_0, TXT_DD_Label_0, List_Names_0, DD_Units_0);
        }
        if (DD_Index == 1)
        {
            Setupspecific(List_OpticalDetection_Actual_1, ActualType_1, TXT_DD_Label_1, List_Names_1, DD_Units_1);
        }
        if (DD_Index == 2)
        {
            Setupspecific(List_OpticalDetection_Actual_0, ActualType_0, TXT_DD_Label_0, List_Names_0, DD_Units_0);
            Setupspecific(List_OpticalDetection_Actual_1, ActualType_1, TXT_DD_Label_1, List_Names_1, DD_Units_1);
        }
        CalculateOpticalRanges();
    }

    void Setupspecific(List<OpticalDetection> L_OD, int ActualType, TMP_Text TXT_DD_Label, List<string> L_Names, TMP_Dropdown DD)
    {
        L_OD.Clear();
        if (ActualType == 0)
        {
            L_OD.AddRange(List_OpticalDetection_Airframes);
        }
        if (ActualType == 1)
        {
            L_OD.AddRange(List_OpticalDetection_Vehicles);
        }
        if (ActualType == 2)
        {
            L_OD.AddRange(List_OpticalDetection_Ships);
        }
        if (ActualType == 3)
        {
            L_OD.AddRange(List_OpticalDetection_Buildings);
        }

        L_Names.Clear();
        DD.ClearOptions();

        if (ActualType == 0)
        {
            L_Names.Add("Choose Airframe");
        }
        if (ActualType == 1)
        {
            L_Names.Add("Choose Vehicle");
        }
        if (ActualType == 2)
        {
            L_Names.Add("Choose Ship");
        }
        if (ActualType == 3)
        {
            L_Names.Add("Choose Building");
        }

        foreach (var item in L_OD)
        {
            L_Names.Add(item.name);
        }

        DD.AddOptions(L_Names);
        DD.value = 0;
        DD.RefreshShownValue();

        Debug.Log($"{TXT_DD_Label} set to {L_Names[0]}");
        //OBJ_WinnerBackground.SetActive(false);
    }

    public void BTN_MissionPlanner()
    {
        SceneManager.LoadScene("MissionPlanner");
    }

    public void BTN_Exit()
    {
        Application.Quit();
    }
}
