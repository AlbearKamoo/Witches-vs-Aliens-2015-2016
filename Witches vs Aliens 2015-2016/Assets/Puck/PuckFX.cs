using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VisualAnimate))]
public class PuckFX : MonoBehaviour, IObserver<BumpedSideChangedMessage> {
    [SerializeField]
    protected GameObject impactVFXPrefab;

    const float fxTime = 1f;
    const float ssfxTime = 0.05f;
    const float ssfxIntensityMultiplier = 0.00025f;
    const float tearIntensityMultiplier = 0.000001f;

    VisualAnimate vfx;
    Rigidbody2D rigid;
    LastBumped bumped;
    Collider2D coll;

    [SerializeField]
    [AutoLink(parentTag = Tags.puck, parentName = "AlienSideVFX")]
    protected SpriteRenderer alienSideVFX;

    [SerializeField]
    [AutoLink(parentTag = Tags.puck, parentName = "WitchSideVFX")]
    protected GameObject witchSideVFX;

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
                witchSideVFX.SetActive(false);// = false;
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
        bumped.Subscribe(this);
	}
    public void Hide() //called after a goal is scored
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        coll.enabled = false;
    }

    public void Respawn(Vector2 position)
    {
        rigid.constraints = RigidbodyConstraints2D.None;
        transform.position = position;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        coll.enabled = true;
        vfx.DoFX();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GoalScreenTearing.self.doTear(ssfxTime, other.relativeVelocity.sqrMagnitude * tearIntensityMultiplier);
        ScreenShake.RandomShake(this, ssfxTime, other.relativeVelocity.sqrMagnitude * ssfxIntensityMultiplier);
        IIgnorePuckVFX ignore = other.collider.GetComponent<IIgnorePuckVFX>();
        if (ignore == null) //if ignore exists, then we don't spawn FX, and the ignorepuckVFX spawns them
        {
            SimplePool.Spawn(impactVFXPrefab, (Vector3)(other.contacts[0].point) + Vector3.back);
        }
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
                alienSideVFX.enabled = false;
                witchSideVFX.SetActive(true);
                break;
            case Side.RIGHT:
                alienSideVFX.enabled = true;
                witchSideVFX.SetActive(false);
                break;
        }
    }
}

//indicates that the puck should let the object spawn the VFX
public interface IIgnorePuckVFX
{
}
