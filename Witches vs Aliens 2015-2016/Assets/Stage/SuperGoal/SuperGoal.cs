using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SuperGoal : MonoBehaviour {
    Collider2D coll;
    SuperGoal _mirror;
    public SuperGoal mirror { set { _mirror = value; } }
    bool _active = false;
    public bool active
    {
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
                    coll.enabled = true;
                }
            }
            else if (_active)
            {
                coll.enabled = false;
            }
            _active = value;
        }
    }

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

	// Use this for initialization
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
        if (!other.CompareTag(Tags.puck))
            return;

        other.transform.position = _mirror.transform.TransformPoint((transform.InverseTransformPoint(other.transform.position)));
        active = false;
        _mirror.active = false;
    }
}
