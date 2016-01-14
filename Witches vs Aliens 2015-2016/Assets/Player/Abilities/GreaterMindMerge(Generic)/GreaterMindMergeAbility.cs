using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GreaterMindMergeAbility : TimedGenericAbility, IOpponentsAbility
{
    [SerializeField]
    protected GameObject visualsPrefab;

    OpponentJoints[] joints;
    public List<Transform> opponents { 
        set {
            if (joints != null)
            {
                //remove old joints
                for (int i = 0; i < joints.Length; i++)
                {
                    Destroy(joints[i].joint);
                    Destroy(joints[i].selfVisuals.gameObject);
                    Destroy(joints[i].otherVisuals.gameObject);
                }
            }

            joints = new OpponentJoints[value.Count];
            Rigidbody2D rigid = GetComponentInParent<Rigidbody2D>();
            GameObject rigidObject = rigid.gameObject;
            for(int i = 0; i < value.Count; i++)
            {
                GameObject selfSpawnedVisuals = SimplePool.Spawn(visualsPrefab);
                selfSpawnedVisuals.transform.SetParent(rigidObject.transform, false);
                selfSpawnedVisuals.SetActive(false);
                
                GameObject otherSpawnedVisuals = SimplePool.Spawn(visualsPrefab);
                otherSpawnedVisuals.transform.SetParent(value[i].transform, false);
                otherSpawnedVisuals.SetActive(false);

                joints[i] = new OpponentJoints(value[i].GetComponent<Rigidbody2D>(), rigidObject.AddComponent<DistanceJoint2D>(), selfSpawnedVisuals.GetComponent<MindMergeVisuals>(), otherSpawnedVisuals.GetComponent<MindMergeVisuals>());
                joints[i].joint.enabled = false;
                joints[i].joint.connectedBody = joints[i].rigid;
                joints[i].selfVisuals.target = joints[i].rigid;
                joints[i].selfVisuals.flowIn = false;
                joints[i].otherVisuals.target = rigid;
                joints[i].otherVisuals.flowIn = true;
            }
        } }

    class OpponentJoints
    {
        public readonly Rigidbody2D rigid;
        public readonly DistanceJoint2D joint;
        public readonly MindMergeVisuals selfVisuals;
        public readonly MindMergeVisuals otherVisuals;

        public bool active
        {
            set
            {
                if(value)
                    joint.distance = Vector2.Distance(joint.transform.position, rigid.position);
                joint.enabled = value;
                selfVisuals.gameObject.SetActive(value);
                otherVisuals.gameObject.SetActive(value);
            }
        }

        public OpponentJoints(Rigidbody2D rigid, DistanceJoint2D joint, MindMergeVisuals selfVisuals, MindMergeVisuals otherVisuals)
        {
            this.rigid = rigid;
            this.joint = joint;
            this.selfVisuals = selfVisuals;
            this.otherVisuals = otherVisuals;
        }
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].active = true;
        }
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].active = false;
        }
    }
}
