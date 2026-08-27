using UnityEngine;
using UnityEngine.UI;

public class UI_Notes : MonoBehaviour
{
    ArrowsManager ArrowsManager;
    [SerializeField] Slider SLI_Width;

    [SerializeField] Slider SLI_Color_R;
    [SerializeField] Slider SLI_Color_G;
    [SerializeField] Slider SLI_Color_B;

    Arrow Arrow;
    Color ArrowSelectedColor;

    private void Awake()
    {
        ArrowsManager = GameObject.Find("+---Notes---+").GetComponent<ArrowsManager>();
    }

    private void Start()
    {
        SLI_Width.value = 0.5f;
        SLI_Color_R.value = 0.5f;
        SLI_Color_G.value = 0.5f;
        SLI_Color_B.value = 0.5f;
    }

    public void SetWidth()
    {
        if(ArrowsManager.SelectedArrow == null)
        {
            return;
        }
        
        Arrow = ArrowsManager.SelectedArrow.GetComponent<Arrow>();
        Arrow.SetScaleFaktor(SLI_Width.value * 2f);
    }

    public void SetColor()
    {
        if (ArrowsManager.SelectedArrow == null)
        {
            return;
        }

        ArrowSelectedColor.r = SLI_Color_R.value;
        ArrowSelectedColor.g = SLI_Color_G.value;
        ArrowSelectedColor.b = SLI_Color_B.value;

        Arrow = ArrowsManager.SelectedArrow.GetComponent<Arrow>();
        Arrow.SetUnselectedColor(ArrowSelectedColor);
    }
}
