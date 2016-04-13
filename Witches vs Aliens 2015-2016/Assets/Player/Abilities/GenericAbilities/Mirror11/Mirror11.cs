using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VisualAnimate))]
public class Mirror11 : MonoBehaviour {

    [SerializeField]
    protected GameObject physics;

    [SerializeField]
    protected float inactiveAlpha;

    AbstractPlayerVisuals visuals;

    bool _active;
    public bool active
    {
        get
        {
            return _active;
        }
        set
        {
            physics.SetActive(_active = value);
            if (_active)
            {
                visuals.alpha = 1;
            }
            else
            {
                visuals.alpha = inactiveAlpha;
            }
        }
    }

    public void Initialize(Transform root, Vector3 localPosition)
    {
        visuals = Instantiate(root.GetComponentInChildren<AbstractPlayerVisuals>().gameObject).GetComponent<AbstractPlayerVisuals>();
        visuals.transform.SetParent(this.transform, false);
        visuals.transform.localPosition = Vector3.zero;
        this.transform.SetParent(root, false);
        this.transform.localPosition = localPosition;
    }
}
