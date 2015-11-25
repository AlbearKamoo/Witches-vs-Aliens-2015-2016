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
        defaultAbilityUI = transform.parent.Find("UI").gameObject;
        normalVisuals = transform.parent.GetComponentInChildren<AbstractPlayerVisuals>().gameObject;
        base.Start();
        //ready = true; //for easy testing
    }

    protected override void OnActivate()
    {
        ensureInstantiation();
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
        foreach (ParticleSystem abilityUI in defaultAbilityUI.GetComponentsInChildren<ParticleSystem>())
        {
            abilityUI.Play();
        }
        catVisuals.enabled = false;
        normalVisuals.SetActive(true);
    }

    protected override void onFire(Vector2 direction)
    {
        ready = false;
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

        defaultMove = transform.parent.GetComponentInChildren<MovementAbility>();
        defaultGeneric = transform.parent.GetComponentInChildren<GenericAbility>();

        catMove = GameObject.Instantiate(PounceAbilityprefab).GetComponent<MovementAbility>();
        catMove.transform.SetParent(transform.parent, false);

        catGeneric = GameObject.Instantiate(BatAbilityprefab).GetComponent<GenericAbility>();
        catGeneric.transform.SetParent(transform.parent, false);

        instantiated = true;
    }
}
