using UnityEngine;
using System.Collections;

public class PerlinCloudOctave : CloudOctave {

    public override float[, ,] generateOctave(int width, int height)
    {
        float[, ,] result = new float[width, height, 4];

        float[,] seeds = new float[4, 4];

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
                seeds[x, y] = Random.Range(-99999, 99999);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xLerp = (float)x / width;
                float yLerp = (float)y / width;

                float xLerpShifted = Mathf.Abs(xLerp - 0.5f);
                float yLerpShifted = Mathf.Abs(yLerp - 0.5f);

                for (int c = 0; c < 4; c++)
                {
                    float tilable = 0;
                    float untilable = 0;

                    tilable += Mathf.PerlinNoise(seeds[c, 0] + frequency * xLerpShifted, seeds[c, 1] + frequency * yLerpShifted);

                    untilable += Mathf.PerlinNoise(seeds[c, 2] + frequency * xLerp, seeds[c, 3] + frequency * yLerp);

                    float value = Mathf.Lerp(tilable, untilable, Mathf.Min(2 * Mathf.Min(xLerp, 1 - xLerp), 2 * Mathf.Min(yLerp, 1 - yLerp)));

                    result[x, y, c] = value;

                }
            }
        }

        return result;
    }
}
