using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {

    public static MenuMusic singleton;

	// Use this for initialization
	void Start () {
        if (singleton != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
	}
}
