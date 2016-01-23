using UnityEngine;
using System.Collections;

public class Blit : MonoBehaviour
{
    [SerializeField]
    protected Material material;
    public float intensity = 0;

    void Awake()
    {
        material = Instantiate(material);
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