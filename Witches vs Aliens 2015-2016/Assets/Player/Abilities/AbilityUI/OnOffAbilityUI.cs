using UnityEngine;
using System.Collections;

public class OnOffAbilityUI : AbstractAbilityUI {

    ParticleSystem vfx;
    ParticleSystem ping;

    protected virtual void Awake()
    {
        vfx = transform.Find("MainVisuals").GetComponent<ParticleSystem>();
        ping = transform.Find("Ping").GetComponent<ParticleSystem>();
    }

    public override void Notify(AbilityStateChangedMessage m) //update our display state
    {
        if (m.ready)
        {
            vfx.Play();
            ping.Play();
            Debug.Log("PING!");
        }
        else
        {
            vfx.Stop();
            vfx.Clear();
        }
    }

    public override void Notify(ResetMessage m) //when this happens, disable all distance-emission particle effects for one frame because the player is about to teleport
    {
        if (vfx.isPlaying)
        {
            vfx.Pause();
            vfx.Clear();
            Callback.FireForUpdate(() => vfx.Play(), this);
        }
    }

}
