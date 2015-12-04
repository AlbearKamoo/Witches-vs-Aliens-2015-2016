using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class AbstractAbilityUI : MonoBehaviour, IObserver<AbilityStateChangedMessage>, IObserver<ResetMessage> {

    [SerializeField]
    protected AbilityType type;

    NotSuperAbility ability;
    float radius;

    const float radiusMultiplier = 1.25f;

    public virtual void Construct(AbilityUIConstructorInfo info)
    {
        ability = info.ability;
        ability.Observable(ability).Subscribe(this);
        GetComponentInParent<IObservable<ResetMessage>>().Observable<ResetMessage>().Subscribe(this);
        radius = radiusMultiplier * ability.transform.parent.GetComponentInChildren<CircleCollider2D>().radius;
        switch (type)
        {
            case AbilityType.MOVEMENT:
                transform.localPosition = new Vector2(radius, 0);
                break;
            case AbilityType.GENERIC:
                transform.localPosition = new Vector2(-radius, 0);
                break;
        }
    }

    public abstract void Notify(AbilityStateChangedMessage m); //update our display state

    public abstract void Notify(ResetMessage m); //when this happens, disable all distance-emission particle effects for one frame because the player is about to teleport
}

public class AbilityUIConstructorInfo
{
    public readonly NotSuperAbility ability;

    public AbilityUIConstructorInfo(NotSuperAbility ability)
    {
        this.ability = ability;
    }
}