using UnityEngine;
using System.Collections;

public class Blit : MonoBehaviour
{
    [SerializeField]
    protected Texture tex;
    float intensity = 0;
    public float time;
    Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        Pause.pause();
        material = new Material(Shader.Find("Hidden/Blit"));
        material.SetTexture(Tags.ShaderParams.effectTexture, tex);
        Callback.DoLerpRealtime((float l) => intensity = l, time, this, reverse: true)
            .FollowedBy(() => { intensity = 0; Pause.unPause(); Observers.Post(new InitializationMessage()); Callback.DoLerpRealtime((float l) => Time.timeScale = l, time, this)
                .FollowedBy(() => Destroy(this), this); }, this);
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetFloat(Tags.ShaderParams.cutoff, intensity);
        Graphics.Blit(source, destination, material);
    }
}

public class InitializationMessage : Message
{
    public const string constMessageType = "Initialization";
    public InitializationMessage() : base(constMessageType) {}
}