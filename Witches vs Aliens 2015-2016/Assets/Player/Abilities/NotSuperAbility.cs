using UnityEngine;
using System.Collections;

public abstract class NotSuperAbility : AbstractAbility, IObservable<AbilityStateChangedMessage>
{

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
            _stateChangedObservable.Post(new AbilityStateChangedMessage(value, type));
        }
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
    public readonly AbilityType type;
    public AbilityStateChangedMessage(bool ready, AbilityType type)
    {
        this.ready = ready;
        this.type = type;
    }
}