using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SkillShotAbility : GenericAbility
{

    [SerializeField]
    protected GameObject shotPrefab;

    SkillShotBullet instantiatedShot;

    AudioSource sfx;

    List<Collider2D> ignoreCollisionList;
    InputToAction action;
    FloatStat massMod;

    protected override void Awake()
    {
        base.Awake();
        sfx = GetComponent<AudioSource>();
    }
    protected override void Start()
    {
        base.Start();
        action = GetComponentInParent<InputToAction>();
        Side side = GetComponentInParent<Stats>().side;
        instantiatedShot = Instantiate(shotPrefab).GetComponent<SkillShotBullet>();
        instantiatedShot.Initialize(side, this);
        instantiatedShot.Active = false;
        active = false; //start on cooldown
    }

    protected override void onFire(Vector2 direction)
    {
        active = true;
        instantiatedShot.Fire(this.transform.position, direction);
        massMod = action.mass.addModifier(99999f);
        sfx.Play();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        action.mass.removeModifier(massMod);
        massMod = null;
    }

    protected override void Reset(float timeTillActive)
    {
        instantiatedShot.Reset();
        active = false;
        //start on cooldown
        Callback.FireAndForget(SetOnCooldown, timeTillActive, this);
        
    }
}
