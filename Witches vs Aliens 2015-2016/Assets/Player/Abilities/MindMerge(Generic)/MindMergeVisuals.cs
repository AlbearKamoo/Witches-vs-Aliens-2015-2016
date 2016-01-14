using UnityEngine;
using System.Collections;

public class MindMergeVisuals : MonoBehaviour {

    [SerializeField]
    protected GameObject trailPrefab;

    [SerializeField]
    protected int numTrails;

    [SerializeField]
    protected float animationSpeed;

    public bool flowIn = false;
    public Rigidbody2D target;

    MindMergeTrail[] trails;
    float time;

	// Use this for initialization
	void Start () {
        trails = new MindMergeTrail[numTrails];
        for (int i = 0; i < numTrails; i++)
        {
            GameObject spawnedTrailPrefab = SimplePool.Spawn(trailPrefab);
            spawnedTrailPrefab.transform.SetParent(this.transform, false);
            trails[i] = spawnedTrailPrefab.GetComponent<MindMergeTrail>();
            trails[i].flowIn = flowIn;
        }
	}
	
	// Update is called once per frame
	void Update () {
        time += animationSpeed * Time.deltaTime;
        for (int i = 0; i < numTrails; i++)
        {
            trails[i].setCycleProgress(time + ((float)i / numTrails));
        }
        this.transform.rotation = (target.position - (Vector2)(this.transform.position)).ToRotation();
	}
}
