using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolygonCollider2D))]
public class CircularWall : MonoBehaviour {

    [SerializeField]
    protected int numPoints;

    [SerializeField]
    protected Vector2 center;

    [SerializeField]
    protected float radius;

    [SerializeField]
    [CanBeDefaultOrNull]
    protected float startAngle;

    [SerializeField]
    [CanBeDefaultOrNull]
    protected float endAngle;

    PolygonCollider2D col;

    void Awake()
    {
        col = GetComponent<PolygonCollider2D>();
        Vector2[] newPoints = new Vector2[numPoints + 4];
        newPoints[0] = col.points[0];
        newPoints[1] = col.points[1];
        newPoints[2] = col.points[2];
        newPoints[newPoints.Length - 1] = col.points[col.points.Length - 1];

        int max = newPoints.Length - 4;

        for (int i = 0; i <= max; i++)
        {
            float progress = ((float) i) / ((float) max);
            float angle = Mathf.Lerp(startAngle, endAngle, progress);
            angle *= Mathf.Deg2Rad;
            newPoints[i + 3] = center + radius * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        }
        col.points = newPoints;
    }
}
