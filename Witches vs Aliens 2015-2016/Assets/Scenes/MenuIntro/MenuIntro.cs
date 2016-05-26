using UnityEngine;
using System.Collections;

public class MenuIntro : MonoBehaviour {

    [SerializeField]
    protected GameObject menuMusic;

    static bool disabled = false;

	// Use this for initialization
	void Start () {
        if (disabled)
        {
            Instantiate(menuMusic);
            Destroy(this.gameObject);
        }
        else
        {
            disabled = true;
            Callback.FireAndForget(() => {
                Instantiate(menuMusic);
                Destroy(this.gameObject);
            }, 5, this);
        }
	}

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Instantiate(menuMusic);
            Destroy(this.gameObject);
        }
    }
}
