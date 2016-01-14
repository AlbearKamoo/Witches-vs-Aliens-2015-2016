using UnityEngine;
using System.Collections;

public class MindMergeAbility : TimedGenericAbility, IPuckAbility
{
    [SerializeField]
    protected GameObject visualsPrefab;

    Rigidbody2D puckRigid;
    public Transform puck { set { puckRigid = value.GetComponent<Rigidbody2D>(); } }

    DistanceJoint2D joint;
    MindMergeVisuals selfVisuals;
    MindMergeVisuals otherVisuals;

    protected override void Start()
    {
        Rigidbody2D rigid = GetComponentInParent<Rigidbody2D>();
        joint = rigid.gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.connectedBody = puckRigid;
        GameObject selfSpawnedVisuals = SimplePool.Spawn(visualsPrefab);
        selfSpawnedVisuals.transform.SetParent(rigid.transform, false);
        selfVisuals = selfSpawnedVisuals.GetComponent<MindMergeVisuals>();
        selfVisuals.target = puckRigid;
        selfVisuals.flowIn = false;

        GameObject otherSpawnedVisuals = SimplePool.Spawn(visualsPrefab);
        otherSpawnedVisuals.transform.SetParent(puckRigid.transform, false);
        otherVisuals = otherSpawnedVisuals.GetComponent<MindMergeVisuals>();
        otherVisuals.target = rigid;
        otherVisuals.flowIn = true;

        selfSpawnedVisuals.SetActive(false);
        otherSpawnedVisuals.SetActive(false);

        base.Start();
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        joint.distance = Vector2.Distance(puckRigid.position, joint.transform.position);
        joint.enabled = true;
        selfVisuals.gameObject.SetActive(true);
        otherVisuals.gameObject.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        joint.enabled = false;
        selfVisuals.gameObject.SetActive(false);
        otherVisuals.gameObject.SetActive(false);
    }
}
