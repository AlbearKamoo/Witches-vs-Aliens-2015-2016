﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Placed as a child of the registration puck, in order to handle world-space visuals for registration.
/// </summary>

//class is now legacy
/*
public class PlayerRegistrationVisuals : MonoBehaviour {
    bool active = false;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].SetActive(value);
            }
        }
    }

    GameObject[] arrows;

	// Use this for initialization
	void Start () {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        arrows = new GameObject[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            arrows[i] = renderers[i].gameObject;
            arrows[i].SetActive(active);
        }
	}
}
*/