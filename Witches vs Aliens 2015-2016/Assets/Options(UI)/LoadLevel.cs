using UnityEngine;
using System.Collections;

//allows us to set up scene changes with button inspectors

public class LoadLevel : MonoBehaviour {

    public void LoadSene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}
