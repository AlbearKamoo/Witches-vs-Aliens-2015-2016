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
    protected float spawnInFXTime;

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
            if (value)
            {
                vfx.Play();
                foreach (Collider2D coll in ignoreCollisionList)
                    Physics2D.IgnoreCollision(ball, coll); //ignore collision gets wiped when the collider is deactivated
                Callback.DoLerp((float l) => { rend.material.SetFloat(Tags.ShaderParams.cutoff, l); ParticleSystem.ShapeModule shape = vfx.shape; shape.arc = l * 360; }, spawnInFXTime, this);
            }
            else
            {
                vfx.Stop();
            }
        }
    }

    PolygonCollider2D ball;
    public PolygonCollider2D Ball { get { return ball; } }

    LineRenderer rend;
    ParticleSystem vfx;
    Rigidbody2D rigid;
    List<Collider2D> ignoreCollisionList;

    const float TwoPI = 2 * Mathf.PI;

	// Use this for initialization
	void Awake () {
        ignoreCollisionList = new List<Collider2D>();

        rigid = GetComponent<Rigidbody2D>();

        vfx = GetComponent<ParticleSystem>();

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
            Vector2 direction = angle.RadToVector2();
            points[i] = innerRadius * direction;
            rend.SetPosition(i, visualRadius * direction);

        }

        // outer points
        for (int i = numSides + 1; i < points.Length; i++)
        {
            float angle = TwoPI * i / numSides;
            points[i] = outerRadius * angle.RadToVector2();
        }

        ball.points = points;
	}

    public void ignoreColliders(IEnumerable<Collider2D> colliders)
    {
        ignoreCollisionList.AddRange(colliders);
    }

    public void ignoreCollider(Collider2D collider)
    {
        ignoreCollisionList.Add(collider);
    }

    void OnCollisionEnter2D()
    {
        //limit velocity
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, initialMaxSpeed);
    }
}
