using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidSuper : TimedSuperAbility, IPuckAbility, IAlliesAbility, IOpponentsAbility
{
    [SerializeField]
    protected GameObject boidPrefab;

    [SerializeField]
    protected int numBoids;

    List<Transform> _opponents;
    public List<Transform> opponents { set { _opponents = value; } }

    List<Transform> _allies;
    List<Collider2D> allyColliders;
    public List<Transform> allies { set { 
        _allies = value;
        allyColliders = new List<Collider2D>();
        for (int i = 0; i < value.Count; i++)
        {
            allyColliders.AddRange(value[i].GetComponentsInChildren<Collider2D>());
        }
    } }

    Collider2D puckCollider;
    public Transform puck { set { puckCollider = value.GetComponent<Collider2D>(); } }

    List<Boid> boids = new List<Boid>();

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _allies.Count; i++)
        {
            _allies[i].gameObject.AddComponent<MobileAvoidBoid>();
        }
        puckCollider.gameObject.AddComponent<MobileAvoidBoid>();

        ready = true; //for easy testing
    }

    protected override void OnActivate()
    {
        Debug.Log("activate");
        base.OnActivate();
        for (int i = 0; i < numBoids; i++)
        {
            GameObject instantiatedBoid = Instantiate(boidPrefab);
            Boid newBoid = instantiatedBoid.GetComponent<Boid>();
            boids.Add(newBoid);

            newBoid.ignoreCollision(puckCollider);
            newBoid.ignoreCollisions(allyColliders);
        }
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        for (int i = 0; i < boids.Count; i++)
        {
            Destroy(boids[i].gameObject);
        }
        boids.Clear();
    }
}
