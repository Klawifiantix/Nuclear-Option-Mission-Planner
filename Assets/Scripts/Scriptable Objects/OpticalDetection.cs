using UnityEngine;

[CreateAssetMenu(fileName = "OpticalDetection", menuName = "Scriptable Objects/OpticalDetection")]
public class OpticalDetection : ScriptableObject
{
    public float Range_Visible;
    public float Range_Visual;
    public float VisualMagnification;
    public Sprite SPR_Avatar;
}
