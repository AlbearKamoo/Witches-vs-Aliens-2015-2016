using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))]
public class BoidSuper : TimedSuperAbility, IPuckAbility, IAlliesAbility, IRandomAbility
{
    [SerializeField]
    protected GameObject boidPrefab;

    [SerializeField]
    protected GameObject boidOverlay;

    [SerializeField]
    protected int numBoids;

    [SerializeField]
    protected int boidsSpawnedPerFrame;

    List<Transform> _allies;
    List<Collider2D> allyColliders;
    public List<Transform> allies { set { 
        _allies = value;
        allyColliders = new List<Collider2D>();
        for (int i = 0; i < value.Count; i++)
        {
            allyColliders.AddRange(value[i].GetComponentsInChildren<Collider2D>(true));
        }
    } }

    AudioSource sfx;
    GameObject instantiatedOverlay;

    Collider2D puckCollider;
    public Transform puck { set { puckCollider = value.GetComponent<Collider2D>(); } }

    List<Boid> boids = new List<Boid>();

    protected void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < _allies.Count; i++)
        {
            _allies[i].gameObject.AddComponent<MobileAvoidBoid>();
        }
        puckCollider.gameObject.AddComponent<MobileAvoidBoid>();

        //ready = true; //for easy testing
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        List<Collider2D> boidColliders = new List<Collider2D>();
        StartCoroutine(spawnBoidsAsync(boidColliders));
        /*
        for (int i = 0; i < numBoids; i++)
        {
            spawnBoid(boidColliders);
        }
         */
        sfx.Play();
        Assert.IsNull(instantiatedOverlay);
        instantiatedOverlay = SimplePool.Spawn(boidOverlay, Vector2.zero);
    }

    IEnumerator spawnBoidsAsync(List<Collider2D> boidColliders)
    {
        int i = 0;
        while (i < numBoids)
        {
            for (int j = 0; j < boidsSpawnedPerFrame && i < numBoids; j++)
            {
                spawnBoid(boidColliders);
                i++;
            }
            yield return null; //next frame
        }
    }

    void spawnBoid(List<Collider2D> boidColliders)
    {
        GameObject instantiatedBoid = Instantiate(boidPrefab, 5 * Random.insideUnitCircle, Quaternion.identity) as GameObject;
        Boid newBoid = instantiatedBoid.GetComponent<Boid>();
        boids.Add(newBoid);

        newBoid.ignoreCollision(puckCollider);
        newBoid.ignoreCollisions(allyColliders);
        newBoid.ignoreCollisions(boidColliders);

        boidColliders.Add(newBoid.Coll);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        for (int i = 0; i < boids.Count; i++)
        {
            Destroy(boids[i].gameObject);
        }
        boids.Clear();
        sfx.Stop();
        SimplePool.Despawn(instantiatedOverlay);
        instantiatedOverlay = null;
    }
}
