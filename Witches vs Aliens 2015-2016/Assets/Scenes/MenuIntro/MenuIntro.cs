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
            /*
            Callback.FireAndForget(() => {
                
            }, GetComponent<AudioSource>().clip.length, this);
            */
            CanvasGroup alphaControl = GetComponent<CanvasGroup>();

            Callback.FireAndForget(() =>
            {
                Callback.DoLerp((float l) => alphaControl.alpha = l * l * l, 2, this, reverse: true).FollowedBy(() =>
                {
                    Instantiate(menuMusic);
                    Destroy(this.gameObject);
                }, this);
            }, GetComponent<AudioSource>().clip.length - 2, this);
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
