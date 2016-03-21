using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VisualAnimate))]
[RequireComponent(typeof(StatsReference))]
public class Mirror : MonoBehaviour {

    [SerializeField]
    public bool reverseInputOnMirror;

    InputToAction mirrorTarget;
    Rigidbody2D mirrorRigidbody;
    Rigidbody2D myRigidbody;

    VisualAnimate vfx;

    bool _active;
    public bool active
    {
        get
        {
            return _active;
        }
        set
        {
            this.gameObject.SetActive(_active = value);
            if (_active)
            {
                UpdateMirror();
                //vfx.DoFX();
                mirrorTarget.PostFixedUpdateDelegates.Add(UpdateMirror);
                if(reverseInputOnMirror)
                    mirrorTarget.PreFixedUpdateDelegates.Add(ReverseInput);
            }
            else
            {
                mirrorTarget.PostFixedUpdateDelegates.Remove(UpdateMirror);
                if (reverseInputOnMirror)
                    mirrorTarget.PreFixedUpdateDelegates.Remove(ReverseInput);
            }
        }
    }

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        vfx = GetComponent<VisualAnimate>();
    }

    public void Initialize(InputToAction mirrorTarget)
    {
        this.mirrorTarget = mirrorTarget;
        mirrorRigidbody = mirrorTarget.GetComponent<Rigidbody2D>();
        GameObject visuals = Instantiate(mirrorTarget.GetComponentInChildren<AbstractPlayerVisuals>().gameObject);
        visuals.transform.SetParent(this.transform, false);
        visuals.transform.localPosition = Vector3.zero;

        GetComponent<StatsReference>().referencedStat = mirrorTarget.GetComponent<Stats>();
    }

    void ReverseInput()
    {
        mirrorTarget.normalizedMovementInput = -mirrorTarget.normalizedMovementInput;
    }

    public void UpdateMirror()
    {
        myRigidbody.position = -mirrorRigidbody.position;
        myRigidbody.rotation = 180f + mirrorRigidbody.rotation;
        myRigidbody.velocity = -mirrorRigidbody.velocity;
        myRigidbody.mass = mirrorRigidbody.mass;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        UpdateCollision();
    }

    public void UpdateCollision()
    {
        Assert.IsTrue(mirrorRigidbody.mass == myRigidbody.mass);

        Vector2 avgPosition = (mirrorRigidbody.position - myRigidbody.position) / 2;
        Vector2 avgVelocity = (mirrorRigidbody.velocity - myRigidbody.velocity) / 2;

        mirrorRigidbody.position = avgPosition;
        myRigidbody.position = -avgPosition;

        mirrorRigidbody.velocity = avgVelocity;
        myRigidbody.velocity = -avgVelocity;
    }
}
