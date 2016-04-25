// AlanZucconi.com: http://www.alanzucconi.com/?p=4539
using UnityEngine;
using System.Collections;

public class BoidsBlit : MonoBehaviour
{
    [SerializeField]
    protected RenderTexture PreviousFrameBuffer;
    [SerializeField]
    protected Material material;
    bool tested;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (!tested)
        {
            tested = true;
            Graphics.Blit(src, PreviousFrameBuffer);
        }

        RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height, 0);

        material.SetTexture("_PrevTex", PreviousFrameBuffer);
        Graphics.Blit(src, temp, material);
        Graphics.Blit(temp, PreviousFrameBuffer);
        Graphics.Blit(temp, dst);

        RenderTexture.ReleaseTemporary(temp);
    }
}
