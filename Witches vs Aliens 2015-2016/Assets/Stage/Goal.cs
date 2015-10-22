using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
    [SerializeField]
    private Side mySide;
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D other) {
        if(other.collider.CompareTag(Tags.puck))
            Debug.Log("GOOOOOOOOOOOOOOOOOOAL!");
	}

    public enum Side
    {
        LEFT,
        RIGHT
    }
}

