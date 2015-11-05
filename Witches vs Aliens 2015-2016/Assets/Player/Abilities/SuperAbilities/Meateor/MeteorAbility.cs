﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MeteorAbility : SuperAbility {

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
        ready = true;
    }

    protected override void onFire(Vector2 direction)
    {
        sfx.Play();

        Vector2[] previousMeteors = new Vector2[numMeteors];
        for (int i = 0; i < numMeteors; i++)
        {
            bool notFound = true;
            while (notFound)
            {
                Vector2 potentialPos = rangeFromCenter * Random.insideUnitCircle;
                if (!Physics2D.Raycast(Vector2.zero, potentialPos, potentialPos.magnitude, walls)) //if we are inside the walls
                {
                    for(int j = 0; j < i; j++)
                    {
                        if(Vector2.Distance(potentialPos, previousMeteors[j]) < minDistanceBetweenImpacts)
                        {
                            break; //we're too close, generation failed
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
