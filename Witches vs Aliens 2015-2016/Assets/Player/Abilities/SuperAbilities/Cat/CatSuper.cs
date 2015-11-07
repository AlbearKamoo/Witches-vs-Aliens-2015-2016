using UnityEngine;
using System.Collections;

public class CatSuper : SuperAbility {
    [SerializeField]
    protected float duration;

    [SerializeField]
    protected GameObject PounceAbilityprefab;
    MovementAbility defaultMove;
    MovementAbility catMove;

    [SerializeField]
    protected GameObject BatAbilityprefab;
    GenericAbility defaultGeneric;
    GenericAbility catGeneric;

    bool instantiated = false;
    InputToAction action;

    protected override void Start()
    {
        base.Start();
        action = GetComponentInParent<InputToAction>();
        ready = true; //for easy testing
    }

    protected override void onFire(Vector2 direction)
    {
        ready = false;
        ensureInstantiation();

        action.MoveAbility = catMove;
        action.GenAbility = catGeneric;

        Callback.FireAndForget(() =>
            {
                action.MoveAbility = defaultMove;
                action.GenAbility = defaultGeneric;
            }, duration, this);

    }

    void ensureInstantiation()
    {
        if (instantiated)
            return;

        defaultMove = transform.parent.GetComponentInChildren<MovementAbility>();
        defaultGeneric = transform.parent.GetComponentInChildren<GenericAbility>();

        catMove = GameObject.Instantiate(PounceAbilityprefab).GetComponent<MovementAbility>();
        catMove.transform.SetParent(transform.parent, false);

        catGeneric = GameObject.Instantiate(BatAbilityprefab).GetComponent<GenericAbility>();
        catGeneric.transform.SetParent(transform.parent, false);

        instantiated = true;
    }
}
