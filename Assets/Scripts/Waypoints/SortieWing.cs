using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Splines;
using TMPro;
public class SortieWing : MonoBehaviour
{
    [SerializeField] GameObject OBJ_Wing;
    [SerializeField] GameObject OBJ_BTN_Sub;
    [SerializeField] GameObject OBJ_BTN_Select;
    [SerializeField] List<Sprite> List_Sprites_MapIcons = new List<Sprite>();

    [SerializeField] List<Sprite> List_Sprites_Sortie_0 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_1 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_2 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_3 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_4 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_5 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_6 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_7 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_8 = new List<Sprite>();
    [SerializeField] List<Sprite> List_Sprites_Sortie_9 = new List<Sprite>();
    List<List<Sprite>> List_Sprites_Sorties = new List<List<Sprite>>();    
    
    [SerializeField] List<Image> List_MapIcons = new List<Image>();

    [SerializeField] List<GameObject> List_AnimatedObjects = new List<GameObject>();
    [SerializeField] GameObject OBJ_BTN_Play;
    [SerializeField] GameObject OBJ_BTN_Stop;
    [SerializeField] TMP_Text TXT_Play;

    int ActualSortie;
    SplineContainer ActualSplineContainer;
    SplineAnimatedObject ActualSplineAnimatedObject;

    [SerializeField] float ActualSortieSpeed;
    [SerializeField] bool IsPlaying;

    private void Start()
    {
        List_Sprites_Sorties.Add(List_Sprites_Sortie_0);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_1);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_2);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_3);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_4);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_5);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_6);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_7);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_8);
        List_Sprites_Sorties.Add(List_Sprites_Sortie_9);
    }

    public void EnableWingDisplay(int ActualSortieFresh, SplineContainer ActualSplineContainerFresh, float SortieSpeed)
    {
        ActualSortie = ActualSortieFresh;
        ActualSplineContainer = ActualSplineContainerFresh;
        ActualSortieSpeed = SortieSpeed;

        ActualSplineAnimatedObject = List_AnimatedObjects[ActualSortie].GetComponent<SplineAnimatedObject>();
        ActualSplineAnimatedObject.Speed = ActualSortieSpeed;
        if (ActualSplineContainer != null)
        {
            ActualSplineAnimatedObject.SplineAnimate.Container = ActualSplineContainer;
            if (!ActualSplineAnimatedObject.SplineAnimate.IsPlaying)
            {
                ActualSplineAnimatedObject.SplineAnimate.Restart(false);
                IsPlaying = false;
                ShowPlaySign();
            }
            else
            {
                IsPlaying = true;
                ShowPauseSign();
            }
        }
               

        OBJ_Wing.SetActive(true);

        for (int i = 0; i < List_MapIcons.Count; i++)
        {
            if (List_Sprites_Sorties[ActualSortie][i] != null)
            {
                List_MapIcons[i].enabled = true;
                List_MapIcons[i].sprite = List_Sprites_Sorties[ActualSortie][i];
                OBJ_BTN_Sub.SetActive(true);
                OBJ_BTN_Play.SetActive(true);
                OBJ_BTN_Stop.SetActive(true);
            }
        }
    }

    public void DisableWingDisplay()
    {
        OBJ_Wing.SetActive(false);
        OBJ_BTN_Sub.SetActive(false);
        OBJ_BTN_Select.SetActive(false);
        OBJ_BTN_Play.SetActive(false);
        OBJ_BTN_Stop.SetActive(false);

        for (int i = 0; i < List_MapIcons.Count; i++)
        {
            List_MapIcons[i].enabled = false;
        }
    }

    public void OpenSelection()
    {
        OBJ_BTN_Select.SetActive(!OBJ_BTN_Select.activeSelf);
    }

    public void AddMapIcon(int Index)
    {
        Sprite NewSprite = List_Sprites_MapIcons[Index];

        for (int i = 0; i < List_MapIcons.Count; i++)
        {
            if (List_Sprites_Sorties[ActualSortie][i] == null)
            {
                //List_MapIcons[i].enabled = true;
                List_Sprites_Sorties[ActualSortie][i] = NewSprite;
                //List_MapIcons[i].sprite = NewSprite;

                ActualSplineAnimatedObject.AddMapIcon(NewSprite, Index);

                OBJ_BTN_Sub.SetActive(true);
                OBJ_BTN_Play.SetActive(true);
                OBJ_BTN_Stop.SetActive(true);
                return;
            }
        }
    }

    public void RemoveLastMapIcon()
    {
        for (int i = List_MapIcons.Count -1; i >= 0; i--)
        {
            if(List_Sprites_Sorties[ActualSortie][i] != null)
            {
                List_Sprites_Sorties[ActualSortie][i] = null;
                //List_MapIcons[i].sprite = null;
                //List_MapIcons[i].enabled = false;

                ActualSplineAnimatedObject.RemoveLastMapIcon();

                if (i == 0)
                {
                    OBJ_BTN_Sub.SetActive(false);
                    OBJ_BTN_Play.SetActive(false);
                    OBJ_BTN_Stop.SetActive(false);
                    ActualSplineAnimatedObject.SplineAnimate.Restart(false);
                }
                return;
            }
        }
    }

    public void StartPause()
    {
        if(!IsPlaying)
        {
            ActualSplineAnimatedObject.Start = true;
            IsPlaying = true;
            ShowPauseSign();
            OBJ_BTN_Select.SetActive(false);
        }
        else
        {
            ActualSplineAnimatedObject.Pause = true;
            IsPlaying = false;
            ShowPlaySign();
            OBJ_BTN_Select.SetActive(false);
        }
    }

    public void Stop()
    {
        if(IsPlaying)
        {
            ActualSplineAnimatedObject.Reset = true;
            IsPlaying = false;
            ShowPlaySign();
            OBJ_BTN_Select.SetActive(false);
        }
    }

    public void UpdateSortieSpeed(float Speed)
    {
        ActualSortieSpeed = Speed;
        ActualSplineAnimatedObject.Speed = ActualSortieSpeed;
    }

    public void ClearWing()
    {
        for (int i = 0; i < List_Sprites_Sorties[ActualSortie].Count; i++)
        {
            List_Sprites_Sorties[ActualSortie][i] = null;
        }
        IsPlaying = false;
        ActualSplineAnimatedObject.ClearWing();
    }

    void ShowPlaySign()
    {
        TXT_Play.fontSize = 72.86f;
        TXT_Play.text = ">";
    }

    void ShowPauseSign()
    {
        TXT_Play.fontSize = 45f;
        TXT_Play.text = "||";
    }
}
