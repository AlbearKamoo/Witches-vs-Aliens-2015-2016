using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
    [SerializeField]
    private Side mySide;

    ParticleSystem vfx;

    [AutoLink(parentTag = Tags.stage, parentName = "PuckRespawnPoint")]
    public Transform respawnPoint;

    void Start()
    {
        vfx = GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void OnCollisionEnter2D (Collision2D other) {
        if (!other.collider.CompareTag(Tags.puck))
            return;
        Debug.Log("GOOOOOOOOOOOOOOOOOOOOOOAL!");
        other.transform.position = respawnPoint.position;
        other.transform.GetComponent<PuckFX>().Respawn();
        vfx.Play();
        ScreenShake.RandomShake(this, 0.1f, 0.25f);
	}

    public enum Side
    {
        LEFT,
        RIGHT
    }
}

