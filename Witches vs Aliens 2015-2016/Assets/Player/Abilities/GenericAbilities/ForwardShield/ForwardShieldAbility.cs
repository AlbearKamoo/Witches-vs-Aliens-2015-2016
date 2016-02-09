using UnityEngine;
using System.Collections;

public class ForwardShieldAbility : TimedGenericAbility, IObserver<MovementAbilityFiredMessage>
{
    [SerializeField]
    protected GameObject forwardShieldPrefab;

    GameObject forwardShield;

    protected override void Awake()
    {
        base.Awake();
        forwardShield = GameObject.Instantiate(forwardShieldPrefab);
        forwardShield.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
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
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        forwardShield.SetActive(false);
    }
}
