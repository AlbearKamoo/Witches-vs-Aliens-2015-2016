using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VisualAnimate))]
public class PuckFX : MonoBehaviour, IObserver<BumpedSideChangedMessage> {
    [SerializeField]
    protected GameObject impactVFXPrefab;

    const float fxTime = 1f;
    const float ssfxTime = 0.05f;
    const float ssfxIntensityMultiplier = 0.00025f;

    VisualAnimate vfx;
    Rigidbody2D rigid;
    LastBumped bumped;
    Collider2D coll;
    [SerializeField]
    [AutoLink(parentTag = Tags.puck, parentName = "AlienSideVFX")]
    protected SpriteRenderer alienSideVFX;
    bool _perSideEffectsActive = false;
    public bool perSideEffectsActive
    {
        get
        {
            return _perSideEffectsActive;
        }
        set
        {
            if (value)
            {
                if (!_perSideEffectsActive)
                {
                    UpdateSideEffects(bumped.side);
                }
            }
            else if (_perSideEffectsActive)
            {
                //deactivate per side effects
                alienSideVFX.enabled = false;
            }
            _perSideEffectsActive = value;
        }
    }
	// Use this for initialization
	void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        vfx = GetComponent<VisualAnimate>();
        coll = GetComponent<Collider2D>();
        bumped = GetComponent<LastBumped>();
        bumped.Observable().Subscribe(this);
	}
    public void Hide() //called after a goal is scored
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        coll.enabled = false;
    }

    public void Respawn(Vector2 position)
    {
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.MovePosition(position);
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        coll.enabled = true;
        vfx.DoFX();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        ScreenShake.RandomShake(this, ssfxTime, other.relativeVelocity.sqrMagnitude * ssfxIntensityMultiplier);
        SimplePool.Spawn(impactVFXPrefab, (Vector3)(other.contacts[0].point) + Vector3.back);
    }

    public void Notify(BumpedSideChangedMessage message)
    {
        if (_perSideEffectsActive)
            UpdateSideEffects(message.side); //potential problems with multiple trues/falses instead of alternating
    }

    void UpdateSideEffects(Side side)
    {
        switch (side)
        {
            case Side.LEFT:
                alienSideVFX.enabled = true;
                break;
            case Side.RIGHT:
                alienSideVFX.enabled = false;
                break;
        }
    }
}
