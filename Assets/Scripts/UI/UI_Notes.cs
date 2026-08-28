using UnityEngine;
using UnityEngine.UI;

public class UI_Notes : MonoBehaviour
{
    Notes Notes;
    [SerializeField] Slider SLI_Width;

    [SerializeField] Slider SLI_Color_R;
    [SerializeField] Slider SLI_Color_G;
    [SerializeField] Slider SLI_Color_B;

    [SerializeField] GameObject OBJ_BackgroundArrow;
    [SerializeField] GameObject OBJ_BackgroundArea;

    [SerializeField] GameObject OBJ_Width;
    [SerializeField] GameObject OBJ_Color_R;
    [SerializeField] GameObject OBJ_Color_G;
    [SerializeField] GameObject OBJ_Color_B;
    Arrow Arrow;
    Area Area;
    Color NoteUnselectedColor;

    private void Awake()
    {
        Notes = GameObject.Find("+---Notes---+").GetComponent<Notes>();
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
        if(Notes.SelectedNote == null)
        {
            return;
        }

        if(Notes.SelectedNote.GetComponent<Arrow>())
        {
            Arrow = Notes.SelectedNote.GetComponent<Arrow>();
            Arrow.SetScaleFaktor(SLI_Width.value * 2f);
        }
    }

    public void SetColor()
    {
        if (Notes.SelectedNote == null)
        {
            return;
        }

        NoteUnselectedColor.r = SLI_Color_R.value;
        NoteUnselectedColor.g = SLI_Color_G.value;
        NoteUnselectedColor.b = SLI_Color_B.value;

        if(Notes.SelectedNote.GetComponent<Arrow>())
        {
            Arrow = Notes.SelectedNote.GetComponent<Arrow>();
            Arrow.SetUnselectedColor(NoteUnselectedColor);
        }

        if(Notes.SelectedNote.GetComponent<Area>())
        {
            Area = Notes.SelectedNote.GetComponent<Area>();
            Area.SetUnselectedColor(NoteUnselectedColor);
        }
    }

    public void DisplayArrowControls()
    {
        OBJ_BackgroundArrow.SetActive(true);
        OBJ_BackgroundArea.SetActive(false);

        OBJ_Width.SetActive(true);
        OBJ_Color_R.SetActive(true);
        OBJ_Color_G.SetActive(true);
        OBJ_Color_B.SetActive(true);
    }

    public void DisplayAreaControls()
    {
        OBJ_BackgroundArrow.SetActive(false);
        OBJ_BackgroundArea.SetActive(true);

        OBJ_Width.SetActive(false);
        OBJ_Color_R.SetActive(true);
        OBJ_Color_G.SetActive(true);
        OBJ_Color_B.SetActive(true);
    }

    public void HideAll()
    {
        OBJ_BackgroundArrow.SetActive(false);
        OBJ_BackgroundArea.SetActive(false);

        OBJ_Width.SetActive(false);
        OBJ_Color_R.SetActive(false);
        OBJ_Color_G.SetActive(false);
        OBJ_Color_B.SetActive(false);
    }
}
