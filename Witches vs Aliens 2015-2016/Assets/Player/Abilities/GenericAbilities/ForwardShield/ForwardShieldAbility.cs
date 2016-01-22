using UnityEngine;
using System.Collections;

public class ForwardShieldAbility : TimedGenericAbility
{
    [SerializeField]
    protected GameObject forwardShieldPrefab;

    GameObject forwardShield;

    protected override void Awake()
    {
        base.Awake();
        forwardShield = GameObject.Instantiate(forwardShieldPrefab);
        forwardShield.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        forwardShield.transform.SetParent(transform.root.Find("Rotating"), false);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        forwardShield.SetActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        forwardShield.SetActive(false);
    }
}
