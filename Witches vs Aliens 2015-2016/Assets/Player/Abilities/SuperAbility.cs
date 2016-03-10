using UnityEngine;
using System.Collections;

public class SuperAbility : AbstractAbility
{
    public override AbilityType type { get { return AbilityType.SUPER; } }
    bool _ready = false;
    public override bool ready {
        get { return _ready; }
        set
        {
            visuals.SetActive(value);
            _ready = value;
        }
    }

    GameObject visuals;

    protected virtual void Start()
    {
        if(transform.parent != null)
            visuals = transform.parent.Find("SuperUI").gameObject;
    }

    protected override void onFire(Vector2 direction)
    {
        Debug.Log("SUPER!"); //override and remove this
        ready = false;
    }
}
