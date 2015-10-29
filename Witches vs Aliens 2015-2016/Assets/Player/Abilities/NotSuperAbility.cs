using UnityEngine;
using System.Collections;

public abstract class NotSuperAbility : AbstractAbility {

    Observable<AbilityStateChangedMessage> _stateChangedObservable = new Observable<AbilityStateChangedMessage>();
    public Observable<AbilityStateChangedMessage> stateChangedObservable
    {
        get { return _stateChangedObservable; }
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
            stateChangedObservable.Post(new AbilityStateChangedMessage(value));
        }
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        Callback.FireAndForget(() => ready = true, cooldownTime, this);
    }
}
