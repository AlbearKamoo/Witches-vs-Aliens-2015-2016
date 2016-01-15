using UnityEngine;
using System.Collections;

public class ContagionAbility : GenericAbility {

    [SerializeField]
    protected GameObject contagionPrefab;

    [SerializeField]
    protected float duration;

    Stats stats;
    Contagion localContagion;

	// Use this for initialization
	protected override void Start () {
        stats = GetComponentInParent<Stats>();
        localContagion = AddContagion(this.transform.root.gameObject);
        base.Start();
	}

    public Contagion TryAddContagion(Transform target)
    {
        GameObject root = target.GetBaseParent().gameObject;
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
                result = AddContagion(root);
                result.active = true;
                return result;
            }
        }

        return null;
    }

    Contagion AddContagion(GameObject targetRoot)
    {
        Contagion result = targetRoot.gameObject.AddComponent<Contagion>();
        GameObject effects = SimplePool.Spawn(contagionPrefab);
        effects.transform.SetParent(targetRoot.transform, false);
        result.Initialize(duration, this, effects.GetComponent<ContagionEffects>());
        return result;
    }

    protected override void onFire(Vector2 direction)
    {
        localContagion.active = active = true;
        active = false;
    }
}
