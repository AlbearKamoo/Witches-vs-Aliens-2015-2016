using UnityEngine;
using System.Collections;

//basically the same as Invoke() and InvokeRepeating(), but no reflection!

//also includes LINQ-ish functions for doing lerps

//probably will need expansions/reworks when I actually use this in something


public static class Callback {
    public delegate void CallbackMethod();

    //code that accepts a lerp value from zero to one
    public delegate void Lerpable(float lerpValue);

    //keep the autocomplete namespace distinct between the IEnumerators and the coroutines
    public static class Routines
    {
        //basically Invoke
        public static IEnumerator FireAndForgetRoutine(CallbackMethod code, float time, MonoBehaviour callingScript, Mode mode = Mode.UPDATE)
        {
            switch(mode)
            {
                case Mode.UPDATE:
                case Mode.FIXEDUPDATE:
                    yield return new WaitForSeconds(time);
                    break;
                case Mode.REALTIME:
                    yield return callingScript.StartCoroutine(WaitForRealSecondsRoutine(time));
                    break;
            }
            code();
        }

        //Fires the code on the next update/fixed update. Lazy way to keep the code from affecting what you're doing right now
        public static IEnumerator FireForUpdateRoutine(CallbackMethod code, Mode mode = Mode.UPDATE)
        {
            switch(mode)
            {
                case Mode.UPDATE:
                case Mode.REALTIME:
                    yield return null;
                    break;
                case Mode.FIXEDUPDATE:
                    yield return new WaitForFixedUpdate();
                    break;
            }
            code();
        }

        // TODO : replace with a YieldInstruction override

        //same as WaitForSeconds(), but is not affected by timewarping
        public static IEnumerator WaitForRealSecondsRoutine(float seconds)
        {
            float pauseStartTime = Time.realtimeSinceStartup;
            float pauseEndTime = pauseStartTime + seconds;

            while (Time.realtimeSinceStartup < pauseEndTime)
            {
                yield return 0;
            }
        }

        //does a standard coroutine Lerp on a bit of code, from zero to one by default.
        public static IEnumerator DoLerpRoutine(Lerpable code, float time, MonoBehaviour callingScript, bool reverse = false, Mode mode = Mode.UPDATE)
        {
            IEnumerator routine = null;
            switch (mode)
            {
                case Mode.UPDATE:
                    routine = DoLerpUpdateTimeRoutine(code, time, reverse);
                    break;
                case Mode.FIXEDUPDATE:
                    routine = DoLerpFixedTimeRoutine(code, time, reverse);
                    break;
                case Mode.REALTIME:
                    routine = DoLerpRealtimeRoutine(code, time, reverse);
                    break;
            }
            yield return callingScript.StartCoroutine(routine);
            code(reverse?0:1);
        }

        public static IEnumerator DoLerpUpdateTimeRoutine(Lerpable code, float time, bool reverse = false)
        {
            if (!reverse)
            {
                float timeElapsed = 0;
                while (timeElapsed < time)
                {
                    code(timeElapsed / time);
                    yield return null;
                    timeElapsed += Time.deltaTime;
                }
            }
            else
            {
                float timeRemaining = time;
                while (timeRemaining > 0)
                {
                    code(timeRemaining / time);
                    yield return null;
                    timeRemaining -= Time.deltaTime;
                }
            }
        }

        //same, but run the lerp code independent of any timewarping
        public static IEnumerator DoLerpRealtimeRoutine(Lerpable code, float time, bool reverse = false)
        {
            float realStartTime = Time.realtimeSinceStartup;
            float realEndTime = realStartTime + time;
            if (!reverse)
            {
                while (Time.realtimeSinceStartup < realEndTime)
                {
                    code((Time.realtimeSinceStartup - realStartTime) / time);
                    yield return null;
                }
            }
            else
            {
                while (Time.realtimeSinceStartup < realEndTime)
                {
                    code((realEndTime - Time.realtimeSinceStartup) / time);
                    yield return null;
                }
            }
        }

        //used Time.FixedDeltaTime instead of delta time (for important physics/gameplay things)
        public static IEnumerator DoLerpFixedTimeRoutine(Lerpable code, float time, bool reverse = false)
        {
            if (!reverse)
            {
                float timeElapsed = 0;
                while (timeElapsed < time)
                {
                    code(timeElapsed / time);
                    yield return new WaitForFixedUpdate();
                    timeElapsed += Time.fixedDeltaTime;
                }
            }
            else
            {
                float timeRemaining = time;
                while (timeRemaining > 0)
                {
                    code(timeRemaining / time);
                    yield return new WaitForFixedUpdate();
                    timeRemaining -= Time.fixedDeltaTime;
                }
            }

            code(reverse ? 0 : 1);
        }

        public static IEnumerator WaitForRoutine(Coroutine waitFor, CallbackMethod code)
        {
            yield return waitFor;
            code();
        }
    }

    private static Coroutine RunIfActiveAndEnabled(MonoBehaviour callingScript, IEnumerator code)
    {
        if (callingScript.isActiveAndEnabled)
            return callingScript.StartCoroutine(code);
        else
            return null;
    }

    //wrappers for the routines in the Routines class so that we don't need to call StartCoroutine()
    public static Coroutine FireAndForget(this CallbackMethod code, float time, MonoBehaviour callingScript, Mode mode = Mode.UPDATE)
    {
        return RunIfActiveAndEnabled(callingScript, Routines.FireAndForgetRoutine(code, time, callingScript, mode));
    }

    public static Coroutine FireForUpdate(this CallbackMethod code, MonoBehaviour callingScript, Mode mode = Mode.UPDATE)
    {
        return RunIfActiveAndEnabled(callingScript, Routines.FireForUpdateRoutine(code, mode));
    }

    public static Coroutine WaitForRealSeconds(float seconds, MonoBehaviour callingScript)
    {
        return RunIfActiveAndEnabled(callingScript, Routines.WaitForRealSecondsRoutine(seconds));
    }

    public static Coroutine DoLerp(Lerpable code, float time, MonoBehaviour callingScript, bool reverse = false, Mode mode = Mode.UPDATE)
    {
        return RunIfActiveAndEnabled(callingScript, Routines.DoLerpRoutine(code, time, callingScript, reverse, mode));
    }

    public static Coroutine FollowedBy(this Coroutine toFollow, CallbackMethod code, MonoBehaviour callingScript)
    {
        return RunIfActiveAndEnabled(callingScript, Routines.WaitForRoutine(toFollow, code));
    }

    public enum Mode
    {
        UPDATE,
        FIXEDUPDATE,
        REALTIME
    }
}

public class Countdown
{
    public delegate Coroutine CountdownFunction();
    public bool active {
        set
        {
            if (value)
                Start();
            else
                Stop();
        }
        get { return countdown != null; }
    }
    readonly CountdownFunction routine;
    readonly MonoBehaviour callingScript;
    Coroutine countdown = null;

    public Countdown(CountdownFunction routine, MonoBehaviour callingScript)
    {
        this.routine = routine;
        this.callingScript = callingScript;
    }

    public bool Start()
    {
        if (countdown == null)
        {
            startCountdown();
            return true;
        }
        return false;
    }

    public void Restart()
    {
        Stop();
        startCountdown();
    }

    void startCountdown()
    {
        countdown = callingScript.StartCoroutine(countdownRoutine());
    }

    IEnumerator countdownRoutine()
    {
        yield return routine();
        countdown = null;
    }

    public void Stop()
    {
        callingScript.StopCoroutine(countdown);
    }

    public static Countdown TimedCountdown(Callback.CallbackMethod code, float time, MonoBehaviour callingScript, Callback.Mode mode = Callback.Mode.UPDATE)
    {
        return new Countdown(() => Callback.FireAndForget(code, time, callingScript, mode), callingScript);
    }
}