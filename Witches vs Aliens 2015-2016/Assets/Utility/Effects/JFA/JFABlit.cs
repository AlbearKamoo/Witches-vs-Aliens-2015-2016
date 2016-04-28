// AlanZucconi.com: http://www.alanzucconi.com/?p=4539
using UnityEngine;
using System.Collections;

public class JFABlit : MonoBehaviour
{
    [SerializeField]
    protected Material materialFirst;
    [SerializeField]
    protected Material materialGeneral;
    [SerializeField]
    protected Material materialFinal;
    bool tested;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        RenderTexture temp1 = RenderTexture.GetTemporary(src.width, src.height, 0);
        RenderTexture temp2 = RenderTexture.GetTemporary(src.width, src.height, 0);

        temp1.filterMode = FilterMode.Point;
        temp2.filterMode = FilterMode.Point;

        materialFirst.SetFloat("_Width", src.width);
        materialGeneral.SetFloat("_Width", src.width);
        materialFirst.SetFloat("_Height", src.height);
        materialGeneral.SetFloat("_Height", src.height);

        materialFirst.SetFloat("_Distance", 1024);
        Graphics.Blit(src, temp1, materialFirst);


        materialGeneral.SetFloat("_Distance", 512);
        Graphics.Blit(temp1, temp2, materialGeneral);
        materialGeneral.SetFloat("_Distance", 256);
        Graphics.Blit(temp1, temp2, materialGeneral);


        materialGeneral.SetFloat("_Distance", 128);
        Graphics.Blit(temp2, temp1, materialGeneral);
        materialGeneral.SetFloat("_Distance", 64);
        Graphics.Blit(temp1, temp2, materialGeneral);

        
        materialGeneral.SetFloat("_Distance", 32);
        Graphics.Blit(temp2, temp1, materialGeneral);
        materialGeneral.SetFloat("_Distance", 16);
        Graphics.Blit(temp1, temp2, materialGeneral);


        materialGeneral.SetFloat("_Distance", 8);
        Graphics.Blit(temp2, temp1, materialGeneral);
        materialGeneral.SetFloat("_Distance", 4);
        Graphics.Blit(temp1, temp2, materialGeneral);


        materialGeneral.SetFloat("_Distance", 2);
        Graphics.Blit(temp2, temp1, materialGeneral);
        materialGeneral.SetFloat("_Distance", 1);
        Graphics.Blit(temp1, temp2, materialGeneral);
        
        materialFinal.SetTexture("_JFATex", temp2);
        Graphics.Blit(src, dst, materialFinal);

        RenderTexture.ReleaseTemporary(temp1);
        RenderTexture.ReleaseTemporary(temp2);
    }
}
