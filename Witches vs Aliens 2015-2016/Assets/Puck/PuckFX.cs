using UnityEngine;
using System.Collections;

public class PuckFX : MonoBehaviour {
    const float fxTime = 1f;
    const float ssfxTime = 0.05f;
    const float ssfxIntensityMultiplier = 0.000075f;
    Material mat;
    Rigidbody2D rigid;
	// Use this for initialization
	void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        mat = GetComponentInChildren<SpriteRenderer>().material;
	}

    public void Respawn()
    {
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        Callback.DoLerp((float t) => { mat.SetFloat(Tags.ShaderParams.cutoff, t); }, fxTime, this);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Bing!");
        ScreenShake.RandomShake(this, ssfxTime, other.relativeVelocity.sqrMagnitude * ssfxIntensityMultiplier);
    }
}
