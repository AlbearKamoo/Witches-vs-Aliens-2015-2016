using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VisualAnimate))]
public class PuckFX : MonoBehaviour {
    public GameObject impactVFXPrefab;

    const float fxTime = 1f;
    const float ssfxTime = 0.05f;
    const float ssfxIntensityMultiplier = 0.000075f;

    VisualAnimate vfx;
    Rigidbody2D rigid;
	// Use this for initialization
	void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        vfx = GetComponent<VisualAnimate>();
	}

    void Start()
    {
        vfx.DoFX();
    }

    public void Respawn()
    {
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        vfx.DoFX();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        ScreenShake.RandomShake(this, ssfxTime, other.relativeVelocity.sqrMagnitude * ssfxIntensityMultiplier);
        SimplePool.Spawn(impactVFXPrefab, (Vector3)(other.contacts[0].point) + Vector3.back);
    }
}
