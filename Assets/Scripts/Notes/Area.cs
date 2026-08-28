using UnityEngine;
using System.Collections.Generic;

public class Area : MonoBehaviour
{
    MeshFilter MF;
    MeshRenderer MR;
    MeshCollider MC;
    LineRenderer LR;
    Material MAT_Fill;
    Material MAT_Line;

    [SerializeField] List<Vector3> List_AreaCorners = new List<Vector3>();

    [SerializeField] Color COL_Selected;
    [SerializeField] Color COL_Unselected;
    public float COL_Unselected_R;
    public float COL_Unselected_G;
    public float COL_Unselected_B;

    [SerializeField] float Length;

    private void Awake()
    {
        MF = GetComponent<MeshFilter>();
        MR = GetComponent<MeshRenderer>();
        MC = GetComponent<MeshCollider>();
        LR = GetComponent<LineRenderer>();

        MAT_Fill = MR.materials[0];
        MAT_Line = LR.materials[0];
    }

    private void OnEnable()
    {
        LR.positionCount = 0;
        List_AreaCorners.Clear();
        COL_Unselected = Color.yellow;

        COL_Unselected_R = COL_Unselected.r;
        COL_Unselected_G = COL_Unselected.g;
        COL_Unselected_B = COL_Unselected.b;
    }

    private void Start()
    {
        MAT_Line.SetColor("_Color", COL_Unselected);

        Color MaterialColor = COL_Unselected;
        MaterialColor /= 2f;
        MAT_Fill.SetFloat("_Alpha", 0.25f);
        MAT_Fill.SetColor("_Color", MaterialColor);
    }

    public void AddCorner(Vector3 Position)
    {
        List_AreaCorners.Add(Position);
        UpdateArea();
    }

    public bool RemoveLastCorner()
    {
        if (List_AreaCorners.Count == 0)
        {
            return true;
        }

        List_AreaCorners.RemoveAt(List_AreaCorners.Count - 1);
        UpdateArea();

        if (List_AreaCorners.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateArea()
    {
        LR.positionCount = List_AreaCorners.Count;
        LR.SetPositions(List_AreaCorners.ToArray());

        if (List_AreaCorners.Count >= 3)
        {
            GenerateMesh();
        }
        else
        {
            if (MF.sharedMesh != null)
            {
                MF.sharedMesh.Clear();
            }
            if (MC != null)
            {
                MC.sharedMesh = null;
            }
        }

        CalculateLength();
    }

    private void GenerateMesh()
    {
        int vertexCount = List_AreaCorners.Count;

        Vector3[] localVertices = new Vector3[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(List_AreaCorners[i]);
            localPos.y -= 0.1f;
            localVertices[i] = localPos;
        }

        // 1. Haupt-Mesh für den Renderer erzeugen (einfacher Fächer)
        Mesh renderMesh = new Mesh();
        int triangleCount = (vertexCount - 2) * 3;
        int[] renderTriangles = new int[triangleCount];

        int triIndex = 0;
        for (int i = 1; i < vertexCount - 1; i++)
        {
            renderTriangles[triIndex] = 0;
            renderTriangles[triIndex + 1] = i;
            renderTriangles[triIndex + 2] = i + 1;
            triIndex += 3;
        }

        renderMesh.vertices = localVertices;
        renderMesh.triangles = renderTriangles;
        renderMesh.RecalculateNormals();
        renderMesh.RecalculateBounds();

        MF.sharedMesh = renderMesh;

        // 2. Grid-basiertes Collider-Mesh erzeugen, um große Dreiecke & Warnungen zu verhindern
        if (MC != null)
        {
            MC.sharedMesh = null;
            Mesh colliderMesh = CreateGridColliderMesh(localVertices, 200f);
            MC.sharedMesh = colliderMesh;
        }
    }

    private Mesh CreateGridColliderMesh(Vector3[] polygonVertices, float maxCellSize)
    {
        // Bounding Box des Polygons ermitteln
        Bounds bounds = new Bounds(polygonVertices[0], Vector3.zero);
        for (int i = 1; i < polygonVertices.Length; i++)
        {
            bounds.Encapsulate(polygonVertices[i]);
        }

        int xSteps = Mathf.Max(2, Mathf.CeilToInt(bounds.size.x / maxCellSize));
        int zSteps = Mathf.Max(2, Mathf.CeilToInt(bounds.size.z / maxCellSize));

        float dx = bounds.size.x / xSteps;
        float dz = bounds.size.z / zSteps;

        List<Vector3> gridVerts = new List<Vector3>();
        Dictionary<Vector2Int, int> gridIndices = new Dictionary<Vector2Int, int>();

        // Punkte auf dem Grid generieren, die innerhalb des Polygons liegen
        for (int x = 0; x <= xSteps; x++)
        {
            for (int z = 0; z <= zSteps; z++)
            {
                float px = bounds.min.x + x * dx;
                float pz = bounds.min.z + z * dz;
                Vector3 candidate = new Vector3(px, polygonVertices[0].y, pz);

                if (IsPointInPolygon(candidate, polygonVertices))
                {
                    gridIndices[new Vector2Int(x, z)] = gridVerts.Count;
                    gridVerts.Add(candidate);
                }
            }
        }

        // Falls das Polygon zu schmal für das Grid ist, auf das Standard-Polygon-Mesh zurückfallen
        if (gridVerts.Count < 3)
        {
            Mesh fallbackMesh = new Mesh();
            fallbackMesh.vertices = polygonVertices;
            int[] fallbackTris = new int[(polygonVertices.Length - 2) * 3];
            int tIdx = 0;
            for (int i = 1; i < polygonVertices.Length - 1; i++)
            {
                fallbackTris[tIdx++] = 0;
                fallbackTris[tIdx++] = i;
                fallbackTris[tIdx++] = i + 1;
            }
            fallbackMesh.triangles = fallbackTris;
            fallbackMesh.RecalculateBounds();
            return fallbackMesh;
        }

        // Dreiecke für das Grid aufbauen
        List<int> gridTris = new List<int>();
        for (int x = 0; x < xSteps; x++)
        {
            for (int z = 0; z < zSteps; z++)
            {
                Vector2Int p00 = new Vector2Int(x, z);
                Vector2Int p10 = new Vector2Int(x + 1, z);
                Vector2Int p01 = new Vector2Int(x, z + 1);
                Vector2Int p11 = new Vector2Int(x + 1, z + 1);

                bool has00 = gridIndices.ContainsKey(p00);
                bool has10 = gridIndices.ContainsKey(p10);
                bool has01 = gridIndices.ContainsKey(p01);
                bool has11 = gridIndices.ContainsKey(p11);

                if (has00 && has10 && has01)
                {
                    gridTris.Add(gridIndices[p00]);
                    gridTris.Add(gridIndices[p01]);
                    gridTris.Add(gridIndices[p10]);
                }
                if (has10 && has01 && has11)
                {
                    gridTris.Add(gridIndices[p10]);
                    gridTris.Add(gridIndices[p01]);
                    gridTris.Add(gridIndices[p11]);
                }
            }
        }

        Mesh gridMesh = new Mesh();
        gridMesh.vertices = gridVerts.ToArray();
        gridMesh.triangles = gridTris.ToArray();
        gridMesh.RecalculateNormals();
        gridMesh.RecalculateBounds();
        return gridMesh;
    }

    private bool IsPointInPolygon(Vector3 point, Vector3[] polygon)
    {
        bool inside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
        {
            if (((polygon[i].z > point.z) != (polygon[j].z > point.z)) &&
                (point.x < (polygon[j].x - polygon[i].x) * (point.z - polygon[i].z) / (polygon[j].z - polygon[i].z) + polygon[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    void CalculateLength()
    {
        Length = 20;

        if(List_AreaCorners.Count >= 3)
        {
            for (int i = 0; i < List_AreaCorners.Count - 1; i++)
            {
                Length += Vector3.Distance(List_AreaCorners[i], List_AreaCorners[i + 1]);
            }
        }

        MAT_Line.SetFloat("_Length", Length / 500f);
    }

    public void SelectArea()
    {
        COL_Selected.a = 1;
        MAT_Line.SetColor("_Color", COL_Selected);

        COL_Selected.a = 0.5f;
        MAT_Fill.SetColor("_Color", COL_Selected);
    }

    public void UnselectArea()
    {
        MAT_Line.SetColor("_Color", COL_Unselected);

        Color MaterialColor = COL_Unselected;
        MaterialColor /= 2f;
        MAT_Fill.SetFloat("_Alpha", 0.25f);
        MAT_Fill.SetColor("_Color", MaterialColor);
    }

    public void SetUnselectedColor(Color NewColor)
    {
        COL_Unselected_R = NewColor.r;
        COL_Unselected_G = NewColor.g;
        COL_Unselected_B = NewColor.b;

        COL_Unselected = NewColor;
        UnselectArea();
    }
}