using UnityEngine;
using System.Collections;

public class Mirror11Ability : TimedGenericAbility
{

    [SerializeField]
    protected GameObject mirrorPrefab;
    const float xSeperation = 25;

    Mirror11[] instantiatedMirrors;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        instantiatedMirrors = new Mirror11[2];
        instantiatedMirrors[0] = Instantiate(mirrorPrefab).GetComponent<Mirror11>();
        instantiatedMirrors[0].Initialize(transform.root, xSeperation * Vector2.right);
        instantiatedMirrors[0].active = false;

        instantiatedMirrors[1] = Instantiate(mirrorPrefab).GetComponent<Mirror11>();
        instantiatedMirrors[1].Initialize(transform.root, xSeperation * Vector2.left);
        instantiatedMirrors[1].active = false;
    }

    void setMirrorsActive(bool active)
    {
        for (int i = 0; i < instantiatedMirrors.Length; i++)
            instantiatedMirrors[i].active = active;
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        setMirrorsActive(true);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        setMirrorsActive(false);
    }
}
