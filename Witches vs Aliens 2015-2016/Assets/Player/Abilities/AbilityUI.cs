using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AutoRotate))]
public class AbilityUI : MonoBehaviour, IObserver<AbilityStateChangedMessage>, IObserver<ResetMessage> {

    [SerializeField]
    protected ParticleSystem movementVFX;
    public ParticleSystem MovementVFX { set { movementVFX = value; } } //to be set before Start. (cat super)
    [SerializeField]
    protected ParticleSystem genericVFX;
    public ParticleSystem GenericVFX { set { genericVFX = value; } }

    MovementAbility move;
    public MovementAbility Move { set { move = value; } }
    GenericAbility gen;
    public GenericAbility Generic { set { gen = value; } }

    float radius;

	// Use this for initialization
	void Start () {
        if(move == null)
            move = transform.parent.GetComponentInChildren<MovementAbility>();
        move.Observable().Subscribe(this);
        if (gen == null)
            gen = transform.parent.GetComponentInChildren<GenericAbility>();
        gen.Observable().Subscribe(this);
        GetComponentInParent<IObservable<ResetMessage>>().Observable().Subscribe(this);

        radius = 1.25f * transform.parent.GetComponentInChildren<CircleCollider2D>().radius;

        movementVFX.transform.localPosition = new Vector2(radius, 0);
        genericVFX.transform.localPosition = new Vector2(-radius, 0);
	}

    public void Notify(AbilityStateChangedMessage m)
    {
        switch (m.type)
        {
            case AbilityType.MOVEMENT:
                if(m.ready)
                    movementVFX.Play();
                else
                    movementVFX.Stop();
                break;
            case AbilityType.GENERIC:
                if (m.ready)
                    genericVFX.Play();
                else
                    genericVFX.Stop();
                break;
        }
    }

    public void Notify(ResetMessage m)
    {
        if (movementVFX.isPlaying)
        {
            movementVFX.Stop();
            Callback.FireForNextFrame(() => movementVFX.Play(), this);
        }
        if (genericVFX.isPlaying)
        {
            genericVFX.Stop();
            Callback.FireForNextFrame(() => genericVFX.Play(), this);
        }
    }
}
