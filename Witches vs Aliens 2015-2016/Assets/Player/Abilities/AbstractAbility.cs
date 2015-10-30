using UnityEngine;
using System.Collections;

public abstract class AbstractAbility : MonoBehaviour {

    bool _active;
    public bool active {
        get
        {
            return _active;
        }
        set
        {
            if (value)
            {
                if (!_active)
                {
                    ready = false;
                    OnActivate();
                }
            }
            else if (_active)
            {
                OnDeactivate();
            }
            _active = value;
        }
    }

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }

    public virtual bool ready { get; set; }

    [SerializeField]
    protected float cooldownTime;

    public abstract AbilityType type { get; }

    protected virtual void Start()
    {
        OnDeactivate();
    }

    public bool Fire(Vector2 direction) //not virtual to encourage you to put your stuff in OnFire
    {
        if (ready)
        {
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
        //not abstract because the default behaviour is to do nothing
    }

    protected abstract void onFire(Vector2 direction);
}

public enum AbilityType
{
    MOVEMENT,
    SUPER,
    GENERIC,
}