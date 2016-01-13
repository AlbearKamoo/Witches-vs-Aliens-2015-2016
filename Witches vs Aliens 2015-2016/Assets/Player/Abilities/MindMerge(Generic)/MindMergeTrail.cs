using UnityEngine;
using System.Collections;

//controls and animates a single trail for the mind merge effect

[RequireComponent(typeof(LineRenderer))]
public class MindMergeTrail : MonoBehaviour {

    public float envelopeWidth;
    public float envelopeLength;
    public int numSegments;
    LineRenderer trail;
    float[] sinePoints; //compute sine once, and cache the values

    public bool flowIn { 
        get {
            return trail.material.GetFloat("_AlphaNoiseSpeed") >= 0;
        }
        set
        {
            trail.material.SetFloat("_AlphaNoiseSpeed", value ? flowSpeed : -flowSpeed);
        }
    }

    public float flowSpeed
    {
        get
        {
            return Mathf.Abs(trail.material.GetFloat("_AlphaNoiseSpeed"));
        }
    }

    const float twoPI = 2 * Mathf.PI;
    

	// Use this for initialization
	void Awake () {
        trail = GetComponent<LineRenderer>();
        sinePoints = new float[numSegments + 1]; // + 1 for the beginning, and + 1 for the end
        trail.SetVertexCount(sinePoints.Length);
        for (int i = 0; i <= numSegments; i++)
        {
            float lerp = (float)i / numSegments;
            sinePoints[i] = Mathf.Sin(Mathf.PI * lerp);
            
        }
	}

    void UpdateTrailPositions(float height)
    {
        for (int i = 0; i <= numSegments; i++)
        {
            float lerp = (float)i / numSegments;
            trail.SetPosition(i, new Vector2(lerp * envelopeLength, height * sinePoints[i]));
        }
    }
    /// <summary>
    /// Update the trail visuals to the indicated point in the animation. progress should be in the range [0, 1), with zero being the start and one being the end/loop.
    /// </summary>
    /// <param name="progress">A float in the range [0, 1), indicating the progress along the animation.</param>
    public void setCycleProgress(float progress)
    {
        float amplitude = envelopeWidth * Mathf.Sin(twoPI * progress);
        UpdateTrailPositions(amplitude);
    }
}
