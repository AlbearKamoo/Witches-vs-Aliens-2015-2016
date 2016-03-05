using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MeteorSuper : SuperAbility, IRandomAbility
{
    [SerializeField]
    protected int numMeteors;

    [SerializeField]
    protected float rangeFromCenter;

    [SerializeField]
    protected float minDistanceBetweenImpacts;

    [SerializeField]
    protected GameObject MeteorPrefab;

    LayerMask walls;
    Side side;
    AudioSource sfx;

    protected virtual void Awake()
    {
        walls = LayerMask.GetMask(new string[] { Tags.Layers.stage });
        sfx = GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        base.Start();
        side = GetComponentInParent<Stats>().side;
        ready = true; //for easy testing
    }

    protected override void onFire(Vector2 direction)
    {
        sfx.Play();

        Vector2[] previousMeteors = new Vector2[numMeteors];
        for (int i = 0; i < numMeteors; i++)
        {
            bool notFound = true;
        Restart: while (notFound)
            {
                Vector2 potentialPos = rangeFromCenter * Random.insideUnitCircle;
                if (!Physics2D.Raycast(Vector2.zero, potentialPos, potentialPos.magnitude, walls)) //if we are inside the walls
                {
                    for(int j = 0; j < i; j++)
                    {
                        if(Vector2.Distance(potentialPos, previousMeteors[j]) < minDistanceBetweenImpacts)
                        {
                            goto Restart; //we're too close, generation failed, try again //goto for precise loop breaking (don't want to break only the innermost loop or all the way to the outermost loop, only the middle while loop)
                        }
                    }

                    //no problems, accept
                    previousMeteors[i] = potentialPos;
                    SimplePool.Spawn(MeteorPrefab, potentialPos).GetComponent<MeteorTarget>().side = side;
                    notFound = false;
                }
            }
        }

        ready = false;
    }
}
