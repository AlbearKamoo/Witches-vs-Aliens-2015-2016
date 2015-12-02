using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody2D))]
public class PuckSpeedLimiter : MonoBehaviour, ISpeedLimiter
{
    [SerializeField]
    protected float initialMaxSpeed;
    Rigidbody2D rigid;

    public float maxSpeed { get; set; }
    // Use this for initialization
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        maxSpeed = initialMaxSpeed;
    }

    // Update is called once per frame
    void OnCollisionEnter2D()
    {
        //limit velocity
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
    }
}

public interface ISpeedLimiter
{
    float maxSpeed { get; }
}