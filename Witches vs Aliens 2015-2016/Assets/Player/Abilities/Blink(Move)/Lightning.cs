using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Lightning : MonoBehaviour {

    LineRenderer line;

    public float envelopeWidth;
    public float maxRiseOverRun;
    public float pointsPerDistance;
    public float duration;

	// Use this for initialization
	void Awake () {
        line = GetComponent<LineRenderer>();
        DoFX(new Vector2(10, 0));
	}
	
	// Update is called once per frame
	public void DoFX (Vector2 target) {
        createBolt(target);
        Callback.DoLerp(Decay, duration, this, reverse : true).FollowedBy(reset, this);
	}

    private void Decay(float lerp)
    {
        Color newColor = Color.white.setAlphaFloat(lerp);
        line.SetColors(newColor, newColor);
    }

    private void reset()
    {
        SimplePool.Despawn(this.gameObject);
        line.SetColors(Color.white, Color.white);
    }

    private void createBolt(Vector2 target)
    {

        Vector2 displacement = target - (Vector2)(transform.position);

        float normalizedRiseOverRun = displacement.magnitude * maxRiseOverRun;

        float[] Xpoints = new float[Mathf.Max(Mathf.FloorToInt(pointsPerDistance * displacement.magnitude) + 1, 1)]; //array containing all the points we will use; length of array is number of points, determined by distance * pointsperdistance
        float[] Ypoints = new float[Xpoints.Length];
        Xpoints[0] = 0;
        for (int i = 1; i < Xpoints.Length; i++)
        {
            Xpoints[i] = Random.value;
        }
        System.Array.Sort<float>(Xpoints);

        Ypoints[0] = 0;

        for (int i = 1; i < Ypoints.Length; i++)
        {
            float YDiff = normalizedRiseOverRun * (Xpoints[i] - Xpoints[i - 1]);
            Ypoints[i] = Random.Range(Mathf.Max(-envelopeWidth, Ypoints[i - 1] - YDiff), Mathf.Min(+envelopeWidth, Ypoints[i - 1] + YDiff));
        }

        float generalSlope = Ypoints[Ypoints.Length - 1] - Ypoints[0];
        //shift points downward or upward along the general slope of the line so that the general slope becomes zero
        for (int i = 1; i < Ypoints.Length; i++)
        {
            Ypoints[i] = Mathf.Clamp(Ypoints[i] - generalSlope * Xpoints[i], -envelopeWidth, envelopeWidth);
        }

        Vector2 normal = new Vector2(-displacement.y, displacement.x).normalized;

        //now put this data into a linerenderer
        line.SetVertexCount(Xpoints.Length);

        for (int i = 1; i < Xpoints.Length; i++)
        {
            line.SetPosition(i, Ypoints[i] * normal + Xpoints[i] * displacement);
        }
    }
}
