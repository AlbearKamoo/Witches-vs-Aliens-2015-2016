using UnityEngine;
using System.Collections;

public class RotationLock : MonoBehaviour {
    Transform thisTransform;
    Transform parentTransform;
    float angle = 0;

    [SerializeField]
    protected float rotationSpeed;
	// Use this for initialization
	void Awake () {
        thisTransform = this.transform;
        parentTransform = thisTransform.parent;
	}
	
	// Update is called once per frame
	void Update () {
        angle += rotationSpeed * Time.deltaTime;
        thisTransform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
	}
}
