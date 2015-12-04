using UnityEngine;
using System.Collections;

public class RandomizePitchAudioAction : MonoBehaviour, AudioAction {
    [SerializeField]
    protected float minPitch;

    [SerializeField]
    protected float maxPitch;

    public void ApplyAudioAction(AudioSource target)
    {
        target.pitch = Random.Range(minPitch, maxPitch);
    }
}
