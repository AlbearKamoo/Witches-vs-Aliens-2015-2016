using UnityEngine;
using System.Collections;

public class TeleportMirrorAbility : GenericAbility
{

    [SerializeField]
    protected GameObject mirrorPrefab;

    Mirror instantiatedMirror;
    Rigidbody2D mirrorRigidbody;
    Rigidbody2D myRigidbody;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        myRigidbody = GetComponentInParent<Rigidbody2D>();

        instantiatedMirror = Instantiate(mirrorPrefab).GetComponent<Mirror>();
        instantiatedMirror.Initialize(GetComponentInParent<InputToAction>());
        instantiatedMirror.active = false;
        mirrorRigidbody = instantiatedMirror.GetComponent<Rigidbody2D>();
    }

    protected override void onFire(Vector2 direction)
    {
        //active = true; //to toggle the UI and cooldown
        Vector3 thisPosition = this.transform.position;

        //do the teleport
        myRigidbody.position = mirrorRigidbody.position;
        mirrorRigidbody.position = thisPosition;
        instantiatedMirror.UpdateCollision();
        Debug.Log("swap");

        //active = false;
    }
}
