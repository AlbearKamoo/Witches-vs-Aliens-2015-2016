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
            if (value)
            {
                if (!_ready)
                {
                    rend.enabled = true;
                }
            }
            else if (_ready)
            {
                rend.enabled = false;
            }
            _ready = value;
        }
    }

    SpriteRenderer rend;

    protected override void Start()
    {
        base.Start();
        
        if(transform.parent != null)
            rend = transform.parent.Find("SuperUI").GetComponent<SpriteRenderer>();
    }

    protected override void onFire(Vector2 direction)
    {
        Debug.Log("SUPER!"); //override and remove this
        ready = false;
    }
}
