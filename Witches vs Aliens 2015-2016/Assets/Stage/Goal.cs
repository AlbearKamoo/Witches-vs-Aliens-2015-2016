using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
    [SerializeField]
    private Side mySide;

    [AutoLink(parentTag = Tags.stage, parentName = "PuckRespawnPoint")]
    public Transform respawnPoint;
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D other) {
        if (!other.collider.CompareTag(Tags.puck))
            return;
        Debug.Log("GOOOOOOOOOOOOOOOOOOAL!");
        other.transform.position = respawnPoint.position;
	}

    public enum Side
    {
        LEFT,
        RIGHT
    }
}

