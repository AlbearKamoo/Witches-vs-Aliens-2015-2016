using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
public class ForwardShield : MonoBehaviour {

    [SerializeField]
    protected AudioClip onHitClip;

    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PuckSpeedLimiter puck = other.transform.GetComponentInParent<PuckSpeedLimiter>();
        if (puck != null)
        {
            source.PlayOneShot(onHitClip);
        }
    }
}
