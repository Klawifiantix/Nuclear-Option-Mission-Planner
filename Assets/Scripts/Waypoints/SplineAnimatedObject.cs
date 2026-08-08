using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Splines;

public class SplineAnimatedObject : MonoBehaviour
{
    public SplineAnimate SplineAnimate;

    public float Speed;
    public bool Start;
    public bool Pause;
    public bool Reset;

    [SerializeField] List<Image> List_MapIcons = new List<Image>();
    [SerializeField] List<int> List_MapIcons_Indicies = new List<int>();
    [SerializeField] int LastUsedMapIcon;

    private void Awake()
    {
        SplineAnimate = GetComponent<SplineAnimate>();
    }

    private void Update()
    {
        SplineAnimate.MaxSpeed = Speed / 3.6f;
        
        if(Start)
        {
            Start = false;

            SplineAnimate.Play();
        }

        if(Pause)
        {
            Pause = false;

            SplineAnimate.Pause();
        }

        if(Reset)
        {
            Reset = false;

            SplineAnimate.Restart(false);
        }
    }

    public void AddMapIcon(Sprite MapIcon, int MapInconIndex)
    {
        if(LastUsedMapIcon < List_MapIcons.Count)
        {
            List_MapIcons_Indicies.Add(MapInconIndex);

            List_MapIcons[LastUsedMapIcon].sprite = MapIcon;
            Color ActualColor = List_MapIcons[LastUsedMapIcon].color;
            ActualColor.a = 1;
            List_MapIcons[LastUsedMapIcon].color = ActualColor;
            LastUsedMapIcon++;
        }
    }

    public void RemoveLastMapIcon()
    {
        if(LastUsedMapIcon > -1)
        {
            List_MapIcons_Indicies.RemoveAt(List_MapIcons_Indicies.Count - 1);
            LastUsedMapIcon -= 1;
            List_MapIcons[LastUsedMapIcon].sprite = null;
            Color ActualColor = List_MapIcons[LastUsedMapIcon].color;
            ActualColor.a = 0;
            List_MapIcons[LastUsedMapIcon].color = ActualColor;
        }
    }

    public void ClearWing()
    {
        for (int i = 0; i < List_MapIcons.Count; i++)
        {
            Color ActualColor = List_MapIcons[i].color;
            ActualColor.a = 0;
            List_MapIcons[i].color = ActualColor;

            List_MapIcons[i].sprite = null;
        }
        LastUsedMapIcon = 0;
        SplineAnimate.Container = null;
    }
}
