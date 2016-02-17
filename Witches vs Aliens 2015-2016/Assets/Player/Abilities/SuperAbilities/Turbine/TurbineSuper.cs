using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurbineSuper : SuperAbility, IAlliesAbility
{

    [SerializeField]
    protected GameObject turbinePrefab;

    [SerializeField]
    protected float duration;

    List<Collider2D> allyColliders;
    public List<Transform> allies 
    { 
        set
        {
            allyColliders = new List<Collider2D>();
            for (int i = 0; i < value.Count; i++)
            {
                allyColliders.AddRange(value[i].GetComponentsInChildren<Collider2D>());
            }
        }
    }

    Turbine turbine;

    protected override void Start()
    {
        turbine = Instantiate(turbinePrefab).GetComponent<Turbine>();
        base.Start();
        ready = true; //for easy testing
        turbine.active = false;
    }

    protected override void onFire(Vector2 direction)
    {
        ready = false;

        turbine.active = true;

        turbine.ignoreCollisions(allyColliders);

        Callback.FireAndForget(() => turbine.active = false, duration, this);
    }
}
