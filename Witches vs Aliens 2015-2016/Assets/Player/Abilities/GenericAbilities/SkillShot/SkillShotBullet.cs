using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(SkillShotPayload))]
public class SkillShotBullet : MonoBehaviour {

    [SerializeField]
    protected float speed;

    SkillShotAbility source;
    public SkillShotAbility Source { get { return source; } }
    List<Collider2D> ignoreCollisionList = new List<Collider2D>();
    Rigidbody2D rigid;
    SkillShotPayload payload;

    Transform interactables;

    Side side;
    public Side Side { get { return side; } }

    bool active = true;

    public bool Active
    {
        set
        {
            interactables.gameObject.SetActive(value);
            active = value;
        }
    }

    void Awake()
    {
        interactables = transform.Find("interactables");
        rigid = GetComponent<Rigidbody2D>();
        interactables.gameObject.SetActive(false);
    }

    public void Initialize(Side side, SkillShotAbility source)
    {
        payload = GetComponent<SkillShotPayload>();
        this.side = side;
        this.source = source;
        payload.Initialize(this);
    }

    public void Fire(Vector2 origin, Vector2 direction)
    {
        Active = true;
        rigid.position = origin;
        rigid.velocity = speed * direction.normalized;
    }

    public void Reset()
    {
        payload.Reset();
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.stage))
        {
            Active = false; //despawn when we hit a wall
            return;
        }

        checkPlayer(other);
        checkPuck(other);
    }

    protected void checkPlayer(Collider2D other)
    {
        Stats stats = other.GetComponentInParent<Stats>();
        if (stats != null && stats.side != side)
        {
            payload.DeliverToPlayer(stats);
            Active = false;
        }
    }

    protected void checkPuck(Collider2D other)
    {
        PuckSpeedLimiter puck = other.GetComponentInParent<PuckSpeedLimiter>();
        if (puck != null)
        {
            payload.DeliverToPuck(puck);
            Active = false;
        }
    }
}
