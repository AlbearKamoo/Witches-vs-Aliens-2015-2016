using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class PuckSpeedLimiter : MonoBehaviour
{

    Rigidbody2D rigid;
    public float maxSpeed;
    // Use this for initialization
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnCollisionEnter2D()
    {
        //limit velocity
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
    }
}
