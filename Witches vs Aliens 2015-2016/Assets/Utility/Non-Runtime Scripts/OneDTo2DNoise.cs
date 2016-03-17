using UnityEngine;
using System.Collections;
using System.IO;

public class OneDTo2DNoise : MonoBehaviour {

    [SerializeField]
    protected Texture2D OneDNoiseTexture;

	// Use this for initialization
	void Start () {
        byte[] bytes = OneDNoiseTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/ScriptingOutput/New 2DNoise.png", bytes);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
