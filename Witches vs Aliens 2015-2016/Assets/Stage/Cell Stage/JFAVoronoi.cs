using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class JFAVoronoi : MonoBehaviour {
    [SerializeField]
    protected RenderTexture VoronoiSeeds;

    [SerializeField]
    protected RenderTexture JFAVoronoiStep;

    [SerializeField]
    protected RenderTexture JFAVoronoiStep2;

    [SerializeField]
    protected Material JFAFirst;

    [SerializeField]
    protected Material JFAVoronoiPass;

    [SerializeField]
    protected Material JFAFinal;

	// Use this for initialization
    void Start()
    {
        JFAFirst = Instantiate(JFAFirst);
        JFAVoronoiPass = Instantiate(JFAVoronoiPass);
        JFAFinal = Instantiate(JFAFinal);

        Assert.IsTrue(VoronoiSeeds.width == JFAVoronoiStep.width);
        JFAFirst.SetFloat("_Width", VoronoiSeeds.width);
        JFAVoronoiPass.SetFloat("_Width", VoronoiSeeds.width);
        JFAFinal.SetFloat("_Width", VoronoiSeeds.width);

        Assert.IsTrue(VoronoiSeeds.height == JFAVoronoiStep.height);
        JFAFirst.SetFloat("_Height", VoronoiSeeds.height);
        JFAVoronoiPass.SetFloat("_Height", VoronoiSeeds.height);
        JFAFinal.SetFloat("_Height", VoronoiSeeds.height);

        JFAFirst.SetFloat("_Distance", 512);
    }


	// Update is called once per frame
	void Update () {
        JFAVoronoiPass.SetFloat("_Distance", Mathf.Pow(2, 8));
        /*
        Graphics.Blit(JFAVoronoiStep, JFAVoronoiStep2, JFAVoronoiPass);
        RenderTexture temp = JFAVoronoiStep;
        JFAVoronoiStep = JFAVoronoiStep2;
        JFAVoronoiStep2 = temp;
         * */
        /*
        for (int i = 8; i >= 0; i--)
        {
            JFAVoronoiPass.SetFloat("_Distance", Mathf.Pow(2, i));
            Graphics.Blit(JFAVoronoiStep, JFAVoronoiStep, JFAVoronoiPass);
        }

        Graphics.Blit(VoronoiSeeds, JFAFinal);
         * */
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, JFAVoronoiStep, JFAFirst);

        JFAVoronoiPass.SetFloat("_Distance", Mathf.Pow(2, 8));
        Graphics.Blit(JFAVoronoiStep, JFAVoronoiStep2, JFAVoronoiPass);

        JFAVoronoiPass.SetFloat("_Distance", Mathf.Pow(2, 7));
        Graphics.Blit(JFAVoronoiStep2, JFAVoronoiStep, JFAVoronoiPass);


        /*
        for (int i = 8; i >= 0; i-= 2)
        {
            JFAVoronoiPass.SetFloat("_Distance", Mathf.Pow(2, i));
            Graphics.Blit(JFAVoronoiStep, JFAVoronoiStep2, JFAVoronoiPass);

            JFAVoronoiPass.SetFloat("_Distance", Mathf.Pow(2, i-1));
            Graphics.Blit(JFAVoronoiStep2, JFAVoronoiStep, JFAVoronoiPass);
        }
        */
        Graphics.Blit(JFAVoronoiStep, destination);
    }
}
