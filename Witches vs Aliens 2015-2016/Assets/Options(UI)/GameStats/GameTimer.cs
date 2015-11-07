using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameTimer : MonoBehaviour {

    const string overtime = "OVERTIME!";

    [SerializeField]
    protected GameObject CountdownPrefab;

    [SerializeField]
    protected Color OvertimeTextColor;
    Color normalTextColor;
    [SerializeField]
    protected Color OvertimeOutlineColor;
    Color normalOutlineColor;

    public float timeRemainingSec;
    Text UITimer;
    Outline timeOutline;
    Image background;
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
                timeRemainingSec = 0;
                setTime();
                //stuffs
                if (!GetComponentInParent<Score>().GameEnd())
                {
                    //overtime!
                    UITimer.text = overtime;
                    UITimer.alignment = TextAnchor.MiddleCenter;
                    //and VFX
                }
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
        normalTextColor = UITimer.color;
        timeOutline = GetComponent<Outline>();
        normalOutlineColor = timeOutline.effectColor;
        setTime();
	}

    void Start()
    {
        background = transform.parent.GetComponentInChildren<Image>();
    }

    void setTime()
    {
        Debug.Log(timeRemainingSec);
        if (timeRemainingSec == 0f)
        {
            UITimer.color = OvertimeTextColor;
            timeOutline.effectColor = OvertimeOutlineColor;
            background.color = Color.white;
        }
        else if (timeRemainingSec < 10)
        {
            float lerpValue = (1 - (timeRemainingSec / 10)) / 2;
            UITimer.color = Color.Lerp(normalTextColor, OvertimeTextColor, lerpValue);
            timeOutline.effectColor = Color.Lerp(normalOutlineColor, OvertimeOutlineColor, lerpValue);
            background.color = Color.Lerp(Color.clear, Color.white, lerpValue);
        }
        UITimer.text = Format.formatMilliseconds(timeRemainingSec);
    }
}
