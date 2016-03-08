using UnityEngine;
using System.Collections;

public class ForwardShieldAbility : TimedGenericAbility, IObserver<MovementAbilityFiredMessage>
{
    [SerializeField]
    protected GameObject forwardShieldPrefab;

    GameObject forwardShield;
    InputToAction action;
    FloatStat massMod;

    protected override void Awake()
    {
        base.Awake();
        forwardShield = GameObject.Instantiate(forwardShieldPrefab);
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
        massMod = action.mass.addModifier(99999f);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        forwardShield.SetActive(false);
        action.mass.removeModifier(massMod);
        massMod = null;
    }
}
