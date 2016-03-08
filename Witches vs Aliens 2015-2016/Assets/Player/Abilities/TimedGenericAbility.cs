using UnityEngine;
using System.Collections;

public class TimedGenericAbility : AbstractGenericAbility {

    [SerializeField]
    protected float maxDuration;

    protected override void onFire(Vector2 direction)
    {
        StartCoroutine(UpdateCharge());
    }

    IEnumerator UpdateCharge()
    {
        active = true;
        float duration = 0;
        while (active)
        {
            yield return new WaitForFixedUpdate();
            duration += Time.fixedDeltaTime;
            if (duration > maxDuration)
            {
                active = false;
                yield break;
            }
        }
    }

    protected override void Reset(float timeTillActive)
    {
        active = false;
    }
}
