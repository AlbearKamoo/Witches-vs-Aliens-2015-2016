using UnityEngine;
using System.Collections;

public class MultiChargeBlinkAbility : BlinkAbility {
    [SerializeField]
    protected int maxCharges;

    int currentCharges;
    Countdown recharge;

    protected void Awake()
    {
        recharge = Countdown.TimedCountdown(() => ready = true, cooldownTime, this);
        currentCharges = maxCharges;
    }

    public override bool ready
    {
        set
        {
            if (value)
            {
                currentCharges++;
                if (currentCharges > maxCharges)
                {
                    currentCharges = maxCharges;
                }
                else
                {
                    recharge.Restart();
                }

                base.ready = true;
            }
            else
            {
                currentCharges--;
                if (currentCharges == 0)
                    base.ready = false;
                else
                    base.ready = true;
            }
        }
    }

    protected override AbilityStateChangedMessage stateMessage()
    {
        return new ChargesAbilityStateChangedMessage(currentCharges);
    }

    protected override AbilityUIConstructorInfo constructorInfo()
    {
        return new ChargesAbilityUIConstructorInfo(this, maxCharges);
    }

    protected override void StartCooldown()
    {
        recharge.Start();
    }
}
