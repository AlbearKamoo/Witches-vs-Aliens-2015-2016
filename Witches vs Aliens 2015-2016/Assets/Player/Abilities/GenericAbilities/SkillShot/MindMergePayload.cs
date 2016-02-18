using UnityEngine;
using System.Collections;

public class MindMergePayload : SkillShotPayload {

    [SerializeField]
    protected GameObject visualsPrefab;

    [SerializeField]
    protected float duration;

    DistanceJoint2D joint;
    MindMergeVisuals selfVisuals;
    MindMergeVisuals otherVisuals;



    public override void Initialize(SkillShotBullet bullet)
    {
        base.Initialize(bullet);

        Rigidbody2D rigid = bullet.Source.GetComponentInParent<Rigidbody2D>();
        joint = rigid.gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.maxDistanceOnly = true;
        //joint.connectedBody = puckRigid;

        GameObject selfSpawnedVisuals = SimplePool.Spawn(visualsPrefab);
        selfSpawnedVisuals.transform.SetParent(rigid.transform, false);
        selfVisuals = selfSpawnedVisuals.GetComponent<MindMergeVisuals>();
        //selfVisuals.target = puckRigid;
        selfVisuals.flowIn = false;

        GameObject otherSpawnedVisuals = SimplePool.Spawn(visualsPrefab);
        otherVisuals = otherSpawnedVisuals.GetComponent<MindMergeVisuals>();
        otherVisuals.target = rigid;
        otherVisuals.flowIn = true;

        selfSpawnedVisuals.SetActive(false);
        otherSpawnedVisuals.SetActive(false);
    }

    public override void DeliverToPlayer(Stats target)
    {
        AttachToTarget(target.GetComponent<Rigidbody2D>());
    }

    public override void DeliverToPuck(PuckSpeedLimiter target)
    {
        AttachToTarget(target.GetComponent<Rigidbody2D>());
    }

    void AttachToTarget(Rigidbody2D target)
    {
        Debug.Log(target);
        joint.connectedBody = target;
        selfVisuals.target = target;
        otherVisuals.transform.SetParent(target.transform, false);

        joint.distance = Vector2.Distance(target.position, joint.transform.position);
        joint.enabled = true;
        selfVisuals.gameObject.SetActive(true);
        otherVisuals.gameObject.SetActive(true);

        Callback.FireAndForget(Reset, duration, this);
    }

    public override void Reset()
    {
        joint.enabled = false;
        selfVisuals.gameObject.SetActive(false);
        otherVisuals.gameObject.SetActive(false);
    }
}
