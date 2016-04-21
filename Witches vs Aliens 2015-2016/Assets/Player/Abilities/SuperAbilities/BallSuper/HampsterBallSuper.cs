using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HampsterBallSuper : TimedSuperAbility, IPuckAbility, IAlliesAbility, IOpponentsAbility, IObserver<ResetMessage>
{
    [SerializeField]
    protected GameObject hampsterBallPrefab;

    HampsterBall[] balls;

    List<Transform> _opponents;
    public List<Transform> opponents { set { _opponents = value; } }

    List<Transform> _allies;
    public List<Transform> allies { set { _allies = value; } }

    Collider2D puckCollider;
    public Transform puck { set { puckCollider = value.GetComponent<Collider2D>(); } }

    protected override void Start()
    {
        base.Start();

        balls = new HampsterBall[_opponents.Count];
        for (int i = 0; i < _opponents.Count; i++)
        {
            balls[i] = SimplePool.Spawn(hampsterBallPrefab).GetComponent<HampsterBall>();

            balls[i].ignoreCollider(puckCollider);

            foreach (Transform ally in _allies)
                balls[i].ignoreColliders(ally.GetComponentsInChildren<Collider2D>());
            balls[i].active = false;
        }

        IObservable<ResetMessage> resetObservable = GetComponentInParent<IObservable<ResetMessage>>();
        if (resetObservable != null)
            resetObservable.Subscribe(this);

        ready = true; //for easy testing
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].transform.position = _opponents[i].position;
            balls[i].active = true;
        }
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].active = false;
        }
    }

    public void Notify(ResetMessage m)
    {
        if (active)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].transform.position = Vector2.zero;
            }
        }
    }
}
