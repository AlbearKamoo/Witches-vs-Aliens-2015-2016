using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SpringMesh : MonoBehaviour {

    public GameObject springNode;
    public int height;
    public int width;
    public float anchorFrequency;
    public float anchorDampening;
    public float connectorFrequency;
    public float scale;
    const float triangleWidth = 0.5f;
    const float triangleHeight = 0.86602540378443864676372317075294f; //sqrt(3) / 2
    GameObject[,] nodes; 
	// Use this for initialization
    MeshRenderer rend;
    MeshFilter filter;
    Vector2 seed;
    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        rend.sortingLayerName = Tags.stage;
        seed = new Vector2(1000 * Random.value, 1000 * Random.value);
    }

	void Start () {
        Vector2 offset = this.transform.position;
        nodes = new GameObject[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = Instantiate(springNode);
                Vector2 position = 1.1f * scale * (new Vector2((x - width / 2) - triangleWidth * (y - height / 2), (y - height / 2) * triangleHeight));
                position += offset;
                nodes[x, y].transform.position = position;
                SpringJoint2D anchorSpring = nodes[x, y].AddComponent<SpringJoint2D>();
                anchorSpring.connectedAnchor = position;
                anchorSpring.distance = 0;
                anchorSpring.frequency = anchorFrequency;
                anchorSpring.dampingRatio = anchorDampening;

                nodes[x, y].GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle;
            }
        } 

        //connect the nodes

        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                SpringJoint2D connectorSpring = nodes[x, y].AddComponent<SpringJoint2D>();
                connectorSpring.connectedBody = nodes[x - 1, y].GetComponent<Rigidbody2D>();
                connectorSpring.frequency = connectorFrequency;
                connectorSpring.distance = scale;

                connectorSpring = nodes[x, y].AddComponent<SpringJoint2D>();
                connectorSpring.connectedBody = nodes[x, y - 1].GetComponent<Rigidbody2D>();
                connectorSpring.frequency = connectorFrequency;
                connectorSpring.distance = scale;

                connectorSpring = nodes[x, y].AddComponent<SpringJoint2D>();
                connectorSpring.connectedBody = nodes[x - 1, y - 1].GetComponent<Rigidbody2D>();
                connectorSpring.frequency = connectorFrequency;
                connectorSpring.distance = scale;
            }
        }

        CreateMesh();
	}

    void Update()
    {
        seed += new Vector2(0, Time.deltaTime);
        UpdateMesh();
    }

    void CreateMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        Vector3 offset = this.transform.position;

        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                Vector3 v1 = nodes[x - 1, y - 1].transform.position - offset;//new Vector3(x1, y1, 0);
                Vector3 v2 = nodes[x - 1, y].transform.position - offset;//new Vector3(x1, y2, 0);
                Vector3 v3 = nodes[x, y].transform.position - offset;//new Vector3(x2, y2, 0);
                Vector3 v4 = nodes[x, y - 1].transform.position - offset;//new Vector3(x2, y1, 0);

                int v = verts.Count; //future index of v1

                verts.Add(v1);
                cols.Add(PerlinColor(seed, v1));
                uvs.Add(new Vector2(0.0f, 0.0f));

                verts.Add(v2);
                cols.Add(PerlinColor(seed, v2));
                uvs.Add(new Vector2(0.0f, 1.0f));

                verts.Add(v3);
                cols.Add(PerlinColor(seed, v3));
                uvs.Add(new Vector2(1.0f, 1.0f));

                verts.Add(v4);
                cols.Add(PerlinColor(seed, v4));
                uvs.Add(new Vector2(0.0f, 1.0f));

                //upper left triangle
                tris.Add(v);
                tris.Add(v + 1);
                tris.Add(v + 2);

                //bottom right triangle
                tris.Add(v + 2);
                tris.Add(v + 3);
                tris.Add(v);
            }
        }

        Mesh m = new Mesh();
        m.vertices = verts.ToArray();
        m.colors32 = cols.ToArray();
        m.uv = uvs.ToArray();
        m.triangles = tris.ToArray();
        m.RecalculateBounds();
        filter.mesh = m;
    }

    void UpdateMesh()
    {
        Vector3[] verts = new Vector3[filter.mesh.vertexCount];
        Color32[] cols = new Color32[filter.mesh.vertexCount];
        int index = 0;

        Vector3 offset = this.transform.position;

        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                Vector3 v1 = nodes[x - 1, y - 1].transform.position - offset;//new Vector3(x1, y1, 0);
                Vector3 v2 = nodes[x - 1, y].transform.position - offset;//new Vector3(x1, y2, 0);
                Vector3 v3 = nodes[x, y].transform.position - offset;//new Vector3(x2, y2, 0);
                Vector3 v4 = nodes[x, y - 1].transform.position - offset;//new Vector3(x2, y1, 0);

                verts[index] = v1;
                cols[index++] = PerlinColor(seed, v1);

                verts[index] = v2;
                cols[index++] = PerlinColor(seed, v2);

                verts[index] = v3;
                cols[index++] = PerlinColor(seed, v3);

                verts[index] = v4;
                cols[index++] = PerlinColor(seed, v4);
            }
        }
        filter.mesh.vertices = verts;
        filter.mesh.colors32 = cols;
    }

    static Color PerlinColor(Vector2 seed, Vector2 pos)
    {
        return HSVColor.HSVToRGB((Mathf.PerlinNoise(seed.x + pos.x / 17, seed.y + pos.y / 17) + seed.y/ 100) % 1, 0.75f, 1);
    }
}
