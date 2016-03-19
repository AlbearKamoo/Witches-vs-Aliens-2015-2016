using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Options should be in the Tag static class

//Sets the audiosource to use music's volume settings

[RequireComponent(typeof(AudioSource))]
public class VolumeController : MonoBehaviour, IObserver<Message>
{
    AudioSource music;

    float baseVolume;

    void Awake()
    {
        music = GetComponent<AudioSource>();
        music.ignoreListenerVolume = true;

        baseVolume = music.volume;

        Observers.Subscribe(this, Tags.Options.MusicLevel, Tags.Options.SoundLevel);

        if (PlayerPrefs.HasKey(Tags.Options.SoundLevel)) // set volumes from stored values
            AudioListener.volume = PlayerPrefs.GetInt(Tags.Options.SoundLevel) / 100.0f;
        else
            PlayerPrefs.SetInt(Tags.Options.SoundLevel, 100);

        if (PlayerPrefs.HasKey(Tags.Options.MusicLevel)) // set volumes from stored values
            music.volume = baseVolume * PlayerPrefs.GetInt(Tags.Options.MusicLevel) / 100.0f;
        else
            PlayerPrefs.SetInt(Tags.Options.MusicLevel, 100);
        PlayerPrefs.Save();

    }

    public void Notify(Message message)
    {
        switch (message.messageType)
        {
            case Tags.Options.MusicLevel:
                music.volume = baseVolume * PlayerPrefs.GetInt(Tags.Options.MusicLevel) / 100.0f;
                break;
            case Tags.Options.SoundLevel:
                AudioListener.volume = PlayerPrefs.GetInt(Tags.Options.SoundLevel) / 100.0f;
                break;
            //else it's for something else
        }
    }
}