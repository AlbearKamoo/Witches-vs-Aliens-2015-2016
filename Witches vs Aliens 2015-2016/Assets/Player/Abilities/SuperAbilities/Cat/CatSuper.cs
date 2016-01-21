using UnityEngine;
using System.Collections;

public class CatSuper : SuperAbility {
    [SerializeField]
    protected float duration;

    [SerializeField]
    protected GameObject PounceAbilityprefab;
    AbstractMovementAbility defaultMove;
    AbstractMovementAbility catMove;

    [SerializeField]
    protected GameObject BatAbilityprefab;
    AbstractGenericAbility defaultGeneric;
    AbstractGenericAbility catGeneric;

    bool instantiated = false;
    InputToAction action;
    GameObject defaultAbilityUI;
    SpriteRenderer catVisuals;
    GameObject normalVisuals;

    void Awake()
    {
        catVisuals = GetComponent<SpriteRenderer>();
    }
    protected override void Start()
    {
        
        action = GetComponentInParent<InputToAction>();
        defaultAbilityUI = transform.parent.GetComponentInChildren<AbilityUIParent>().gameObject;
        normalVisuals = transform.parent.GetComponentInChildren<AbstractPlayerVisuals>().gameObject;
        ensureInstantiation();
        base.Start();
        //ready = true; //for easy testing
    }

    protected override void OnActivate()
    {
        action.MoveAbility = catMove;
        action.GenAbility = catGeneric;
        defaultAbilityUI.SetActive(false);
        normalVisuals.SetActive(false);
        catVisuals.enabled = true;
    }
    protected override void OnDeactivate()
    {
        action.MoveAbility = defaultMove;
        action.GenAbility = defaultGeneric;
        defaultAbilityUI.SetActive(true);
        catVisuals.enabled = false;
        normalVisuals.SetActive(true);
        defaultMove.ready = true;
        defaultGeneric.ready = true;
    }

    protected override void onFire(Vector2 direction)
    {
        active = true;

        Callback.FireAndForget(() =>
            {
                active = false;
            }, duration, this);

    }

    void ensureInstantiation()
    {
        if (instantiated)
            return;

        defaultMove = transform.parent.GetComponentInChildren<AbstractMovementAbility>();
        defaultGeneric = transform.parent.GetComponentInChildren<AbstractGenericAbility>();

        catMove = GameObject.Instantiate(PounceAbilityprefab).GetComponent<AbstractMovementAbility>();
        catMove.transform.SetParent(transform.parent, false);

        catGeneric = GameObject.Instantiate(BatAbilityprefab).GetComponent<AbstractGenericAbility>();
        catGeneric.transform.SetParent(transform.parent, false);

        instantiated = true;
    }
}
