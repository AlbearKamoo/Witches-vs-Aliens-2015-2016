using UnityEngine;
using System.Collections;

/// <summary>
/// Wrapper class for IEnumerator Coroutines that simplifies pausing and restarting
/// </summary>
public class Countdown {

    //delegate types
    public delegate IEnumerator GetIEnumerator();

    GetIEnumerator source;
    IEnumerator countdown;
    IEnumerator countdownWrapper;
    MonoBehaviour script;

    /// <summary>
    /// Is the countdown running?
    /// </summary>
    public bool active
    {
        get { return countdown != null && !paused; }
    }

    bool paused = false;

    /// <summary>
    /// Is the countdown paused?
    /// </summary>
    public bool Paused { get { return paused; } }

    /// <summary>
    /// Constructs a new countdown.
    /// </summary>
    /// <param name="source">Coroutine to run.</param>
    /// <param name="script">Script to run Coroutine on.</param>
    public Countdown(GetIEnumerator source, MonoBehaviour script)
    {
        this.source = source;
        this.script = script;
    }

    /// <summary>
    /// Constructs a new countdown.
    /// </summary>
    /// <param name="source">Coroutine to run.</param>
    /// <param name="script">Script to run Coroutine on.</param>
    /// <param name="playOnAwake">If true, starts the countdown automatically after construction.</param>
    public Countdown(GetIEnumerator source, MonoBehaviour script, bool playOnAwake) : this(source, script)
    {
        if (playOnAwake)
            Play();
    }

    /// <summary>
    /// Start the countdown, if it has not yet been started or unpaused.
    /// </summary>
    /// <returns>True if the countdown was not already playing, False otherwise.</returns>
    public bool Play()
    {
        if (paused)
        {
            script.StartCoroutine(countdown);
            script.StartCoroutine(countdownWrapper);
            paused = false;
            return true;
        }
        else if (countdown == null)
        {
            countdownWrapper = CountdownWrapper();
            script.StartCoroutine(countdownWrapper);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Restarts the countdown from the beginning.
    /// </summary>
    public void Restart()
    {
        if (countdown != null)
        {
            paused = false; //the if to check if it's true is redundant
            script.StopCoroutine(countdown);
            script.StopCoroutine(countdownWrapper);
        }
        countdownWrapper = CountdownWrapper();
        script.StartCoroutine(countdownWrapper);
    }

    /// <summary>
    /// Pauses the countdown
    /// </summary>
    /// <returns>True if the countdown was active, False otherwise.</returns>
    public bool Pause()
    {
        if (active)
        {
            script.StopCoroutine(countdown);
            script.StopCoroutine(countdownWrapper);
            paused = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Immediately terminates the countdown.
    /// </summary>
    /// <returns>True if the countdown was active or paused, False otherwise.</returns>
    public bool Stop()
    {
        if (countdown != null)
        {
            paused = false; //the if to check if it's true is redundant
            script.StopCoroutine(countdown);
            script.StopCoroutine(countdownWrapper);
            countdown = null;
            countdownWrapper = null;
            return true;
        }
        return false;
    }

    IEnumerator CountdownWrapper()
    {
        countdown = source();
        yield return script.StartCoroutine(countdown);
        countdown = null;
    }
}
