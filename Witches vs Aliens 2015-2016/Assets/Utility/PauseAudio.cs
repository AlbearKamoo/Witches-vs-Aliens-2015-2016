using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PauseAudio : MonoBehaviour, IObserver<PausedMessage>, IObserver<UnPausedMessage>
{

    AudioSource audioSource;

	// Use this for initialization
	void Awake () {
        audioSource = GetComponent<AudioSource>();
        Pause.pausedObservable.Subscribe(this);
        Pause.unpausedObservable.Subscribe(this);
	}

    public void Notify(PausedMessage m)
    {
        audioSource.Pause();
    }

    public void Notify(UnPausedMessage m)
    {
        audioSource.UnPause();
    }
}
