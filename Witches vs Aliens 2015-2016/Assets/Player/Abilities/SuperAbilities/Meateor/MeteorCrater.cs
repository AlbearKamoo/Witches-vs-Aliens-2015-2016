using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Collider2D))]
public class MeteorCrater : MonoBehaviour, ISpawnable {
    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float speedNerf;

    SpriteRenderer rend;
    ParticleSystem vfx;
    Collider2D coll;
    Side _side;
    public Side side {
        set { _side = value; }
    }

    Dictionary<InputToAction, FloatStat> modifiers = new Dictionary<InputToAction, FloatStat>();

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        vfx = GetComponent<ParticleSystem>();
        coll = GetComponent<Collider2D>();
    }

	// Use this for initialization
	public void Create () { //called on spawn
        Callback.FireAndForget(despawn, duration, this);
        vfx.Play();
        Callback.DoLerp((float l) => rend.color = rend.color.setAlphaFloat(l), 0.5f, this);
        ScreenShake.RandomShake(this, 0.05f, 0.1f);
        //vfx stuff
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
        if(other.CompareTag(Tags.player))
        {
            if (other.GetComponentInParent<Stats>().side != _side)
            {
                InputToAction otherController = other.GetComponentInParent<InputToAction>();
                if (!modifiers.ContainsKey(otherController))
                {
                    modifiers[otherController] = otherController.maxSpeed.addModifier(speedNerf);
                }
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
                modifiers.Remove(otherController);
            }
        }
    }

    void despawn()
    {
        foreach (var element in modifiers)
        {
            element.Key.maxSpeed.removeModifier(element.Value);
        }
        coll.enabled = false;
        Callback.DoLerp((float l) => rend.color = rend.color.setAlphaFloat(l), 1f, this, reverse: true).FollowedBy(() => { coll.enabled = true; SimplePool.Despawn(this.gameObject); }, this);
    }
}
