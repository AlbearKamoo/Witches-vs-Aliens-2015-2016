using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class InputToMovement : MonoBehaviour {

    Rigidbody2D rigid;

    public Vector2 normalizedInput { get; set; }
    public float maxSpeed;
    public float accel;
    private float scaledAccel; //scales the acceleration to maxSpeed; what we actually use for stuff.
    //Don't change the rigidbody mass from 1; change accel and maxSpeed instead
	// Use this for initialization
	void Awake () {
         rigid = GetComponent<Rigidbody2D>();
         scaledAccel = maxSpeed * accel;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity = Vector2.MoveTowards(rigid.velocity, maxSpeed * normalizedInput, scaledAccel * Time.fixedDeltaTime);
	}
}
