using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
public class ForwardShield : MonoBehaviour, IIgnorePuckVFX
{

    [SerializeField]
    protected GameObject impactVFXPrefab;

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
            SimplePool.Spawn(impactVFXPrefab, (Vector3)(other.contacts[0].point) + Vector3.back);
        }
    }
}
