using UnityEngine;
using System.Collections;
using System.IO;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField]
    protected Vector2 dimensions;

    [SerializeField]
    protected Vector2[] noiseScales; //scale, weight

    // Use this for initialization
    void Start()
    {
        int width = Mathf.RoundToInt(dimensions.x);
        int height = Mathf.RoundToInt(dimensions.x);

        float max = 0;
        float[] seeds = new float[8 * noiseScales.Length];
        for (int i = 0; i < noiseScales.Length; i++)
        {
            max += noiseScales[i].y;
        }

        for (int i = 0; i < seeds.Length; i++)
        {
            seeds[i] = Random.Range(-99999, 99999);
        }

            if (max == 0)
            {
                max = 1;
            }

        Texture2D result = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float xLerp = (float)x / width;
                    float yLerp = (float)y / width;

                    float OneMinXLerp = 1 - xLerp;
                    float OneMinYLerp = 1 - yLerp;

                    float valueXTilable = 0;
                    float valueYTilable = 0;

                    float valueXUntilable = 0;
                    float valueYUntilable = 0;

                    for (int i = 0; i < noiseScales.Length; i++)
                    {
                        valueXTilable += noiseScales[i].y * (xLerp + yLerp) * Mathf.PerlinNoise(seeds[8 * i + 0] + noiseScales[i].x * xLerp, seeds[8 * i + 1] + noiseScales[i].x * yLerp);
                        valueXTilable += noiseScales[i].y * (xLerp + OneMinYLerp) * Mathf.PerlinNoise(seeds[8 * i + 0] + noiseScales[i].x * xLerp, seeds[8 * i + 1] + noiseScales[i].x * OneMinYLerp);
                        valueXTilable += noiseScales[i].y * (OneMinXLerp + yLerp) * Mathf.PerlinNoise(seeds[8 * i + 0] + noiseScales[i].x * OneMinXLerp, seeds[8 * i + 1] + noiseScales[i].x * yLerp);
                        valueXTilable += noiseScales[i].y * (OneMinXLerp + OneMinYLerp) * Mathf.PerlinNoise(seeds[8 * i + 0] + noiseScales[i].x * OneMinXLerp, seeds[8 * i + 1] + noiseScales[i].x * OneMinYLerp);

                        valueYTilable += noiseScales[i].y * (xLerp + yLerp) * Mathf.PerlinNoise(seeds[8 * i + 2] + noiseScales[i].x * xLerp, seeds[8 * i + 3] + noiseScales[i].x * yLerp);
                        valueYTilable += noiseScales[i].y * (xLerp + OneMinYLerp) * Mathf.PerlinNoise(seeds[8 * i + 2] + noiseScales[i].x * xLerp, seeds[8 * i + 3] + noiseScales[i].x * OneMinYLerp);
                        valueYTilable += noiseScales[i].y * (OneMinXLerp + yLerp) * Mathf.PerlinNoise(seeds[8 * i + 2] + noiseScales[i].x * OneMinXLerp, seeds[8 * i + 3] + noiseScales[i].x * yLerp);
                        valueYTilable += noiseScales[i].y * (OneMinXLerp + OneMinYLerp) * Mathf.PerlinNoise(seeds[8 * i + 2] + noiseScales[i].x * OneMinXLerp, seeds[8 * i + 3] + noiseScales[i].x * OneMinYLerp);

                        valueXUntilable += noiseScales[i].y * Mathf.PerlinNoise(seeds[8 * i + 4] + noiseScales[i].x * xLerp, seeds[8 * i + 5] + noiseScales[i].x * yLerp);
                        valueYUntilable += noiseScales[i].y * Mathf.PerlinNoise(seeds[8 * i + 6] + noiseScales[i].x * xLerp, seeds[8 * i + 7] + noiseScales[i].x * yLerp);
                    }

                    valueXTilable /= (4 * max);
                    valueYTilable /= (4 * max);

                    valueXUntilable /= max;
                    valueYUntilable /= max;

                    float valueX = Mathf.Lerp(valueXTilable, valueXUntilable, Mathf.Min(2 * Mathf.Min(xLerp, OneMinXLerp), 2 * Mathf.Min(yLerp, OneMinYLerp)));
                    float valueY = Mathf.Lerp(valueYTilable, valueYUntilable, Mathf.Min(2 * Mathf.Min(xLerp, OneMinXLerp), 2 * Mathf.Min(yLerp, OneMinYLerp)));

                    Color newColor = new Color(valueX, valueY, 0);
                    result.SetPixel(x, y, newColor);
                }
            }
        result.Apply();

        byte[] bytes = result.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/ScriptingOutput/New Cloud.png", bytes);
    }
}
