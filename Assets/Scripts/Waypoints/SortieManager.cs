using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Splines;

public class SortieManager : MonoBehaviour
{
    [SerializeField] List<Sortie> List_Sorties = new List<Sortie>();
    [HideInInspector][SerializeField] GameObject Prefab_Sortie;
    [HideInInspector][SerializeField] GameObject Prefab_Waypoint;
    [SerializeField] bool Metric;

    [SerializeField] int ActualSortie;
    [SerializeField] int ActualSortie_Old;
    [SerializeField] int AmountSorties_Old;

    [HideInInspector][SerializeField] GameObject OBJ_BTN_SwitchMeasurement;
    [HideInInspector][SerializeField] List<GameObject> List_SortieButtons = new List<GameObject>();
    [HideInInspector][SerializeField] GameObject OBJ_INP_Speed;
    [HideInInspector][SerializeField] TMP_InputField INP_Speed;
    [HideInInspector][SerializeField] TMP_Text TXT_Input_Placeholder;
    [HideInInspector][SerializeField] GameObject OBJ_ActualMeasurement;
    [HideInInspector][SerializeField] TMP_Text TXT_ActualMeasurement;
    [HideInInspector][SerializeField] GameObject OBJ_SortieInfo;
    [HideInInspector][SerializeField] TMP_Text TXT_Distance_Total;
    [HideInInspector][SerializeField] TMP_Text TXT_TravelTime_Total;

    [SerializeField] Color Color_BTN_Active;
    [SerializeField] Color Color_BTN_Inactive;
    [SerializeField] Color Color_BTN_Empty;

    Inputs Inputs;
    MouseWorldPosition MWP;
    SortieWing SortieWing;

    bool ResetActive;
    int ResetCounter;
    int ResetFrames;
    int ResetSortie;

    private void Awake()
    {
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
        MWP = GameObject.Find("+---Control Center---+").GetComponent<MouseWorldPosition>();
        SortieWing = GetComponent<SortieWing>();

        ResetSortie = -1;
    }

    private void Start()
    {
        OBJ_BTN_SwitchMeasurement.SetActive(false);
        OBJ_INP_Speed.SetActive(false);
        OBJ_ActualMeasurement.SetActive(false);
        OBJ_SortieInfo.SetActive(false);
        SortieWing.DisableWingDisplay();

        foreach (var Object in List_SortieButtons)
        {
            Object.SetActive(false);
        }
    }

    private void Update()
    {
        PlaceWaypoint();
        DeleteLastWaypoint();

        HighlightSelectedSortie();
        SortieButtonsManagement();

        RestSorties();
    }

    void PlaceWaypoint()
    {
        if(Inputs.LShift_Held && Inputs.Select_Down)
        {
            if (List_Sorties.Count < ActualSortie + 1)
            {
                CreateNewSortie();
            }

            Vector3 Position = MWP.MousePosition;
            Position.y = 25f;

            if (List_Sorties[ActualSortie].List_InactiveWaypoints.Count == 0)
            {
                GameObject NewWaypoint = GameObject.Instantiate(Prefab_Waypoint);
                NewWaypoint.transform.position = Position;

                List_Sorties[ActualSortie].AddWaypoint(NewWaypoint, Metric);
            }

            if(List_Sorties[ActualSortie].List_InactiveWaypoints.Count > 0)
            {
                GameObject NewWaypoint = List_Sorties[ActualSortie].List_InactiveWaypoints[0];
                NewWaypoint.transform.position = Position;

                List_Sorties[ActualSortie].AddWaypoint(NewWaypoint, Metric);
                List_Sorties[ActualSortie].List_InactiveWaypoints.RemoveAt(0);
            }

            RefreshSortieButtons();
        }
    }

    void DeleteLastWaypoint()
    {
        if (Inputs.LShift_Held && Inputs.Deselect_Down)
        {
            if (List_Sorties[ActualSortie] == null)
            {
                return;
            }

            if (List_Sorties[ActualSortie].List_Waypoints.Count == 2)
            {
                SortieWing.Stop();
            }
            if(List_Sorties[ActualSortie].List_Waypoints.Count == 1)
            {
                SortieWing.ClearWing();
            }

            List_Sorties[ActualSortie].RemoveLastWaypoint();
            RefreshSortieButtons();
        }
    }

    void CreateNewSortie()
    {
        //Metric = false;
        
        GameObject NewSortie = GameObject.Instantiate(Prefab_Sortie);
        NewSortie.transform.position = Vector3.zero;
        NewSortie.transform.localRotation = Quaternion.identity;

        NewSortie.transform.SetParent(transform);
        NewSortie.transform.name = $"Sortie_{ActualSortie}";

        List_Sorties.Add(NewSortie.GetComponent<Sortie>());
    }

    public void SwitchMeasurement()
    {
        Metric = !Metric;

        for (int i = 0; i < List_Sorties.Count; i++)
        {
            List_Sorties[i].SwitchMeasurement(Metric);
        }

        if(Metric)
        {
            float speedMetric = 0;            
            if (float.TryParse(INP_Speed.text, out float inpValue))
            {
                speedMetric = inpValue * 1.852f;
            }
            INP_Speed.text = $"{speedMetric}";

            TXT_Input_Placeholder.text = "speed";
            TXT_ActualMeasurement.text = $"metric | km/h";
        }
        else
        {
            float speedImperial = 0;
            if (float.TryParse(INP_Speed.text, out float inpValue))
            {
                speedImperial = inpValue / 1.852f;
            }
            INP_Speed.text = $"{speedImperial}";

            TXT_Input_Placeholder.text = "speed";
            TXT_ActualMeasurement.text = $"imperial | kt";
        }

        string STR_Distance = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelDistanceString();
        string STR_TravelTime = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelTimeString();

        TXT_Distance_Total.text = STR_Distance;
        TXT_TravelTime_Total.text = STR_TravelTime;

        Debug.Log("Switch");
    }

    void UpdateMeasurement()
    {
        for (int i = 0; i < List_Sorties.Count; i++)
        {
            List_Sorties[i].SwitchMeasurement(Metric);
        }

        if (Metric)
        {
            if(List_Sorties.Count > ActualSortie)
            {
                float speedMetric = List_Sorties[ActualSortie].TravelSpeed;
                INP_Speed.text = $"{speedMetric}";
            }
            else
            {
                INP_Speed.text = $"{0f}";
            }
            
            TXT_Input_Placeholder.text = "speed";
            TXT_ActualMeasurement.text = $"metric | km/h";
        }
        else
        {
            if(List_Sorties.Count > ActualSortie)
            {
                float speedImperial = List_Sorties[ActualSortie].TravelSpeed / 1.852f;
                INP_Speed.text = $"{speedImperial}";
            }
            else
            {
                INP_Speed.text = $"{0f}";
            }

            TXT_Input_Placeholder.text = "speed";
            TXT_ActualMeasurement.text = $"imperial | kt";
        }

        if (List_Sorties.Count > ActualSortie)
        {
            if (List_Sorties[ActualSortie].List_Waypoints.Count > 0)
            {
                string STR_Distance = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelDistanceString();
                string STR_TravelTime = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelTimeString();

                TXT_Distance_Total.text = STR_Distance;
                TXT_TravelTime_Total.text = STR_TravelTime;
            }
        }
    }

    public void ChangeSortie(int Index)
    {
        ActualSortie = Index;
        UpdateMeasurement();
    }

    void SortieButtonsManagement()
    {
        int AmountSorties = List_Sorties.Count;

        if(AmountSorties_Old != AmountSorties)
        {
            AmountSorties_Old = AmountSorties;
            RefreshSortieButtons();
        }

        if(ActualSortie_Old != ActualSortie)
        {
            ActualSortie_Old = ActualSortie;
            RefreshSortieButtons();
        }
    }

    void RefreshSortieButtons()
    {
        //Alle Buttons inaktiv setzen
        for (int i = 0; i < List_SortieButtons.Count; i++)
        {
            GameObject SortieButton = List_SortieButtons[i];
            Image ButtonImage = SortieButton.GetComponent<Image>();
            ButtonImage.color = Color_BTN_Inactive;

            SortieButton.SetActive(false);
        }
        OBJ_BTN_SwitchMeasurement.SetActive(false);
        OBJ_INP_Speed.SetActive(false);
        OBJ_ActualMeasurement.SetActive(false);
        OBJ_SortieInfo.SetActive(false);
        SortieWing.DisableWingDisplay();

        //Es wird geprüft, ob eine Sortie über Wegpunkte verfügt. Leere Sorties werden nicht bedacht.
        int AmountSortiesWithWaypoint = 0;
        for (int i = 0; i < List_Sorties.Count; i++)
        {
            if (List_Sorties[i].List_Waypoints.Count > 0)
            {
                AmountSortiesWithWaypoint = i + 1;
            }
        }

        if(AmountSortiesWithWaypoint == 0)
        {
            ActualSortie = 0;
        }

        if (AmountSortiesWithWaypoint > 0)
        {
            OBJ_BTN_SwitchMeasurement.SetActive(true);
            OBJ_INP_Speed.SetActive(true);
            OBJ_ActualMeasurement.SetActive(true);
            OBJ_SortieInfo.SetActive(true);

            if (List_Sorties.Count > ActualSortie)
            {
                if (List_Sorties[ActualSortie].List_Waypoints.Count > 0)
                {
                    SplineContainer SortieSplineContainer = List_Sorties[ActualSortie].GetSplineContainer();
                    float SortieSpeed = List_Sorties[ActualSortie].TravelSpeed;
                    SortieWing.EnableWingDisplay(ActualSortie, SortieSplineContainer, SortieSpeed);
                }
            }

            if (Metric)
            {
                TXT_ActualMeasurement.text = $"metric | km/h";
            }
            else
            {
                TXT_ActualMeasurement.text = $"imperial | kt";
            }

            if (List_Sorties.Count > ActualSortie)
            {
                float TravelSpeed = List_Sorties[ActualSortie].TravelSpeed;
                
                if(Metric)
                {
                    INP_Speed.text = $"{TravelSpeed}";
                }
                else
                {
                    INP_Speed.text = $"{TravelSpeed / 1.852f}";
                }


                if (List_Sorties[ActualSortie].List_Waypoints.Count > 0)
                {
                    string STR_Distance = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelDistanceString();
                    string STR_TravelTime = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelTimeString();

                    TXT_Distance_Total.text = STR_Distance;
                    TXT_TravelTime_Total.text = STR_TravelTime;
                }
            }

            for (int i = 0; i < AmountSortiesWithWaypoint; i++)
            {
                List_SortieButtons[i].SetActive(true);

                if (List_Sorties[i].List_Waypoints.Count == 0)
                {
                    GameObject SortieButton2 = List_SortieButtons[i];
                    Image ButtonImage2 = SortieButton2.GetComponent<Image>();
                    ButtonImage2.color = Color_BTN_Empty;
                }
            }

            if (AmountSortiesWithWaypoint < 9)
            {
                List_SortieButtons[AmountSortiesWithWaypoint].SetActive(true);

                GameObject SortieButton2 = List_SortieButtons[AmountSortiesWithWaypoint];
                Image ButtonImage2 = SortieButton2.GetComponent<Image>();
                ButtonImage2.color = Color_BTN_Empty;
            }

            GameObject SortieButton = List_SortieButtons[ActualSortie];
            Image ButtonImage = SortieButton.GetComponent<Image>();
            ButtonImage.color = Color_BTN_Active;
        }
    }

    void HighlightSelectedSortie()
    {
        if (ActualSortie_Old != ActualSortie)
        {
            for (int i = 0; i < List_Sorties.Count; i++)
            {
                if(i != ActualSortie)
                {
                    List_Sorties[i].Deselect(Color_BTN_Inactive);
                }
                else
                {
                    List_Sorties[i].Select(Color_BTN_Active);
                }                
            }
        }
    }

    public void ChangeSortieSpeed()
    {
        if (OBJ_INP_Speed == null)
        {
            return;
        }

        if (INP_Speed == null || string.IsNullOrEmpty(INP_Speed.text))
        {
            return;
        }

        if (float.TryParse(INP_Speed.text, out float inpValue))
        {
            float speedMetric = 0;

            if (Metric)
            {
                speedMetric = inpValue;
            }

            if(!Metric)
            {
                speedMetric = inpValue * 1.852f;
            }

            if (List_Sorties != null && ActualSortie >= 0 && ActualSortie < List_Sorties.Count)
            {
                if (List_Sorties[ActualSortie] != null)
                {
                    List_Sorties[ActualSortie].SetTravelSpeed(speedMetric);
                }
            }

            SortieWing.UpdateSortieSpeed(speedMetric);
        }

        string STR_Distance = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelDistanceString();
        string STR_TravelTime = List_Sorties[ActualSortie].List_Waypoints[List_Sorties[ActualSortie].List_Waypoints.Count - 1].GetComponent<Waypoint>().TravelTimeString();

        TXT_Distance_Total.text = STR_Distance;
        TXT_TravelTime_Total.text = STR_TravelTime;
    }

    //Reset der Sorties nach dem laden, damit alle Airframes an der Startposition sind.
    public void ResetSortiesAtStart()
    {
        ResetActive = true;
    }
    void RestSorties()
    {
        if(ResetActive)
        {
            if(ResetCounter < ResetFrames)
            {
                ResetCounter++;
            }
            else
            {
                ResetCounter = 0;

                if (ResetSortie == -1)
                {
                    ResetSortie = List_Sorties.Count - 1;
                }

                ChangeSortie(ResetSortie);

                ResetSortie--;
                if(ResetSortie == -1)
                {
                    ResetActive = false;
                }
            }
        }
    }
}
