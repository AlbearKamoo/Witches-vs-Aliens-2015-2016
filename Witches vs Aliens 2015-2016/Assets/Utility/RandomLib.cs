using UnityEngine;
using System.Collections;

//a small class to supplement Unity's Random class

public static class RandomLib {

    public static float RandFloatRange(float midpoint, float variance)
    {
        return midpoint + (variance * Random.value);
    }

    public static T[] Shuffle<T>(this T[] originalArray)
    {
        //Fisher-Yates algorithm
        for (int i = 0; i < originalArray.Length; i++)
        {
            T temp = originalArray[i];
            int swapIndex = Random.Range(i, originalArray.Length);
            originalArray[i] = originalArray[swapIndex];
            originalArray[swapIndex] = temp;
        }
        return originalArray;
    }
    /// <summary>
    /// Uses System.DateTime.Now.Ticks to generate a seed.
    /// </summary>
    /// <returns>Returns a random seed for use in Unity's Random library.</returns>
    public static int Seed()
    {
        return (int)(System.DateTime.Now.Ticks);
    }

    /// <summary>
    /// Undo's any seeding of Unity's Random library
    /// </summary>
    public static int ReSeed()
    {
        return (UnityEngine.Random.seed = Seed());
    }
}
