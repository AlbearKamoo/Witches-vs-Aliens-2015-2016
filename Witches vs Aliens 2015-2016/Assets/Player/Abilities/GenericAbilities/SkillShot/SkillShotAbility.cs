using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillShotAbility : GenericAbility
{

    [SerializeField]
    protected GameObject shotPrefab;

    SkillShotBullet instantiatedShot;

    List<Collider2D> ignoreCollisionList;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        Side side = GetComponentInParent<Stats>().side;
        instantiatedShot = Instantiate(shotPrefab).GetComponent<SkillShotBullet>();
        instantiatedShot.Initialize(side, this);
        instantiatedShot.Active = false;
    }

    protected override void onFire(Vector2 direction)
    {
        active = true;
        instantiatedShot.Fire(this.transform.position, direction);
        active = false; //toggle to trigger cooldown
    }

    protected override void Reset()
    {
        instantiatedShot.Reset();
    }
}
