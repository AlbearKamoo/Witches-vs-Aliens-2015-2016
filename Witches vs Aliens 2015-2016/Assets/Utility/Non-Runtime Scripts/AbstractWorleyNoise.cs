using UnityEngine;
using System.Collections;

public class AbstractWorleyNoise : CloudOctave
{

    public override float[, ,] generateOctave(int width, int height)
    {
        Vector2[] seeds = new Vector2[Mathf.RoundToInt(frequency)];
        return null; //TODO
    }
}
