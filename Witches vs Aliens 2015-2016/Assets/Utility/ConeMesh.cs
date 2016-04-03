using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class ConeMesh : MonoBehaviour {

    [SerializeField]
    protected int numTris;

    [SerializeField]
    protected float weight;

    [SerializeField]
    protected float radius;

	// Use this for initialization
	void Awake () {
        float height = radius / weight;

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh m = filter.mesh;

        Vector3[] vertices = new Vector3[2 * (numTris + 1)];
        Vector2[] uvs = new Vector2[2 * (numTris + 1)];
        int[] tris = new int[3 * numTris];

        //apex
        vertices[0] = Vector3.zero;
        uvs[0] = Vector2.zero;

        //first vertex on the rim
        vertices[numTris] = new Vector3(radius, 0, height);
        uvs[numTris] = Vector2.right;

        tris[0] = 0;
        tris[1] = 1;
        tris[2] = numTris;
        
        

        for (int i = 1; i < numTris; i++)
        {
            float lerpValue = (float)i / numTris;
            float angle = 2 * Mathf.PI * lerpValue;

            vertices[i] = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), height);
            uvs[i] = Vector2.right;

            tris[3 * i] = 0;
            tris[3 * i + 1] = i + 1;
            tris[3 * i + 2] = i;
        }

        //apply to mesh
        m.vertices = vertices;
        m.uv = uvs;
        m.SetTriangles(tris, 0);
	}
}
