using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    [SerializeField] TMP_InputField Inputfield;
    [SerializeField] TMP_Text TXT_Shown;

    [SerializeField] Color COL_Selected;
    [SerializeField] Color COL_Unselected;
    public float COL_Unselected_R;
    public float COL_Unselected_G;
    public float COL_Unselected_B;
    public float ScaleFaktor;
    public string LabelText;

    Material MAT_LabelPicker;
    [SerializeField] Image IMG_Textfield;
    [SerializeField] TMP_Text TXT_Textfield;

    private void Awake()
    {
        MAT_LabelPicker = GetComponent<MeshRenderer>().materials[0];
    }

    private void OnEnable()
    {
        SetUnselectedColor(Color.green);
        SetScale(1);

        SelectLabel();
    }

    public void OnChangeContent()
    {
        LabelText = TXT_Shown.text;

        TXT_Shown.fontSize = 840f;
        float width = TXT_Shown.GetPreferredValues(TXT_Shown.text).x;
        Vector2 sizeDelta = Inputfield.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x = width + 500f;
        Inputfield.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }

    public void SetScale(float Size)
    {
        LabelText = TXT_Shown.text;

        ScaleFaktor = Size;
        float Scale = 50;
        float NewSize = Scale * ScaleFaktor;
        Vector3 NewScale = new Vector3(NewSize, 1, NewSize);

        transform.localScale = NewScale;
    }

    public void SelectLabel()
    {
        LabelText = TXT_Shown.text;

        TXT_Textfield.color = COL_Selected;
        IMG_Textfield.color = COL_Selected / 2;
        MAT_LabelPicker.SetColor("_Color", COL_Selected / 2f);
    }

    public void UnselectLabel()
    {
        LabelText = TXT_Shown.text;

        TXT_Textfield.color = COL_Unselected;
        IMG_Textfield.color = COL_Unselected / 2;
        MAT_LabelPicker.SetColor("_Color", COL_Unselected / 2f);
    }

    public void SetUnselectedColor(Color NewColor)
    {
        COL_Unselected_R = NewColor.r;
        COL_Unselected_G = NewColor.g;
        COL_Unselected_B = NewColor.b;

        COL_Unselected = NewColor;
        COL_Unselected.a = 1;
        UnselectLabel();
    }

    public void SetText(string Text)
    {
        Inputfield.text = Text;
    }

    private void OnDisable()
    {
        TXT_Shown.text = "";
    }
}