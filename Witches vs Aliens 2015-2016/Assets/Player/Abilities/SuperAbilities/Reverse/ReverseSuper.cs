using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReverseSuper : SuperAbility, IOpponentsAbility {

    [SerializeField]
    protected GameObject stunVisualsPrefab;

    [SerializeField]
    protected float duration;

    [SerializeField]
    protected float visualsLerpTime;

    List<Transform> _opponents;
    public List<Transform> opponents { set { _opponents = value; } }

    protected override void Start()
    {
        base.Start();
        //ready = true; //for easy testing
    }

    protected override void onFire(Vector2 direction)
    {
        ready = false;
        Callback.CallbackMethod[] modifiers = new Callback.CallbackMethod[_opponents.Count];

        for(int i = 0; i < _opponents.Count; i++)
        {
            InputToAction input = _opponents[i].GetComponent<InputToAction>();
            modifiers[i] = () => input.normalizedMovementInput = -input.normalizedMovementInput;
            input.PreFixedUpdateDelegates.Add(modifiers[i]);
            GameObject visuals = SimplePool.Spawn(stunVisualsPrefab);
            visuals.transform.SetParent(_opponents[i], false);
            SpriteRenderer rend = visuals.GetComponent<SpriteRenderer>();
            Callback.DoLerp((float l) => rend.color.setAlphaFloat(l), visualsLerpTime, this);
            Callback.FireAndForget(() => Callback.DoLerp((float l) => rend.color.setAlphaFloat(l), visualsLerpTime, this, reverse: true), duration - visualsLerpTime, this).FollowedBy(() => SimplePool.Despawn(visuals), this);
        }

        Callback.FireAndForget(() => {
            for(int i = 0; i < _opponents.Count; i++)
                _opponents[i].GetComponent<InputToAction>().PreFixedUpdateDelegates.Remove(modifiers[i]);}, duration, this);
    }
}
