using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Sortie : MonoBehaviour
{
    LineRenderer LR;
    public List<GameObject> List_Waypoints = new List<GameObject>();
    public List<GameObject> List_InactiveWaypoints = new List<GameObject>();

    [SerializeField] float Distance;
    public float TravelSpeed;
    [SerializeField] float TravelTime;

    [SerializeField] SplineContainer SplineContainer;
    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        SetUp_LR();
    }

    public void AddWaypoint(GameObject OBJ_Waypoint, bool Metric)
    {
        OBJ_Waypoint.transform.SetParent(transform);
        OBJ_Waypoint.SetActive(true);

        if (!List_Waypoints.Contains(OBJ_Waypoint))
        {
            if(List_Waypoints.Count == 0)
            {
                Waypoint WP = OBJ_Waypoint.GetComponent<Waypoint>();
                WP.SetInfo(Metric, List_Waypoints.Count, 0, 0);
            }

            if (List_Waypoints.Count > 0)
            {
                Waypoint WP = OBJ_Waypoint.GetComponent<Waypoint>();

                Vector3 PositionPrevious = List_Waypoints[List_Waypoints.Count - 1].transform.position;
                Vector3 PositionActual = OBJ_Waypoint.transform.position;

                Distance += Vector3.Distance(PositionPrevious, PositionActual);

                float SpeedInMS = TravelSpeed / 3.6f;
                TravelTime = Distance / SpeedInMS;

                WP.SetInfo(Metric, List_Waypoints.Count, Distance, TravelTime);
            }


            List_Waypoints.Add(OBJ_Waypoint);

            BezierKnot NewKnot = new BezierKnot((float3)OBJ_Waypoint.transform.position);
            SplineContainer.Spline.Add(NewKnot);

            int lastIndex = SplineContainer.Spline.Count - 1;
            SplineContainer.Spline.SetTangentMode(lastIndex, TangentMode.AutoSmooth);
        }
    }
    public void RemoveLastWaypoint()
    {
        if (List_Waypoints.Count > 0)
        {
            GameObject Waypoint = List_Waypoints[List_Waypoints.Count - 1];

            if(List_Waypoints.Count >= 2)
            {
                Vector3 PositionActual = Waypoint.transform.position;
                Vector3 PositionPrevious = List_Waypoints[List_Waypoints.Count - 2].transform.position;

                Distance -= Vector3.Distance(PositionPrevious, PositionActual);
                float SpeedInMS = TravelSpeed / 3.6f;
                TravelTime = Distance / SpeedInMS;
            }

            Waypoint.SetActive(false);
            if(!List_InactiveWaypoints.Contains(Waypoint))
            {
                List_InactiveWaypoints.Add(Waypoint);
            }

            List_Waypoints.Remove(Waypoint);

            SplineContainer.Spline.RemoveAt(SplineContainer.Spline.Count - 1);
        }
    }
    void SetUp_LR()
    {
        if (SplineContainer == null || SplineContainer.Spline == null || SplineContainer.Spline.Count < 2)
        {
            LR.positionCount = 0;
            return;
        }

        int resolutionPerSegment = 10;
        int totalPoints = (SplineContainer.Spline.Count - 1) * resolutionPerSegment + 1;
        LR.positionCount = totalPoints;

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (float)i / (totalPoints - 1);
            float3 splinePoint = SplineContainer.Spline.EvaluatePosition(t);

            Vector3 pointPosition = SplineContainer.transform.TransformPoint((Vector3)splinePoint);
            pointPosition.y = 20f;

            LR.SetPosition(i, pointPosition);
        }
    }
    public void SwitchMeasurement(bool Metric)
    {
        for (int i = 0; i < List_Waypoints.Count; i++)
        {
            Waypoint WP = List_Waypoints[i].GetComponent<Waypoint>();
            WP.SwitchMeasurement(Metric);
        }
    }
    public void Select(Color Farbe)
    {
        for (int i = 0; i < List_Waypoints.Count; i++)
        {
            Waypoint WP = List_Waypoints[i].GetComponent<Waypoint>();
            WP.SetColor(Farbe, 1f);
        }

        Farbe.a = 0.5f;
        LR.startColor = Farbe;
        LR.endColor = Farbe;
    }
    public void Deselect(Color Farbe)
    {
        Farbe.a = 0.1f;

        for (int i = 0; i < List_Waypoints.Count; i++)
        {
            Waypoint WP = List_Waypoints[i].GetComponent<Waypoint>();
            WP.SetColor(Farbe, 5f);
        }
        
        LR.startColor = Farbe;
        LR.endColor = Farbe;
    }
    public void SetTravelSpeed(float Speed)
    {
        TravelSpeed = Speed;
        float SpeedInMS = TravelSpeed / 3.6f;       

        for (int i = 0; i < List_Waypoints.Count; i++)
        {
            Waypoint WP = List_Waypoints[i].GetComponent<Waypoint>();

            float WP_Distance = WP.Distance;
            TravelTime = WP_Distance / SpeedInMS;
            WP.UpdateTravelTime(TravelTime);
        }
    }

    public SplineContainer GetSplineContainer()
    {
        return SplineContainer;
    }
}
