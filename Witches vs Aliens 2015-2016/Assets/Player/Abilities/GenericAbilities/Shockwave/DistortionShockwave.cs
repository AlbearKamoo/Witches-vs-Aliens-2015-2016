using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class DistortionShockwave : MonoBehaviour {

    public float time;
    public float initialAnnulusRadius;

    Material shockwaveDistortionMat;
	// Use this for initialization
    void Awake()
    {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        shockwaveDistortionMat = rend.material = Instantiate(rend.material);
        Callback.DoLerp((float l) =>
        {
            shockwaveDistortionMat.SetFloat("_MaxRange", l / 2);
            shockwaveDistortionMat.SetFloat("_Annulus", 1 - (l * l));
        }, time, this);
    }
}
