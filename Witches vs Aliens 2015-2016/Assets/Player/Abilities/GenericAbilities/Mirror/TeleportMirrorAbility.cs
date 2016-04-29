using UnityEngine;
using System.Collections;

public class TeleportMirrorAbility : GenericAbility
{
    protected Rigidbody2D myRigidbody;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        myRigidbody = GetComponentInParent<Rigidbody2D>();
    }

    protected override void onFire(Vector2 direction)
    {

        Vector3 thisPosition = myRigidbody.position;

        //do the teleport
        thisPosition.x = -(thisPosition.x);
        myRigidbody.position = thisPosition;

        active = true; //to toggle the UI and cooldown
        active = false;
    }
}
