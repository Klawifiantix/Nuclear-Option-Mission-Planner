using UnityEngine;
using TMPro;
public class Waypoint : MonoBehaviour
{
    Material MAT_Waypoint;
    Camera MainCamera;
    [SerializeField] TMP_Text TXT_ID;
    [SerializeField] TMP_Text TXT_Distance;
    [SerializeField] TMP_Text TXT_TravelTime;

    public float Distance;
    [SerializeField] float TravelTime;

    public bool HoverTarget;
    private void Awake()
    {
        MAT_Waypoint = GetComponent<MeshRenderer>().materials[0];
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        SetScale();
    }

    void SetScale()
    {
        float Scale = 25;
        
        if(HoverTarget)
        {
            Scale = 100;
        }

        Vector3 NewScale = new Vector3(Scale, 1f, Scale);
        transform.localScale = NewScale;
    }

    public void SetColor(Color Color, float Alpha)
    {
        MAT_Waypoint.SetColor("_Color", Color);
        MAT_Waypoint.SetFloat("_Divider", Alpha);
    }

    public void SetInfo(bool Metric, int ID_Fresh, float Distance_Fresh, float TravelTime_Fresh)
    {
        Distance = Distance_Fresh;
        TravelTime = TravelTime_Fresh;        
        
        TXT_ID.text = ID_Fresh.ToString();

        string STR_Distance = "";

        if (Metric)
        {
            if (Distance_Fresh < 1000)
            {
                STR_Distance = $"{Distance_Fresh:F2}m";
            }
            else
            {
                STR_Distance = $"{Distance_Fresh / 1000:F2}km";
            }
        }

        if (!Metric)
        {
            if (Distance_Fresh < 1852f)
            {
                float distanceFeet = Distance_Fresh * 3.28084f;
                STR_Distance = $"{distanceFeet:F2}ft";
            }
            else
            {
                float distanceNM = Distance_Fresh / 1852f;
                STR_Distance = $"{distanceNM:F2}NM";
            }
        }

        TXT_Distance.text = STR_Distance;

        int totalSeconds = Mathf.RoundToInt(TravelTime_Fresh);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string time = $"{minutes:D2}:{seconds:D2}";

        TXT_TravelTime.text = time;
    }

    public void SwitchMeasurement(bool Metric)
    {
        string STR_Distance = "";

        if (Metric)
        {
            if (Distance < 1000)
            {
                STR_Distance = $"{Distance:F2}m";
            }
            else
            {
                STR_Distance = $"{Distance / 1000:F2}km";
            }
        }

        if (!Metric)
        {
            if (Distance < 1852f)
            {
                float distanceFeet = Distance * 3.28084f;
                STR_Distance = $"{distanceFeet:F2}ft";
            }
            else
            {
                float distanceNM = Distance / 1852f;
                STR_Distance = $"{distanceNM:F2}NM";
            }
        }

        TXT_Distance.text = STR_Distance;

        int totalSeconds = Mathf.RoundToInt(TravelTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string time = $"{minutes:D2}:{seconds:D2}";

        TXT_TravelTime.text = time;
    }

    public void UpdateTravelTime(float TravelTime_Fresh)
    {
        TravelTime = TravelTime_Fresh;
        
        int totalSeconds = Mathf.RoundToInt(TravelTime_Fresh);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string time = $"{minutes:D2}:{seconds:D2}";

        TXT_TravelTime.text = time;
    }

    public string TravelDistanceString()
    {
        return TXT_Distance.text;
    }

    public string TravelTimeString()
    {
        return TXT_TravelTime.text;
    }
}
