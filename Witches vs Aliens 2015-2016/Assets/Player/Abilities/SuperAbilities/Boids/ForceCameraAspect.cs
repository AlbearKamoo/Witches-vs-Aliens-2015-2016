using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ForceCameraAspect : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GetComponent<Camera>().aspect = (Screen.width / Screen.height);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
