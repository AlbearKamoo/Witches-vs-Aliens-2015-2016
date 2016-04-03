using UnityEngine;
using System.Collections;

public class BackgroundCells : MonoBehaviour {



    [SerializeField]
    protected GameObject cell;

    [SerializeField]
    protected float cellsPerSecond;

    [SerializeField]
    protected float maxCellSpeed;
	// Use this for initialization

    struct cellParticle
    {
        public Transform particle;
        public Vector2 start;
        public Vector2 end;
        public float lerpValue;
    }

    float duration;
    cellParticle[] particles;

	void Awake () {
        duration = 60 / maxCellSpeed;
        int numActive = Mathf.FloorToInt(duration * cellsPerSecond);
        particles = new cellParticle[numActive];
        for (int i = 0; i < numActive; i++)
        {
            particles[i].lerpValue = (float)i / (float)numActive;
            particles[i].particle = Instantiate(cell).transform;
            SpawnCell(particles, i);
        }
	}

    void Update()
    {
        float deltaLerp = Time.deltaTime / duration;
        for (int i = 0; i < particles.Length; i++)
        {

            particles[i].lerpValue += deltaLerp;
            if (particles[i].lerpValue >= 1)
            {
                particles[i].lerpValue -= 1;
                SpawnCell(particles, i);
            }

            particles[i].particle.position = Vector2.Lerp(particles[i].start, particles[i].end, particles[i].lerpValue);
        }
    }

    void SpawnCell(cellParticle[] particles, int index)
    {
        Vector2 translation = 30 * Random.insideUnitCircle.normalized;

        translation.x *= 16f / 9f;

        Vector2 centerPoint = 10 * Random.insideUnitCircle;

        centerPoint.x *= 16f / 9f;

        centerPoint -= (Vector2)(Vector3.Project(centerPoint, translation));

        particles[index].start = centerPoint - translation;
        particles[index].end = centerPoint + translation;
    }
}
