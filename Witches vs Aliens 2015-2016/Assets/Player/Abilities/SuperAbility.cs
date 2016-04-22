using UnityEngine;
using System.Collections;

public class SuperAbility : AbstractAbility
{
    const float tooltipDelay = 3f;

    public override AbilityType type { get { return AbilityType.SUPER; } }
    bool _ready = false;
    public override bool ready {
        get { return _ready; }
        set
        {
            visuals.SetActive(value);
            if (tooltip != null)
            {
                tooltip.SetActive(false);
                if (value)
                    Callback.FireAndForget(() => { if (ready) tooltip.SetActive(true); }, tooltipDelay, this);
            }

            _ready = value;
        }
    }

    GameObject visuals;
    GameObject tooltip;
    protected virtual void Start()
    {
        if (transform.parent != null)
        {
            visuals = transform.parent.Find("SuperUI").gameObject;
            AbstractPlayerInput input = transform.root.GetComponent<AbstractPlayerInput>();
            if (input != null)
            {
                switch (transform.root.GetComponent<AbstractPlayerInput>().bindings.inputMode)
                {
                    case InputConfiguration.PlayerInputType.MOUSE:
                        tooltip = transform.parent.Find("SuperUI/Tooltips/Mouse Tooltip").gameObject;
                        break;
                    case InputConfiguration.PlayerInputType.JOYSTICK:
                        tooltip = transform.parent.Find("SuperUI/Tooltips/Joystick Tooltip").gameObject;
                        break;
                }
            }
            else
            {
                Destroy(tooltip);
            }
        }
    }

    protected override void onFire(Vector2 direction)
    {
        Debug.Log("SUPER!"); //override and remove this
        ready = false;
    }
}
