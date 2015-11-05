using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Collider2D))]
public class MeteorCrater : MonoBehaviour, ISpawnable {
    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float speedNerf;

    Side _side;
    public Side side {
        set { _side = value; }
    }

    Dictionary<InputToAction, FloatStat> modifiers = new Dictionary<InputToAction, FloatStat>();

	// Use this for initialization
	public void Create () { //called on spawn
        Callback.FireAndForget(() => SimplePool.Despawn(this.gameObject), duration, this);
        //vfx stuff
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
        if(other.CompareTag(Tags.player))
        {
            if (other.GetComponentInParent<Stats>().side != _side)
            {
                InputToAction otherController = other.GetComponentInParent<InputToAction>();
                modifiers[otherController] = otherController.maxSpeed.addModifier(speedNerf);
            }
        }
	}

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag(Tags.player))
        {
            InputToAction otherController = other.GetComponentInParent<InputToAction>();
            if (modifiers.ContainsKey(otherController))
            {
                otherController.maxSpeed.removeModifier(modifiers[otherController]);
            }
        }
    }
}
