using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VisualAnimate))]
[RequireComponent(typeof(StatsReference))]
public class Mirror : MonoBehaviour {

    [SerializeField]
    protected bool reverseInputOnMirror;

    InputToAction mirrorTarget;
    Rigidbody2D mirrorRigidbody;
    Rigidbody2D myRigidbody;

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

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(InputToAction mirrorTarget)
    {
        this.mirrorTarget = mirrorTarget;
        mirrorRigidbody = mirrorTarget.GetComponent<Rigidbody2D>();
        visuals = Instantiate(mirrorTarget.GetComponentInChildren<AbstractPlayerVisuals>().gameObject).GetComponent<AbstractPlayerVisuals>();
        visuals.transform.SetParent(this.transform, false);
        visuals.transform.localPosition = Vector3.zero;

        GetComponent<StatsReference>().referencedStat = mirrorTarget.GetComponent<Stats>();

        UpdateMirror();
        mirrorTarget.PostFixedUpdateDelegates.Add(UpdateMirror);
        if (reverseInputOnMirror)
            mirrorTarget.PreFixedUpdateDelegates.Add(ReverseInput);
    }

    void ReverseInput()
    {
        mirrorTarget.normalizedMovementInput = flipXComponent(mirrorTarget.normalizedMovementInput);
    }

    Vector2 flipXComponent(Vector2 original)
    {
        original.x = -original.x;
        return original;
    }

    public void UpdateMirror()
    {
        myRigidbody.position = flipXComponent(mirrorRigidbody.position);
        myRigidbody.rotation = 180f + mirrorRigidbody.rotation;
        myRigidbody.velocity = flipXComponent(mirrorRigidbody.velocity);
        myRigidbody.mass = mirrorRigidbody.mass;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        UpdateCollision();
        if (other.transform.CompareTag(Tags.puck))
        {
            other.transform.GetComponent<LastBumped>().setLastBumped(mirrorTarget.transform);
        }
    }

    public void UpdateCollision()
    {
        Assert.IsTrue(mirrorRigidbody.mass == myRigidbody.mass);

        Vector2 avgPosition = (mirrorRigidbody.position + flipXComponent(myRigidbody.position)) / 2;
        Vector2 avgVelocity = (mirrorRigidbody.velocity + flipXComponent(myRigidbody.velocity)) / 2;

        mirrorRigidbody.position = avgPosition;
        myRigidbody.position = flipXComponent(avgPosition);

        mirrorRigidbody.velocity = avgVelocity;
        myRigidbody.velocity = flipXComponent(avgVelocity);
    }
}
