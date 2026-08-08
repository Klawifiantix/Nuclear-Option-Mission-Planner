using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Objects/UnitStats")]
public class UnitStats : ScriptableObject
{
    [Header("Type")]
    public bool Building;
    public bool Vehicle;
    public bool Ship;
    public bool Aircraft;
    
    [Header("Picture")]
    public Sprite SPR_Avatar;
    public Sprite SPR_MapIcon;

    [Header("Name")]
    public string STR_Name_Development;
    public string STR_Name_InGame;

    [Header("Characteristics")]
    public float Cost;

    [Header("Performance")]
    public float MaxSpeed;
    public float StallSpeed;
    public float ServiceCeiling;
    public float Maneuverability;
    public float RCS_Clean;
    public float RCS_HeavyMunitions;

    [Header("Onboard Systems")]
    public float Magnification;
    public float MaxOpticalRange;
    public float MaxRadarRange;
    public float AmountFlares;
    public float JammingIntensity;
    public float LaserDesignators;

    [Header("Weapons")]
    public List<WeaponStats> List_Weapons = new List<WeaponStats>();
}
