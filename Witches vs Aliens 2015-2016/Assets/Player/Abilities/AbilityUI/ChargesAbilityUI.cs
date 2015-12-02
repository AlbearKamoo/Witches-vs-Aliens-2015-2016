using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

public class ChargesAbilityUI : AbstractAbilityUI {
    [SerializeField]
    protected GameObject indicatorPrefab;

    [SerializeField]
    protected Vector2 indicatorSeperation;

    int currentCharges = 0;
    ParticleSystem[] vfx;

    public override void Construct(AbilityUIConstructorInfo info)
    {
        base.Construct(info);

        Assert.IsTrue(indicatorSeperation.x >= 0);

        switch (type)
        {
            case AbilityType.MOVEMENT:
                // indicatorSeperation = indicatorSeperation;
                break;
            case AbilityType.GENERIC:
                indicatorSeperation = -indicatorSeperation;
                break;
        }

        int maxCharges = (info as ChargesAbilityUIConstructorInfo).numCharges;

        vfx = new ParticleSystem[maxCharges];

        for (int i = 0; i < maxCharges; i++)
        {
            Transform indicator = Instantiate(indicatorPrefab).transform;
            indicator.SetParent(this.transform, false);
            indicator.localPosition = i * indicatorSeperation;
            vfx[i] = indicator.GetComponent<ParticleSystem>();
            vfx[i].Play();
        }
    }

    public override void Notify(AbilityStateChangedMessage m)
    {
        setNumCharges((m as ChargesAbilityStateChangedMessage).numCharges);
    }

    public override void Notify(ResetMessage m)
    {
        int previousCharges = currentCharges;
        setNumCharges(0);
        Callback.FireForUpdate(() => setNumCharges(previousCharges), this);
    }

    void setNumCharges(int numCharges)
    {
        for (int i = currentCharges; i < numCharges; i++)
        {
            vfx[i].Play();
        }
        
        for(int i = numCharges; i < currentCharges; i++)
        {
            vfx[i].Stop();
            vfx[i].Clear();
        }
        currentCharges = numCharges;
    }
}
public class ChargesAbilityStateChangedMessage : AbilityStateChangedMessage
{
    public readonly int numCharges;

    public ChargesAbilityStateChangedMessage(int numCharges)
        : base(numCharges == 0)
    {
        this.numCharges = numCharges;
    }
}

public class ChargesAbilityUIConstructorInfo : AbilityUIConstructorInfo
{
    public readonly int numCharges;

    public ChargesAbilityUIConstructorInfo(NotSuperAbility ability, int numCharges)
        : base(ability)
    {
        this.numCharges = numCharges;
    }
}