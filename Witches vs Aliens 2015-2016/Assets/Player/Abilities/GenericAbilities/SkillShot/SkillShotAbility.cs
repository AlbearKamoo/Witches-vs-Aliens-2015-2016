using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillShotAbility : GenericAbility
{

    [SerializeField]
    protected GameObject shotPrefab;

    SkillShotBullet instantiatedShot;

    List<Collider2D> ignoreCollisionList;

    protected override void Awake()
    {
        base.Awake();

        instantiatedShot = Instantiate(shotPrefab).GetComponent<SkillShotBullet>();
        instantiatedShot.Active = false;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        Side side = GetComponentInParent<Stats>().side;
        instantiatedShot.Initialize(side, this);
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
