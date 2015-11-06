using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogSuper : SuperAbility, IAlliesAbility, IPuckAbility {

    [SerializeField]
    protected GameObject FogOverlay;

    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float easeTime;

    List<Transform> _allies;
    public List<Transform> allies { set { _allies = value; } }

    Transform _puck;
    public Transform puck { set { _puck = value; } }

    private static int[] witchPos = {Shader.PropertyToID("_WitchPos1"), Shader.PropertyToID("_WitchPos2"), Shader.PropertyToID("_WitchPos3")};
    private static int puckPos = Shader.PropertyToID("_PuckPos");

    protected override void Start()
    {
        base.Start();
        //ready = true; //for easy testing
    }

    protected override void onFire(Vector2 direction)
    {
        ready = false;
        active = true;
        GameObject spawnedOverlay = SimplePool.Spawn(FogOverlay, Vector2.zero);
        Material FogMat = spawnedOverlay.GetComponent<MeshRenderer>().material;
        float baseRangeMin = FogMat.GetFloat(Tags.ShaderParams.rangeMin);
        float baseRangeMax = FogMat.GetFloat(Tags.ShaderParams.rangeMax);

        StartCoroutine(UpdateVisibility(FogMat));

        Callback.DoLerp((float l) =>
            {
                FogMat.SetFloat(Tags.ShaderParams.rangeMin, baseRangeMin * Mathf.Pow(100, l));
                FogMat.SetFloat(Tags.ShaderParams.rangeMax, baseRangeMax * Mathf.Pow(100, l));
            }, easeTime, this, reverse: true);

        Callback.FireAndForget(() => Callback.DoLerp((float l) =>
            {
                FogMat.SetFloat(Tags.ShaderParams.rangeMin, baseRangeMin * Mathf.Pow(100, l));
                FogMat.SetFloat(Tags.ShaderParams.rangeMax, baseRangeMax * Mathf.Pow(100, l));
            }, easeTime, this).FollowedBy(() =>
                {
                    FogMat.SetFloat(Tags.ShaderParams.rangeMin, baseRangeMin);
                    FogMat.SetFloat(Tags.ShaderParams.rangeMax, baseRangeMax);
                    active = false;
                    SimplePool.Despawn(spawnedOverlay);
                }, this), duration, this);
    }

    IEnumerator UpdateVisibility(Material FogMat)
    {
        while (active)
        {
            FogMat.SetVector(puckPos, _puck.position);
            for (int i = 0; i < _allies.Count; i++)
            {
                FogMat.SetVector(witchPos[i], _allies[i].position);
            }
            yield return null;
        }
    }
}
