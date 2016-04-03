using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(MeshFilter))]
public class ForceMeshAspect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Mesh target = GetComponent<MeshFilter>().mesh;

        float aspectRatio = (float)Screen.height / Screen.width;

        Vector2[] newUVs = target.uv;
        Vector3[] newVertices = target.vertices;

        Assert.AreEqual(newUVs.Length, newVertices.Length);

        for (int i = 0; i < newUVs.Length; i++)
        {
            float UVdistance = newUVs[i].y - 0.5f;
            UVdistance *= aspectRatio;
            newUVs[i].y = UVdistance + 0.5f;

            newVertices[i].y *= aspectRatio;
        }
        target.uv = newUVs;
        target.vertices = newVertices;
	}
}
