using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForwardShieldAbility : TimedGenericAbility, IObserver<MovementAbilityFiredMessage>, IIgnoreSpawnedColliders
{
    [SerializeField]
    protected GameObject forwardShieldPrefab;

    GameObject forwardShield;
    InputToAction action;
    FloatStat massMod;
    Collider2D shieldCol;

    List<Collider2D> ignoreCollisionList = new List<Collider2D>();

    public void ignoreColliders(IEnumerable<Collider2D> colliders)
    {
        ignoreCollisionList.AddRange(colliders);
    }

    public void ignoreCollider(Collider2D collider)
    {
        ignoreCollisionList.Add(collider);
    }

    protected override void Awake()
    {
        base.Awake();
        forwardShield = GameObject.Instantiate(forwardShieldPrefab);
        shieldCol = forwardShield.GetComponentInChildren<Collider2D>();
        forwardShield.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        action = GetComponentInParent<InputToAction>();
        forwardShield.transform.SetParent(transform.root.Find("Rotating"), false);
        GetComponentInParent<IObservable<MovementAbilityFiredMessage>>().Subscribe<MovementAbilityFiredMessage>(this);
    }

    protected override void onFire(Vector2 direction)
    {
        //this ability activates when the movement ability activates
    }

    public void Notify(MovementAbilityFiredMessage m)
    {
        base.onFire(m.direction);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        forwardShield.SetActive(true);
        foreach (Collider2D coll in ignoreCollisionList)
            Physics2D.IgnoreCollision(shieldCol, coll); //ignore collision gets wiped when the collider is deactivated
        massMod = action.mass.addModifier(99f);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        forwardShield.SetActive(false);
        action.mass.removeModifier(massMod);
        massMod = null;
    }
}

public interface IIgnoreSpawnedColliders
{
    void ignoreColliders(IEnumerable<Collider2D> colliders);

    void ignoreCollider(Collider2D collider);
}