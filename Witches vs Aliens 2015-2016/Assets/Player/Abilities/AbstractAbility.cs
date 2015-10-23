using UnityEngine;
using System.Collections;

public abstract class AbstractAbility : MonoBehaviour {

    private float _charge = 0f; //one charge generally means one firing
    public float charge { get { return charge; } }
    public bool ready { get { return _charge == 1; } }

    [SerializeField]
    protected float maxCharge;
    [SerializeField]
    protected float chargeTime;

    protected virtual void FixedUpdate()
    {
        if (charge < maxCharge)
        {
            _charge = Mathf.Clamp(charge + Time.fixedDeltaTime / chargeTime, 0, maxCharge);
        }
    }

    public bool Fire()
    {
        if (_charge >= 1)
        {
            _charge -= 1;
            onFire();
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

    protected abstract void onFire();
}
//interfaces to locate abilities
public abstract class MovementAbility : AbstractAbility { }
public abstract class SuperAbility : AbstractAbility { }
public enum AbilityType
{
    MOVEMENT,
    SUPER
}