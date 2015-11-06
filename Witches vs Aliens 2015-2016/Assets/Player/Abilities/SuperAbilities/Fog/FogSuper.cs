using UnityEngine;
using System.Collections;

public class FogSuper : SuperAbility {

    [SerializeField]
    protected GameObject FogOverlay;

    GameObject instantiatedOverlay;

    protected override void onFire(Vector2 direction)
    {
        if (instantiatedOverlay == null)
        {
            instantiatedOverlay = SimplePool.Spawn(FogOverlay, Vector2.zero);
        }
    }
}
