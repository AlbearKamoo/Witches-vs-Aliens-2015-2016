using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class CircularWallAndRenderer : CircularWall {

    [SerializeField]
    protected float lineRendererWidth;

    LineRenderer rend;

    protected override void Awake()
    {
        rend = GetComponent<LineRenderer>();
        rend.SetVertexCount(numPoints + 1);
        rend.SetWidth(lineRendererWidth, lineRendererWidth);
        base.Awake();
    }

    protected override void processElement(int index, Vector2 direction)
    {
        base.processElement(index, direction);
        rend.SetPosition(index, center + (radius - lineRendererWidth/2) * direction);
    }
}
