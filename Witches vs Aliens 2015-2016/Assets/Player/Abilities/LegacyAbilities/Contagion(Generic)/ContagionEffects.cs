using UnityEngine;
using System.Collections;

public class ContagionEffects : MonoBehaviour {

    SpriteRenderer[] renderers;
    ParticleSystem particles;

    public bool active
    {
        set
        {
            this.gameObject.SetActive(value);
            if (value)
            {
                particles.Play();
            }
            else
            {
                particles.Stop();
            }
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.enabled = value;
            }
        }
    }

	// Use this for initialization
	void Awake () {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();
	}
}
