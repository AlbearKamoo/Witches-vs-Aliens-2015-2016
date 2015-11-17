using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class CrowdSFX : MonoBehaviour {

    [SerializeField]
    protected AudioClip[] crowdClips;
    [SerializeField]
    protected float overlap;
    [SerializeField]
    protected float relativeVolume;
    AudioSource sfx;

	// Use this for initialization
	void Awake () {
        sfx = GetComponent<AudioSource>();
        overlap = 1 - overlap;
        StartCoroutine(loopCrowdAudio());
	}

    IEnumerator loopCrowdAudio()
    {
        while (true)
        {
            AudioClip newCrowdClip = crowdClips[Random.Range(0, crowdClips.Length)];
            sfx.PlayOneShot(newCrowdClip, relativeVolume);
            yield return new WaitForSeconds(overlap * newCrowdClip.length);
        }
    }
}
