using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioSource))] // just in case someone really screws up when importing
[RequireComponent(typeof(VolumeController))]
public class MusicManager : MonoBehaviour
{
    private AudioSource source;
    private static bool created = false; //_self has issues when looping back to the starting scene
    bool paused = false;
    private static MusicManager _self; //there can only be one
    public static MusicManager self { get { return _self; } }

    [SerializeField]
    private AudioClip[] playlistData; //for serialization and the inspector; not used after the data is loaded into the queue
    Queue<AudioClip> playlist;

    void Awake()
    {
        if (created)
            Destroy(this.gameObject);
        created = true;
        _self = this;
        DontDestroyOnLoad(this.gameObject);
        Assert.IsTrue(playlistData.Length != 0);
        source = GetComponent<AudioSource>();
        if (!paused)
        {
            source.Play();
            Debug.Log("Play");
        }
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

    public void Pause()
    {
        Debug.Log("Puase");
        paused = true;
        source.Pause();
    }

    public void Skip()
    {
        paused = false;
        playlist.Enqueue(playlist.Peek()); //everything is on a loop
        source.clip = playlist.Dequeue();
        source.Play();
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (!paused && !source.isPlaying)
            {
                playNext();
            }
            yield return null;
        }
    }
}
