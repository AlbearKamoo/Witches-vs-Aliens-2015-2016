using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

    [SerializeField]
    protected float angularSpeed;
	// Update is called once per frame

    void Start()
    {
        this.transform.Rotate(Vector3.back, Random.Range(0f, 360f)); //randomize starting position to avoid synchronization artifacts when multiple players are near each other
    }
	void Update () {
        this.transform.Rotate(Vector3.back, angularSpeed * Time.deltaTime);
	}
}
