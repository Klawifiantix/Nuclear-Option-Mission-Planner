using UnityEngine;

public class Object_Scaler : MonoBehaviour
{
    Camera MainCamera;

    [SerializeField] float OrthographicSize_Old;
    [SerializeField] float ScaleFaktor;
    [SerializeField] float MinScale;

    private void Awake()
    {
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Vector3 NewScale = new Vector3(MinScale, 1f, MinScale);
        transform.localScale = NewScale;
    }

    private void Update()
    {
        if(MainCamera.orthographicSize != OrthographicSize_Old)
        {
            OrthographicSize_Old = MainCamera.orthographicSize;

            float OrthographicSize = MainCamera.orthographicSize;
            float ObjectScale = OrthographicSize / ScaleFaktor;

            if(ObjectScale <= 20)
            {
                Vector3 NewScale = new Vector3(ObjectScale, 1f, ObjectScale);

                transform.localScale = NewScale;
            }
        }
    }
}
