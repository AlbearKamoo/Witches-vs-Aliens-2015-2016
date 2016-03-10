using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(MeshFilter))]
public class CircularLineRenderer : MonoBehaviour {

    [SerializeField]
    protected float innerRadius;

    [SerializeField]
    protected float outerRadius;

    [SerializeField]
    protected int numSides;

    [SerializeField]
    protected 

	// Use this for initialization
	void Awake () {
        Assert.IsTrue(outerRadius > innerRadius);
        Assert.IsTrue(innerRadius >= 0);
        Assert.IsTrue(numSides >= 3);
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh m = filter.mesh;

        Vector3[] vertices = new Vector3[2 * (numSides+1)];
        Vector2[] uvs = new Vector2[2 * (numSides+1)];
        int[] tris = new int[6 * (numSides)];

        vertices[0] = Vector2.right * outerRadius;
        uvs[0] = Vector2.up;
        vertices[1] = Vector2.right * innerRadius;
        uvs[1] = Vector2.zero;

        for (int i = 1; i <= numSides; i++)
        {
            float lerpValue = (float)i / numSides;
            float angle = 2 * Mathf.PI * lerpValue;

            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            int vertexScaledIndex = 2 * i; //two vertices for each side
            int triangleScaledIndex = 3 * vertexScaledIndex; //2 sets of three points for each side

            //outer vertex
            vertices[vertexScaledIndex] = direction * outerRadius;
            uvs[vertexScaledIndex] = new Vector2(lerpValue, 1);

            //inner vertex
            vertices[vertexScaledIndex + 1] = direction * innerRadius;
            uvs[vertexScaledIndex + 1] = new Vector2(lerpValue, 0);

            //outer triangle, with previous vertex-pair
            tris[triangleScaledIndex - 6] = vertexScaledIndex - 2;
            tris[triangleScaledIndex - 5] = vertexScaledIndex - 1;
            tris[triangleScaledIndex - 4] = vertexScaledIndex;

            //inner triangle, with previous vertex-pair
            tris[triangleScaledIndex - 3] = vertexScaledIndex + 1;
            tris[triangleScaledIndex - 2] = vertexScaledIndex;
            tris[triangleScaledIndex - 1] = vertexScaledIndex - 1;
        }

        //apply to mesh
        m.vertices =  vertices;
        m.uv = uvs;
        m.SetTriangles(tris, 0);
	}
}
