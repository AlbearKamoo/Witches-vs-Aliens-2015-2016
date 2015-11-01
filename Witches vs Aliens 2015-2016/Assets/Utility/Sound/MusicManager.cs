using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))] // just in case someone really screws up when importing
[RequireComponent(typeof(VolumeController))]
public class MusicManager : MonoBehaviour
{
    private AudioSource source;
    private static MusicManager _self; //there can only be one

    [SerializeField]
    private AudioClip[] playlistData; //for serialization and the inspector; not used after the data is loaded into the queue
    Queue<AudioClip> playlist;

    void Awake()
    {
        if (_self != null)
            Destroy(this.gameObject);
        _self = this;
        DontDestroyOnLoad(this.gameObject);
        Assert.IsTrue(playlistData.Length != 0);
        source = GetComponent<AudioSource>();
        source.Play();
        playlist = new Queue<AudioClip>(playlistData);
        playlistData = null;
        StartCoroutine(UpdateCoroutine());
    }

    void playNext()
    {
        playlist.Enqueue(playlist.Peek()); //everything is on a loop
        source.clip = playlist.Dequeue();
        source.Play();
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (!source.isPlaying)
            {
                playNext();
            }
            yield return null;
        }
    }
}
