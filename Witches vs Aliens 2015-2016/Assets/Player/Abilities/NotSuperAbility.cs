using UnityEngine;
using System.Collections;

public abstract class NotSuperAbility : AbstractAbility, IObservable<AbilityStateChangedMessage>, IObserver<ResetMessage>
{
    [SerializeField]
    protected GameObject AbilityUIPrefab;

    [SerializeField]
    public float cooldownTime;

    Observable<AbilityStateChangedMessage> _stateChangedObservable = new Observable<AbilityStateChangedMessage>();
    public Observable<AbilityStateChangedMessage> Observable(IObservable<AbilityStateChangedMessage> self)
    {
        return (Observable<AbilityStateChangedMessage>)_stateChangedObservable;
    }

    public override bool ready
    {
        set
        {
            base.ready = value;
            _stateChangedObservable.Post(stateMessage());
        }
    }

    Countdown cooldownCountdown;

    protected virtual AbilityStateChangedMessage stateMessage()
    {
        return new AbilityStateChangedMessage(ready);
    }

    protected virtual void Awake()
    {
        //cooldownCountdown = new Countdown(Cooldown, this); //moved to start
    }

    protected virtual void Start()
    {
        cooldownCountdown = new Countdown(Cooldown, this);
        GameObject UI = Instantiate(AbilityUIPrefab);
        UI.transform.SetParent(transform.root.GetComponentInChildren<AbilityUIParent>().transform, false);
        UI.GetComponent<AbstractAbilityUI>().Construct(constructorInfo());
        IObservable<ResetMessage> resetObservable = GetComponentInParent<IObservable<ResetMessage>>();
        if(resetObservable != null)
            resetObservable.Subscribe(this);
        ready = true;
    }

    protected virtual AbilityUIConstructorInfo constructorInfo()
    {
        return new AbilityUIConstructorInfo(this);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        StartCooldown();
    }

    protected virtual void StartCooldown()
    {
        cooldownCountdown.Play();
    }

    protected void SetOnCooldown()
    {
        active = false;
        ready = false;
        cooldownCountdown.Restart();
    }

    protected virtual IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        ready = true;
    }

    public void Notify(ResetMessage m)
    {
        cooldownCountdown.Stop();
        ready = true;
        Reset(m.timeTillActive);
    }

    protected abstract void Reset(float timeTillActive);
}

public class AbilityStateChangedMessage
{
    public readonly bool ready;
    public AbilityStateChangedMessage(bool ready)
    {
        this.ready = ready;
    }
}