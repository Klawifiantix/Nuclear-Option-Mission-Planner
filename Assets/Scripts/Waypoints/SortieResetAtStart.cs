using UnityEngine;

public class SortieResetAtStart : MonoBehaviour
{
    SortieManager SortieManager;

    private void Awake()
    {
        SortieManager = GetComponent<SortieManager>();
    }

    public void ResetSorties()
    {

    }
}
