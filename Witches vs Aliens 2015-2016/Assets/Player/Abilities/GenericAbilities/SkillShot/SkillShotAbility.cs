using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillShotAbility : GenericAbility
{

    [SerializeField]
    protected GameObject shotPrefab;

    SkillShotBullet instantiatedShot;

    List<Collider2D> ignoreCollisionList;
    InputToAction action;
    FloatStat massMod;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        action = GetComponentInParent<InputToAction>();
        Side side = GetComponentInParent<Stats>().side;
        instantiatedShot = Instantiate(shotPrefab).GetComponent<SkillShotBullet>();
        instantiatedShot.Initialize(side, this);
        instantiatedShot.Active = false;

    }

    protected override void onFire(Vector2 direction)
    {
        active = true;
        instantiatedShot.Fire(this.transform.position, direction);
        massMod = action.mass.addModifier(99999f);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        action.mass.removeModifier(massMod);
        massMod = null;
    }

    protected override void Reset()
    {
        instantiatedShot.Reset();
    }
}
