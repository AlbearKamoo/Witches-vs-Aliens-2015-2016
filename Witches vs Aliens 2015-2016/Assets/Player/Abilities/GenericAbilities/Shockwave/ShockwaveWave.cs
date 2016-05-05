using UnityEngine;
using System.Collections;

public class ShockwaveWave : MonoBehaviour {

    public float scale { 
        set {
            foreach(ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
                p.startSize *= value;
            transform.Find("ShockwaveDistortion").localScale *= value;
    } }


}
