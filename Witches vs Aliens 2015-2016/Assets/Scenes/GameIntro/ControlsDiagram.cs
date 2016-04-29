using UnityEngine;
using System.Collections;

public class ControlsDiagram : MonoBehaviour {

    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float fadeTime;
    Material myMat;

	// Use this for initialization
	void Start () {
        Renderer rend = GetComponent<Renderer>();
        myMat = rend.material = Instantiate(rend.material);

        Callback.FireAndForget(() =>
        {
            Callback.DoLerp((float l) =>
            {
                myMat.SetFloat("_Cutoff", l);
            }, fadeTime, this).FollowedBy(() => Destroy(this.gameObject), this);
        }, duration, this);
	}
}
