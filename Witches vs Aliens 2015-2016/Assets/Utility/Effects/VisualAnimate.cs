using UnityEngine;
using System.Collections;

public class VisualAnimate : MonoBehaviour {

    //places the specified FX shader on the specified target and animates it
    public float fxTime;
    public Material fxMat;
    public SpriteRenderer target;
	// Update is called once per frame
	public void DoFX () {
        Material previousMat = target.material;
        target.material = fxMat;
        Callback.DoLerp((float t) => fxMat.SetFloat(Tags.ShaderParams.cutoff, t), fxTime, this).FollowedBy(() => target.material = previousMat, this);
	}
}
