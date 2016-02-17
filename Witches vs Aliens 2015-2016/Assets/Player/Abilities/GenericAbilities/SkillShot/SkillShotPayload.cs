using UnityEngine;
using System.Collections;

public abstract class SkillShotPayload : MonoBehaviour {

    protected SkillShotBullet bullet;

    public virtual void Initialize(SkillShotBullet bullet)
    {
        this.bullet = bullet;
    }

    public abstract void Reset();

    public abstract void DeliverToPlayer(Stats target);

    public abstract void DeliverToPuck(PuckSpeedLimiter target);
}
