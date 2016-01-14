using UnityEngine;
using System.Collections;

public class ContagionAbility : GenericAbility {


    Contagion localContagion;
	// Use this for initialization
	protected override void Start () {

        base.Start();
	}

    public Contagion TryAddContagion(Transform target)
    {
        if (true)
        {
            return AddContagion(target);
        }
    }

    Contagion AddContagion(Transform target)
    {
        Transform root = target.GetBaseParent();
        return null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
