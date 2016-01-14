using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class HampsterBall : MonoBehaviour, ISpeedLimiter {

    [SerializeField]
    protected float outerRadius;

    [SerializeField]
    protected float innerRadius;

    [SerializeField]
    protected int numSides;

    [SerializeField]
    protected float initialMaxSpeed;
    public float maxSpeed { get { return initialMaxSpeed; } }

    [SerializeField]
    protected PhysicsMaterial2D ballPhysicsMat;

    public bool active
    {
        set
        {
            this.gameObject.SetActive(value);
        }
    }

    PolygonCollider2D ball;
    public PolygonCollider2D Ball { get { return ball; } }

    LineRenderer rend;

    Rigidbody2D rigid;

    const float TwoPI = 2 * Mathf.PI;

	// Use this for initialization
	void Awake () {
        rigid = GetComponent<Rigidbody2D>();

        ball = this.gameObject.AddComponent<PolygonCollider2D>();
        ball.sharedMaterial = ballPhysicsMat;

        rend = GetComponent<LineRenderer>();

        Vector2[] points = new Vector2[2 * (numSides + 1)];
        rend.SetVertexCount(numSides + 1);

        float visualRadius = (outerRadius + innerRadius) / 2;
        rend.SetWidth(outerRadius - innerRadius, outerRadius - innerRadius);

        // inner points
        for (int i = 0; i <= numSides; i++)
        {
            float angle = TwoPI * i / numSides;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            points[i] = innerRadius * direction;
            rend.SetPosition(i, visualRadius * direction);

        }

        // outer points
        for (int i = numSides + 1; i < points.Length; i++)
        {
            float angle = TwoPI * i / numSides;
            points[i] = new Vector2(outerRadius * Mathf.Cos(angle), outerRadius * Mathf.Sin(angle));
        }

        ball.points = points;
	}

    void OnCollisionEnter2D()
    {
        //limit velocity
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, initialMaxSpeed);
    }
}
