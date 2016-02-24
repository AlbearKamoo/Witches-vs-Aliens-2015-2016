using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurbineSuper : TimedSuperAbility, IAlliesAbility
{

    [SerializeField]
    protected GameObject turbinePrefab;

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

    protected override void OnActivate()
    {
        base.OnActivate();
        turbine.active = true;
        turbine.ignoreCollisions(allyColliders);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        turbine.active = false;
    }
}
