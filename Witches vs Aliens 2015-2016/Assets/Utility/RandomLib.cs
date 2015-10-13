using UnityEngine;
using System.Collections;

//a small class to supplement Unity's Random class

public static class RandomLib {

    public static float RandFloatRange(float midpoint, float variance)
    {
        return midpoint + (variance * Random.value);
    }
}
