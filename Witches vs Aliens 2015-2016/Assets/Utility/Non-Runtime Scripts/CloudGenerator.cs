using UnityEngine;
using System.Collections;
using System.IO;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField]
    protected Vector2 dimensions;

    [SerializeField]
    protected OctaveStruct[] octaves;

    [SerializeField]
    protected string filename = "New Cloud.png";
    // Use this for initialization
    void Start()
    {
        int width = Mathf.RoundToInt(dimensions.x);
        int height = Mathf.RoundToInt(dimensions.x);

        float max = 0;

        for (int i = 0; i < octaves.Length; i++)
        {
            max += octaves[i].weight;
        }

            if (max == 0)
            {
                max = 1;
            }

        Texture2D result = new Texture2D(width, height);

        float[, ,] resultArray = new float[width, height, 4];

        for(int x = 0; x < width; x++)
            for(int y = 0; y < width; y++)
                for(int c = 0; c < 4; c++)
                    resultArray[x, y, c] = 0;

        for(int i = 0; i < octaves.Length; i++)        
        {
            float[, ,] addition = octaves[i].octave.generateOctave(width, height);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < width; y++)
                    for (int c = 0; c < 4; c++)
                        resultArray[x, y, c] += octaves[i].weight * addition[x, y, c];

        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                for (int c = 0; c < 4; c++)
                {
                    Color resultColor = new Color(resultArray[x, y, 0] / max, resultArray[x, y, 1] / max, resultArray[x, y, 2] / max, resultArray[x, y, 3] / max);
                    result.SetPixel(x, y, resultColor);
                }
            }
        }

        result.Apply();

        byte[] bytes = result.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/ScriptingOutput/" + filename, bytes);
    }
}

[System.Serializable]
public struct OctaveStruct
{
    public CloudOctave octave;
    public float weight;
}

public abstract class CloudOctave : MonoBehaviour
{
    [SerializeField]
    protected float frequency;

    public abstract float[, ,] generateOctave(int width, int height);

}