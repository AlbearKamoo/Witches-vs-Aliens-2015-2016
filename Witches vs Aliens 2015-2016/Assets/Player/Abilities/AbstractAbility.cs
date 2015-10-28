using UnityEngine;
using System.Collections;

public abstract class AbstractAbility : MonoBehaviour {

    protected float _charge = 0f; //one charge generally means one firing
    public float charge { get { return _charge; } }
    public bool ready { get { return _charge == 1; } }

    [SerializeField]
    protected float maxCharge;
    [SerializeField]
    protected float chargeTime;

    public abstract AbilityType type { get; }

    protected virtual void FixedUpdate()
    {
        if (charge < maxCharge)
        {
            _charge = Mathf.Clamp(_charge + Time.fixedDeltaTime / chargeTime, 0, maxCharge);
        }
    }

    public bool Fire(Vector2 direction)
    {
        if (_charge >= 1)
        {
            _charge -= 1;
            onFire(direction);
            return true;
        }
        else
        {
            return false; //didn't fire
        }
    }

    public virtual void StopFire() //called when key is released
    {

    }

    protected abstract void onFire(Vector2 direction);
}

public enum AbilityType
{
    MOVEMENT,
    SUPER,
    GENERIC,
}