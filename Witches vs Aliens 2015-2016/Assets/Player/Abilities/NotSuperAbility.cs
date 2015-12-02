using UnityEngine;
using System.Collections;

public abstract class NotSuperAbility : AbstractAbility, IObservable<AbilityStateChangedMessage>
{
    [SerializeField]
    protected GameObject AbilityUIPrefab;

    Observable<AbilityStateChangedMessage> _stateChangedObservable = new Observable<AbilityStateChangedMessage>();
    public Observable<AbilityStateChangedMessage> Observable()
    {
        return _stateChangedObservable;
    }

    public override bool ready
    {
        get
        {
            return base.ready;
        }
        set
        {
            base.ready = value;
            _stateChangedObservable.Post(stateMessage());
        }
    }

    protected virtual AbilityStateChangedMessage stateMessage()
    {
        return new AbilityStateChangedMessage(ready);
    }

    protected override void Start()
    {
        base.Start();
        GameObject UI = Instantiate(AbilityUIPrefab);
        UI.transform.SetParent(transform.parent.GetComponentInChildren<AutoRotate>().transform, false);
        UI.GetComponent<AbstractAbilityUI>().Construct(constructorInfo());
        ready = true;
    }

    protected virtual AbilityUIConstructorInfo constructorInfo()
    {
        return new AbilityUIConstructorInfo(this);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        Callback.FireAndForget(() => ready = true, cooldownTime, this);
    }

    [SerializeField]
    protected float cooldownTime;
}

public class AbilityStateChangedMessage
{
    public readonly bool ready;
    public AbilityStateChangedMessage(bool ready)
    {
        this.ready = ready;
    }
}