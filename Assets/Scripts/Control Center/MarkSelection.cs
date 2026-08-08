using System.Collections.Generic;
using UnityEngine;

public class MarkSelection : MonoBehaviour
{
    LineRenderer LR;
    SelectionBox SelectionBox;

    private float radius = 300f;
    private float targetY = 15f;
    private int arcSegments = 12;

    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
        SelectionBox = GameObject.Find("+---Control Center---+").GetComponent<SelectionBox>();
    }

    private void Update()
    {
        DrawSelectionBox();
    }

    private void DrawSelectionBox()
    {
        if (SelectionBox == null || SelectionBox.List_SelectedObjects == null || SelectionBox.List_SelectedObjects.Count == 0)
        {
            if (LR.positionCount > 0)
            {
                LR.positionCount = 0;
            }
            return;
        }

        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < SelectionBox.List_SelectedObjects.Count; i++)
        {
            GameObject obj = SelectionBox.List_SelectedObjects[i];
            if (obj != null)
            {
                points.Add(new Vector2(obj.transform.position.x, obj.transform.position.z));
            }
        }

        if (points.Count == 0)
        {
            LR.positionCount = 0;
            return;
        }

        List<Vector3> linePositions = new List<Vector3>();

        if (points.Count == 1)
        {
            Vector2 center = points[0];
            int segments = 36;
            for (int i = 0; i <= segments; i++)
            {
                float angle = (i / (float)segments) * Mathf.PI * 2f;
                float x = center.x + Mathf.Cos(angle) * radius;
                float z = center.y + Mathf.Sin(angle) * radius;
                linePositions.Add(new Vector3(x, targetY, z));
            }
        }
        else
        {
            List<Vector2> hull = GetConvexHull(points);

            if (hull.Count == 1)
            {
                Vector2 center = hull[0];
                int segments = 36;
                for (int i = 0; i <= segments; i++)
                {
                    float angle = (i / (float)segments) * Mathf.PI * 2f;
                    float x = center.x + Mathf.Cos(angle) * radius;
                    float z = center.y + Mathf.Sin(angle) * radius;
                    linePositions.Add(new Vector3(x, targetY, z));
                }
            }
            else if (hull.Count == 2)
            {
                Vector2 p1 = hull[0];
                Vector2 p2 = hull[1];
                Vector2 dir = (p2 - p1).normalized;
                Vector2 normal = new Vector2(dir.y, -dir.x);

                float startAngle1 = Mathf.Atan2(-normal.y, -normal.x);
                float endAngle1 = Mathf.Atan2(normal.y, normal.x);
                AddArcCCW(linePositions, p1, startAngle1, endAngle1);

                float startAngle2 = Mathf.Atan2(normal.y, normal.x);
                float endAngle2 = Mathf.Atan2(-normal.y, -normal.x);
                AddArcCCW(linePositions, p2, startAngle2, endAngle2);

                linePositions.Add(linePositions[0]);
            }
            else
            {
                for (int i = 0; i < hull.Count; i++)
                {
                    Vector2 prev = hull[(i - 1 + hull.Count) % hull.Count];
                    Vector2 current = hull[i];
                    Vector2 next = hull[(i + 1) % hull.Count];

                    Vector2 dirIn = (current - prev).normalized;
                    Vector2 dirOut = (next - current).normalized;

                    Vector2 normalIn = new Vector2(dirIn.y, -dirIn.x);
                    Vector2 normalOut = new Vector2(dirOut.y, -dirOut.x);

                    float startAngle = Mathf.Atan2(normalIn.y, normalIn.x);
                    float endAngle = Mathf.Atan2(normalOut.y, normalOut.x);

                    AddArcCCW(linePositions, current, startAngle, endAngle);
                }

                if (linePositions.Count > 0)
                {
                    linePositions.Add(linePositions[0]);
                }
            }
        }

        LR.positionCount = linePositions.Count;
        for (int i = 0; i < linePositions.Count; i++)
        {
            LR.SetPosition(i, linePositions[i]);
        }
    }

    private void AddArcCCW(List<Vector3> positions, Vector2 center, float startAngle, float endAngle)
    {
        while (endAngle < startAngle)
        {
            endAngle += Mathf.PI * 2f;
        }

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float x = center.x + Mathf.Cos(angle) * radius;
            float z = center.y + Mathf.Sin(angle) * radius;
            positions.Add(new Vector3(x, targetY, z));
        }
    }

    private List<Vector2> GetConvexHull(List<Vector2> points)
    {
        int n = points.Count;
        if (n <= 1)
        {
            return new List<Vector2>(points);
        }

        points.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        List<Vector2> lower = new List<Vector2>();
        for (int i = 0; i < n; i++)
        {
            while (lower.Count >= 2 && CrossProduct(lower[lower.Count - 2], lower[lower.Count - 1], points[i]) <= 0)
            {
                lower.RemoveAt(lower.Count - 1);
            }
            lower.Add(points[i]);
        }

        List<Vector2> upper = new List<Vector2>();
        for (int i = n - 1; i >= 0; i--)
        {
            while (upper.Count >= 2 && CrossProduct(upper[upper.Count - 2], upper[upper.Count - 1], points[i]) <= 0)
            {
                upper.RemoveAt(upper.Count - 1);
            }
            upper.Add(points[i]);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);

        lower.AddRange(upper);
        return lower;
    }

    private float CrossProduct(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }
}