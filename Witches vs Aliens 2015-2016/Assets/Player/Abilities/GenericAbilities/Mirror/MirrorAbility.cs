﻿using UnityEngine;
using System.Collections;

public class MirrorAbility : TimedGenericAbility {

    [SerializeField]
    protected GameObject mirrorPrefab;

    Mirror instantiatedMirror;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        instantiatedMirror = Instantiate(mirrorPrefab).GetComponent<Mirror>();
        instantiatedMirror.Initialize(GetComponentInParent<InputToAction>());
        instantiatedMirror.active = false;
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (active)
        {
            instantiatedMirror.UpdateCollision();
        }
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        instantiatedMirror.active = true;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        instantiatedMirror.active = false;
    }
}
