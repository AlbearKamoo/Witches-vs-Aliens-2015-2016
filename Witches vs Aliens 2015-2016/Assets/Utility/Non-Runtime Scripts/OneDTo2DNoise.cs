using UnityEngine;
using System.Collections;
using System.IO;

public class OneDTo2DNoise : MonoBehaviour {

    [SerializeField]
    protected Texture2D OneDNoiseTexture;

	// Use this for initialization
	void Start () {
        for (int x = 0; x < OneDNoiseTexture.width; x++)
        {
            for (int y = 0; y < OneDNoiseTexture.height; y++)
            {
                Color originalColor = OneDNoiseTexture.GetPixel(x, y);
                Color rotatedColor = OneDNoiseTexture.GetPixel(y, 1-x);
                Color newColor = new Color(originalColor.r, rotatedColor.r, 0);
                OneDNoiseTexture.SetPixel(x, y, newColor);
            }
        }
        OneDNoiseTexture.Apply();

        byte[] bytes = OneDNoiseTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/ScriptingOutput/New 2DNoise.png", bytes);
	}
}
