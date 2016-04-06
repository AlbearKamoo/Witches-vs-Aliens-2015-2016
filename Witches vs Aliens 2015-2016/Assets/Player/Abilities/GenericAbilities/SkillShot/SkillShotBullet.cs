using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(SkillShotPayload))]
public class SkillShotBullet : MonoBehaviour {

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float maxDuration;

    SkillShotAbility source;
    public SkillShotAbility Source { get { return source; } }
    Rigidbody2D rigid;
    SkillShotPayload payload;

    Transform interactables;

    ParticleSystem vfx;

    Side side;
    public Side Side { get { return side; } }

    bool active = true;

    float timeToLive;

    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            interactables.gameObject.SetActive(value);
            vfx.Stop();
            if (value)
            {
                vfx.Pause();
                vfx.Clear();
                Callback.FireForUpdate(() => vfx.Play(), this, mode : Callback.Mode.FIXEDUPDATE);
                timeToLive = maxDuration;
            }
            active = value;
        }
    }

    void Update()
    {
        if (active)
        {
            timeToLive -= Time.deltaTime;
            if (timeToLive < 0)
            {
                source.active = false;
            }
        }
    }

    void Awake()
    {
        interactables = transform.Find("interactables");
        vfx = transform.Find("particles").GetComponent<ParticleSystem>();
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
        rigid.position = origin;
        Active = true;
        rigid.velocity = speed * direction.normalized;
    }

    public void Reset()
    {
        payload.Reset();
        Active = false;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tags.stage))
        {
            Active = false; //despawn when we hit a wall
            Assert.IsTrue(source.active);
            source.active = false;
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
