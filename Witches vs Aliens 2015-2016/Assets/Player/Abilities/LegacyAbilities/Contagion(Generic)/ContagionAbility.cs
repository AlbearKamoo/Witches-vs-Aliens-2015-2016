using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContagionAbility : AbstractGenericAbility {

    [SerializeField]
    protected GameObject contagionPrefab;

    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float massNerf;

    Stats stats;
    Contagion localContagion;
    List<Contagion> contagions = new List<Contagion>();

	// Use this for initialization
	protected override void Start () {
        stats = GetComponentInParent<Stats>();
        localContagion = AddContagion(this.transform.root.gameObject);
        base.Start();
	}

    public Contagion TryAddContagion(Transform target)
    {
        GameObject root = target.root.gameObject;
        Stats targetStats = root.GetComponent<Stats>();
        if (targetStats && targetStats.side != stats.side)
        {
            Contagion result = root.GetComponent<Contagion>();
            if (result != null)
            {
                if (!result.active)
                {
                    result.active = true;
                    return result;
                }
            }
            else
            {
                result = AddContagion(root, massNerf);
                result.active = true;
                return result;
            }
        }

        return null;
    }

    Contagion AddContagion(GameObject targetRoot, float massNerf = 1)
    {
        Contagion result = targetRoot.gameObject.AddComponent<Contagion>();
        GameObject effects = SimplePool.Spawn(contagionPrefab);
        effects.transform.SetParent(targetRoot.transform, false);
        result.Initialize(duration, massNerf, this, effects.GetComponent<ContagionEffects>());
        contagions.Add(result);
        return result;
    }

    protected override void onFire(Vector2 direction)
    {
        localContagion.active = active = true;
        active = false;
    }

    protected override void Reset()
    {
        for (int i = 0; i < contagions.Count; i++)
        {
            contagions[i].active = false;
        }
    }
}
