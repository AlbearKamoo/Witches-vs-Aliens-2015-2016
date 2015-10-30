using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AutoRotate))]
public class AbilityUI : MonoBehaviour, IObserver<AbilityStateChangedMessage> {

    [SerializeField]
    protected ParticleSystem movementVFX;
    [SerializeField]
    protected ParticleSystem genericVFX;

    float radius;

	// Use this for initialization
	void Start () {
        transform.parent.GetComponentInChildren<MovementAbility>().Observable().Subscribe(this);
        transform.parent.GetComponentInChildren<GenericAbility>().Observable().Subscribe(this);

        radius = transform.parent.GetComponentInChildren<CircleCollider2D>().radius;

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
}
