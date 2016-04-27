using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MainMusicSelect : MonoBehaviour {

    [SerializeField]
    protected AudioClip mainTheme;

    [SerializeField]
    protected AudioClip eightBitMainTheme;
	// Use this for initialization
	void Awake () {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (GameSelection.eightBit)
        {
            audioSource.clip = eightBitMainTheme;
        }
        else
        {
            audioSource.clip = mainTheme;
        }
        audioSource.Play();
	}
}
