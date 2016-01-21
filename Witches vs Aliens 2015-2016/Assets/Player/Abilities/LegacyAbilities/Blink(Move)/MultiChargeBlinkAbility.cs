using UnityEngine;
using System.Collections;

public class MultiChargeBlinkAbility : BlinkAbility {
    [SerializeField]
    protected int maxCharges;

    int currentCharges;
    IEnumerator recharge;

    protected override void Awake()
    {
        base.Awake();
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
                    if (recharge != null)
                    {
                        StopCoroutine(recharge);
                    }
                    recharge = Callback.Routines.FireAndForgetRoutine(() => { ready = true; recharge = null; }, cooldownTime, this);
                    StartCoroutine(recharge);
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
        if (recharge == null)
        {
            recharge = Callback.Routines.FireAndForgetRoutine(() => ready = true, cooldownTime, this);
            StartCoroutine(recharge);
        }
    }
}
