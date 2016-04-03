using UnityEngine;
using System.Collections;

public class BackgroundCells : MonoBehaviour {

    [SerializeField]
    protected GameObject cell;

    [SerializeField]
    protected float cellsPerSecond;

    [SerializeField]
    protected float cellLifetime;

    [SerializeField]
    protected float maxCellSpeed;
	// Use this for initialization
	void Start () {
	
	}

    IEnumerator spawnCells()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / cellsPerSecond);

            SimplePool.Spawn
        }
    }
}
