using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTimer : MonoBehaviour {

    const string overtime = "OVERTIME!";

    public float timeRemainingSec;
    Text UITimer;
    Coroutine timer;
    public bool running
    {
        get { return timer != null; }
        set
        {
            if (timer == null)
            {
                if (value)
                    timer = StartCoroutine(countdown());
            }
            else if (!value)
            {
                StopCoroutine(timer);
                timer = null;
            }


        }
    }
    IEnumerator countdown()
    {
        while (true)
        {
            yield return null;
            timeRemainingSec -= Time.deltaTime;
            if (timeRemainingSec < 0)
            {
                //stuffs
                if (!GetComponentInParent<Score>().GameEnd())
                {
                    //overtime!
                    UITimer.text = overtime;
                    //and VFX
                }
                Debug.Log("GAME END!");
                timer = null;
                yield break;
            }
            else
                setTime();
        }
    }
	// Use this for initialization
	void Awake () {
        UITimer = GetComponent<Text>();
        setTime();
	}

    void setTime()
    {
        UITimer.text = Format.formatMilliseconds(timeRemainingSec);
    }
}
