using UnityEngine;
using System.Collections;

public class PuckFX : MonoBehaviour {
    public GameObject impactVFXPrefab;

    const float fxTime = 1f;
    const float ssfxTime = 0.05f;
    const float ssfxIntensityMultiplier = 0.000075f;

    SpriteRenderer render;
    Material FXmat;
    public Material defaultSpriteShader;
    Rigidbody2D rigid;
	// Use this for initialization
	void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponentInChildren<SpriteRenderer>();
        FXmat = render.material;
        render.material = defaultSpriteShader;
	}

    public void Respawn()
    {
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        render.material = FXmat;
        Callback.DoLerp((float t) => FXmat.SetFloat(Tags.ShaderParams.cutoff, t), fxTime, this).FollowedBy(() => render.material = defaultSpriteShader, this);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        ScreenShake.RandomShake(this, ssfxTime, other.relativeVelocity.sqrMagnitude * ssfxIntensityMultiplier);
        SimplePool.Spawn(impactVFXPrefab, (Vector3)(other.contacts[0].point) + Vector3.back);
    }
}
