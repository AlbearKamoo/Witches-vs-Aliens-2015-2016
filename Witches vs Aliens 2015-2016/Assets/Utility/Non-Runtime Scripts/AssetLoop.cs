#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
public class AssetLoop : MonoBehaviour {

    const string searchPattern = "*.prefab";

	//Don't modify this method; modify selectAsset and ProcessAsset
    void Start()
    {
        string[] files = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < files.Length; i++)
        {
            if (selectAsset(files[i]))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(files[i], typeof(GameObject)) as GameObject;
                Debug.Log(go);
                ProcessAsset(go);
            }
        }
    }

    //do filtering in this method
    bool selectAsset(string assetPath)
    {
        return assetPath.EndsWith(".prefab");
    }

    //do scripting in this method
    void ProcessAsset(GameObject go)
    {
        AudioSource[] audioSources = go.GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = 0.1f;
        }
    }
}
#endif