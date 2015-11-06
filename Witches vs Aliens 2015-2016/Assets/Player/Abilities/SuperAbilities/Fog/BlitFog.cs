using UnityEngine;
using System.Collections;

public class BlitFog : MonoBehaviour {

    [SerializeField]
    protected Texture tex;
    float intensity = 0.5f;
    public float time;
    Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        Pause.pause();
        material = new Material(Shader.Find("Hidden/FogVisibility"));
        //material.SetTexture(Tags.ShaderParams.effectTexture, tex);
        //Callback.DoLerpRealtime((float l) => intensity = l, time, this, reverse: true)
        //    .FollowedBy(() => { intensity = 0; Pause.unPause(); Destroy(this); }, this);
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }
        //material.SetFloat(Tags.ShaderParams.cutoff, intensity);
        Graphics.Blit(source, destination, material);
    }
}
