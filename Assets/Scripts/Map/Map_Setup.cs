using UnityEngine;
using System.Collections.Generic;

public class Map_Setup : MonoBehaviour
{
    [SerializeField] List<Texture2D> List_MapVersions_Heartland = new List<Texture2D>();
    [SerializeField] List<Texture2D> List_MapVersions_Ignus = new List<Texture2D>();

    [SerializeField] int IndexMap;
    int IndexTexture;

    Transform TRANS_Map;
    Material MAT_Map;
    Inputs Inputs;

    private void Awake()
    {
        TRANS_Map = GetComponent<Transform>();
        MAT_Map = GetComponent<MeshRenderer>().materials[0];
        Inputs = GameObject.Find("+---Control Center---+").GetComponent<Inputs>();
    }

    private void Start()
    {
        SetMapTexture();
    }

    private void Update()
    {
        if(Inputs.ChangeStyle_Down)
        {
            SetMapTexture();
        }
    }

    void SetMapTexture()
    {
        if(IndexMap == 0)
        {
            Texture2D TEX_Map = List_MapVersions_Heartland[IndexTexture];
            MAT_Map.SetTexture("_Input", TEX_Map);
            SetMapSize(82000f, 82000f);
        }
        if(IndexMap == 1)
        {
            Texture2D TEX_Map = List_MapVersions_Ignus[IndexTexture];
            MAT_Map.SetTexture("_Input", TEX_Map);
            SetMapSize(164000f, 164000f);
        }

        IndexTexture++;
        if(IndexMap == 0)
        {
            if (IndexTexture >= List_MapVersions_Heartland.Count)
            {
                IndexTexture = 0;
            }
        }
        if(IndexMap == 1)
        {
            if (IndexTexture >= List_MapVersions_Ignus.Count)
            {
                IndexTexture = 0;
            }
        }
        
    }

    void SetMapSize(float X, float Y)
    {
        Vector3 NewScale = new Vector3(X, Y, 1);
        TRANS_Map.localScale = NewScale;
    }

    public void InitializeMap(int Index)
    {
        IndexMap = Index;
        
        if (IndexMap == 0)
        {
            Texture2D TEX_Map = List_MapVersions_Heartland[0];
            MAT_Map.SetTexture("_Input", TEX_Map);
            SetMapSize(82000f, 82000f);
        }
        if (IndexMap == 1)
        {
            Texture2D TEX_Map = List_MapVersions_Ignus[0];
            MAT_Map.SetTexture("_Input", TEX_Map);
            SetMapSize(164000f, 164000f);
        }
    }

    public int GetCurrentMapIndex()
    {
        return IndexMap;
    }
}
