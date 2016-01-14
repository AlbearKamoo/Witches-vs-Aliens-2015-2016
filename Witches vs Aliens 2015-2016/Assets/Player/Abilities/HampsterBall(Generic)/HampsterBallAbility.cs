using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HampsterBallAbility : TimedGenericAbility, IPuckAbility, IAlliesAbility
{

    [SerializeField]
    protected GameObject hampsterBallPrefab;

    public Transform puck { set { Physics2D.IgnoreCollision(ball.Ball, value.GetComponent<Collider2D>()); } }

    HampsterBall ball;

    public List<Transform> allies
    {
        set {
            foreach (Transform ally in value)
                foreach(Collider2D coll in ally.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(ball.Ball, coll);
        }
    }

    protected void Awake()
    {
        ball = SimplePool.Spawn(hampsterBallPrefab).GetComponent<HampsterBall>();
        ball.active = false;
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        ball.transform.position = this.transform.position;
        ball.active = true;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        ball.active = false;
    }
}
