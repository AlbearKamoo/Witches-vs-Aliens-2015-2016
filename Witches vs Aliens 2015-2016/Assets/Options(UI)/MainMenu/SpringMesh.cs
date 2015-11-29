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
        nodes = new GameObject[width,height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = Instantiate(springNode);
                Vector2 position = 1.1f * scale * (new Vector2((x - width / 2) - triangleWidth * (y - height / 2), (y - height / 2) * triangleHeight));
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
        CreateMesh();
    }

    void CreateMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Color32> cols = new List<Color32>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        int t = 0;  // number of tris
        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {

                float x1 = nodes[x - 1, y].transform.position.x;
                float y1 = nodes[x, y].transform.position.y;
                float x2 = nodes[x, y].transform.position.x;
                float y2 = nodes[x, y].transform.position.y;

                Vector3 v1 = nodes[x - 1, y - 1].transform.position;//new Vector3(x1, y1, 0);
                Vector3 v2 = nodes[x - 1, y].transform.position;//new Vector3(x1, y2, 0);
                Vector3 v3 = nodes[x, y].transform.position;//new Vector3(x2, y2, 0);
                Vector3 v4 = nodes[x, y - 1].transform.position;//new Vector3(x2, y1, 0);

                verts.Add(v1);
                cols.Add(PerlinColor(seed, v1));
                verts.Add(v2);
                cols.Add(PerlinColor(seed, v2));
                verts.Add(v3);
                cols.Add(PerlinColor(seed, v3));
                verts.Add(v3);
                cols.Add(PerlinColor(seed, v3));
                verts.Add(v4);
                cols.Add(PerlinColor(seed, v4));
                verts.Add(v1);
                cols.Add(PerlinColor(seed, v1));

                // main texture uvs
                uvs.Add(new Vector2(0.0f, 0.0f));
                uvs.Add(new Vector2(0.0f, 1.0f));
                uvs.Add(new Vector2(1.0f, 1.0f));
                uvs.Add(new Vector2(1.0f, 1.0f));
                uvs.Add(new Vector2(0.0f, 1.0f));
                uvs.Add(new Vector2(0.0f, 0.0f));

                for (int i = 0; i < 6; i++)
                    tris.Add(t++);
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

    static Color PerlinColor(Vector2 seed, Vector2 pos)
    {
        return HSVColor.HSVToRGB((Mathf.PerlinNoise(seed.x + pos.x / 17, seed.y + pos.y / 17) + seed.y/ 100) % 1, 0.75f, 1);
    }
}
