using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GreaterMindMergeAbility : GenericAbility, IOpponentsAbility
{
    [SerializeField]
    protected float maxDuration;

    OpponentJoints[] joints;
    public List<Transform> opponents { 
        set {
            if (joints != null)
            {
                //remove old joints
                for (int i = 0; i < joints.Length; i++)
                {
                    Destroy(joints[i].joint);
                }
            }

            joints = new OpponentJoints[value.Count];
            GameObject rigid = GetComponentInParent<Rigidbody2D>().gameObject;
            for(int i = 0; i < value.Count; i++)
            {
                joints[i] = new OpponentJoints(value[i].GetComponent<Rigidbody2D>(), rigid.AddComponent<DistanceJoint2D>());
                joints[i].joint.enabled = false;
                joints[i].joint.connectedBody = joints[i].rigid;
            }
        } }

    class OpponentJoints
    {
        public readonly Rigidbody2D rigid;
        public readonly DistanceJoint2D joint;
        public OpponentJoints(Rigidbody2D rigid, DistanceJoint2D joint)
        {
            this.rigid = rigid;
            this.joint = joint;
        }
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].joint.distance = Vector2.Distance(joints[i].joint.transform.position, joints[i].rigid.position);
            joints[i].joint.enabled = true;
        }
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].joint.enabled = false;
        }
    }

    protected override void onFire(Vector2 direction)
    {
        StartCoroutine(UpdateCharge());
    }

    IEnumerator UpdateCharge()
    {
        active = true;
        float duration = 0;
        while (active)
        {
            yield return new WaitForFixedUpdate();
            duration += Time.fixedDeltaTime;
            if (duration > maxDuration)
            {
                active = false;
                yield break;
            }
        }
    }
}
