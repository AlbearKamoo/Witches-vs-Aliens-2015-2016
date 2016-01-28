using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class GameTimer : MonoBehaviour {

    const string overtime = "OVRTME";

    [SerializeField]
    protected GameObject CountdownPrefab;

    [SerializeField]
    protected GameObject OvertimeCountdownPrefab;

    [SerializeField]
    protected Color OvertimeTextColor;
    Color normalTextColor;
    [SerializeField]
    protected Color OvertimeOutlineColor;
    Color normalOutlineColor;

    public float timeRemainingSec;

    Text UITimer;
    Outline timeOutline;
    Image backgroundImg;
    Material background;
    IEnumerator timer;
    Queue<float> countdownTimes = new Queue<float>(new float[] { 5, 4, 3, 2, 1, -1 }); //-1 to ensure that Peek() does not fail
    public bool running
    {
        get { return timer != null; }
        set {
            if (value)
            {
                if (timer == null)
                {
                    timer = countdown();
                    StartCoroutine(timer);
                }
            }
            else if(timer != null)
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
            if (timeRemainingSec < countdownTimes.Peek())
            {
                SimplePool.Spawn(CountdownPrefab, Vector3.zero).GetComponent<TimerCountdown>().count = countdownTimes.Dequeue().ToString();
            }
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
                    SimplePool.Spawn(OvertimeCountdownPrefab, Vector3.zero).GetComponent<TimerCountdown>().count = overtime;
                    Observers.Post(new OvertimeMessage());
                    //and VFX
                }
                running = false;
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
        backgroundImg = transform.parent.GetComponentInChildren<Image>();
        background = Instantiate(backgroundImg.material);
        backgroundImg.material = background;
    }

    void setTime()
    {
        if (timeRemainingSec == 0f)
        {
            UITimer.color = OvertimeTextColor;
            timeOutline.effectColor = OvertimeOutlineColor;
            background.SetFloat(Tags.ShaderParams.imageStrength, 1);
            background.SetFloat(Tags.ShaderParams.alpha, 1);
        }
        else if (timeRemainingSec < 10)
        {
            float lerpValue = (1 - (timeRemainingSec / 10)) / 2;
            UITimer.color = Color.Lerp(normalTextColor, OvertimeTextColor, lerpValue);
            timeOutline.effectColor = Color.Lerp(normalOutlineColor, OvertimeOutlineColor, lerpValue);
            background.SetFloat(Tags.ShaderParams.imageStrength, lerpValue);
            background.SetFloat(Tags.ShaderParams.alpha, lerpValue);
        }
        UITimer.text = Format.formatSeconds(timeRemainingSec);
    }
}

public class OvertimeMessage : Message
{
    public const string classMessageType = "OvertimeMessage";
    public OvertimeMessage()
        : base(classMessageType)
    {
    }
}