using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class StunTeleportAbility : TeleportMirrorAbility {

    [SerializeField]
    protected GameObject hitVisualsPrefab;
    [SerializeField]
    protected float radius;
    [SerializeField]
    protected float stunTime;
    Stats myStats;
    List<GameObject> hitVisuals = new List<GameObject>();
    Countdown resetVisualsCountdown;
    MeshRenderer rend;
    AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        resetVisualsCountdown = new Countdown(() => Callback.Routines.FireAndForgetRoutine(clearHitVisuals, stunTime, this), this);
        rend = GetComponent<MeshRenderer>();
        rend.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        myStats = GetComponentInParent<Stats>();
    }

    protected override void onFire(Vector2 direction)
    {
        base.onFire(direction);

        audioSource.Play();

        foreach (Collider2D coll in Physics2D.OverlapCircleAll(myRigidbody.position, radius))
        {
            hitTarget(coll.transform.root, stunTime);
        }
        resetVisualsCountdown.Play();
        rend.enabled = true;
        Callback.FireAndForget(() => rend.enabled = false, 1, this);
    }

    bool hitTarget(Transform hit, float stunTime)
    {
        InputToAction input = hit.GetComponent<InputToAction>();
        if (input != null)
        {
            Stats otherStats = hit.GetComponent<Stats>();
            if (otherStats.side != myStats.side)
            {
                input.DisableMovement(stunTime);
                GameObject visuals = SimplePool.Spawn(hitVisualsPrefab);
                visuals.transform.SetParent(hit);
                visuals.transform.localPosition = Vector3.zero;
                hitVisuals.Add(visuals);
                return true;
            }
        }
        return false;
    }

    void clearHitVisuals()
    {
        for (int i = 0; i < hitVisuals.Count; i++)
        {
            SimplePool.Despawn(hitVisuals[i]);
        }
        hitVisuals.Clear();
    }
}
