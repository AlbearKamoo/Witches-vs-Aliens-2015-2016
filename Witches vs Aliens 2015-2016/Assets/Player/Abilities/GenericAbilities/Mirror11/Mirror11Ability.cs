using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mirror11Ability : TimedGenericAbility
{
    [SerializeField]
    protected GameObject mirrorPrefab;
    const float xSeperation = 25;
    const float ySeperation = 15;

    List<Mirror11> instantiatedMirrors = new List<Mirror11>();

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                Mirror11 instantiatedMirror = Instantiate(mirrorPrefab).GetComponent<Mirror11>();
                instantiatedMirror.Initialize(transform.root, new Vector2(x * xSeperation, y * ySeperation));
                instantiatedMirror.active = false;
                instantiatedMirrors.Add(instantiatedMirror);
            }
        }
    }

    void setMirrorsActive(bool active)
    {
        for (int i = 0; i < instantiatedMirrors.Count; i++)
            instantiatedMirrors[i].active = active;
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        setMirrorsActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        setMirrorsActive(false);
    }
}
