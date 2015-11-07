using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OvertimeCountdown : Countdown {

    Image backgroundImg;
    Material background;

    protected override void Awake()
    {
        base.Awake();
        backgroundImg = GetComponentInChildren<Image>();
        background = Instantiate(backgroundImg.material);
        backgroundImg.material = background;
    }

    protected override void lerpIn(float l)
    {
        base.lerpIn(l);
        background.SetFloat(Tags.ShaderParams.imageStrength, 0.5f + l);
        background.SetFloat(Tags.ShaderParams.alpha, 0.5f + l);
    }

    protected override void Despawn()
    {
        Color clearEndTextColor = endTextColor.setAlphaFloat(0);
        Color clearEndOutlineColor = endOutlineColor.setAlphaFloat(0);
        Callback.DoLerp((float l) =>
        {
            text.color = Color.Lerp(endTextColor, clearEndTextColor, l);
            outline.effectColor = Color.Lerp(endOutlineColor, clearEndOutlineColor, l);
            background.SetFloat(Tags.ShaderParams.imageStrength, 1-l);
            background.SetFloat(Tags.ShaderParams.alpha, 1-l);
        }, 0.5f, this).FollowedBy(base.Despawn, this);
    }
}
