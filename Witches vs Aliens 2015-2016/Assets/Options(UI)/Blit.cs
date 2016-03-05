using UnityEngine;
using System.Collections;

public class Blit : MonoBehaviour
{
    [SerializeField]
    [CanBeDefaultOrNull]
    protected Material material;

    [CanBeDefaultOrNull]
    public float intensity = 0;

    [CanBeDefaultOrNull]
    public bool useIntensityInShader = true;

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
        if (useIntensityInShader)
            material.SetFloat(Tags.ShaderParams.cutoff, intensity);
        Graphics.Blit(source, destination, material);
    }
}