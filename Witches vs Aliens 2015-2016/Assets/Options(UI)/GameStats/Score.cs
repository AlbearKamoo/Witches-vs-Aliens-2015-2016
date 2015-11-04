using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class Score : MonoBehaviour, IObserver<Message> {
    [SerializeField]
    [AutoLink(parentTag = Tags.canvas, parentName = "LeftScoreBoard")]
    protected Text leftScoreBoard;
    Outline leftOutline;

    [SerializeField]
    [AutoLink(parentTag = Tags.canvas, parentName = "RightScoreBoard")]
    protected Text rightScoreBoard;
    Outline rightOutline;

    Material background;
    bool overtime = false;

    [SerializeField]
    protected float scoreFXTime;

    int leftScore = 0;
    int rightScore = 0;
	// Use this for initialization
    void Awake()
    {
        Observers.Subscribe(this, new string[] { GoalScoredMessage.classMessageType });
        Image image = GetComponent<Image>();
        background = Instantiate(image.material); //workaround to avoid modifying assets directly
        image.material = background;
        leftOutline = leftScoreBoard.GetComponent<Outline>();
        rightOutline = rightScoreBoard.GetComponent<Outline>();

        int imageStrength = Shader.PropertyToID("_ImageStrength");
        float baseImageStrength = background.GetFloat(imageStrength);
        int alpha = Shader.PropertyToID("_MainTexAlpha");
        float baseAlpha = background.GetFloat(alpha);
        CanvasGroup group = GetComponent<CanvasGroup>();
        Callback.DoLerp((float l) => {background.SetFloat(imageStrength, baseImageStrength * l); background.SetFloat(alpha, baseAlpha * l); group.alpha = l; }, 1f, this)
            .FollowedBy(() => Destroy(group), this);
    }

    void UpdateScore(Side side)
    {
        switch (side)
        {
            case Side.LEFT:
                UpdateUI(leftScoreBoard, leftOutline, ++leftScore);
                break;
            case Side.RIGHT:
                UpdateUI(rightScoreBoard, rightOutline, ++rightScore);
                break;
        }
        if (overtime)
            GameEnd();
    }

    void UpdateUI(Text text, Outline outline, int newScore)
    {
        text.text = newScore.ToString("00");
        Color baseOutlineColor = outline.effectColor;
        Color baseTextColor = text.color;
        
        Callback.DoLerp((float l) => {text.color = Color.Lerp(Color.black, baseTextColor, l);
        outline.effectColor = Color.Lerp(Color.white, baseOutlineColor, l);
        }, scoreFXTime, this).FollowedBy(() => { text.color = baseTextColor; outline.effectColor = baseOutlineColor;}, this);
    }

    public void Notify(Message m)
    {
        switch (m.messageType)
        {
            case GoalScoredMessage.classMessageType:
                UpdateScore((m as GoalScoredMessage).side);
                animateBackgroundTexture();
                break;
        }
    }

    void animateBackgroundTexture()
    {
        float baseNoiseStrength = background.GetFloat(Tags.ShaderParams.noiseStrength);
        Callback.DoLerp((float l) => background.SetFloat(Tags.ShaderParams.noiseStrength, baseNoiseStrength * (1 + Mathf.Pow((1 - l), 2) * 10 * Mathf.Cos(60f * l))), scoreFXTime / 2, this)
            .FollowedBy(() => background.SetFloat(Tags.ShaderParams.noiseStrength, baseNoiseStrength), this);
    }

    public bool GameEnd()
    {
        if (leftScore == rightScore)
        {
            overtime = true;
            return false;
        }
        else
        {
            //end the game
            Application.LoadLevel(Tags.Scenes.select); //should really be the main menu, but we don't have one
            return true;
        }
    }
}
