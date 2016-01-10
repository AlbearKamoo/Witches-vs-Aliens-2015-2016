using UnityEngine;
using System.Collections;

public class MeteorTarget : MonoBehaviour, ISpawnable {

    [SerializeField]
    protected GameObject MeteorCraterPrefab;

    [SerializeField]
    protected float delayMax;

    [SerializeField]
    protected float targetingTime;

    public Side side;

    public void Create()
    {
        transform.rotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).ToRotation();

        Vector2[] initialPositions = new Vector2[transform.childCount];

        int i = 0;
        foreach (Transform child in transform)
        {
            initialPositions[i] = child.localPosition;
            i++;
        }

        foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
            rend.enabled = false;

        Callback.FireAndForget(() =>
            {
            foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
                rend.enabled = true;

            Callback.DoLerp((float l) =>
                {
                    int i1 = 0;
                    foreach (Transform child in transform)
                    {
                        child.localPosition = Vector2.Lerp(initialPositions[i1], Vector2.zero, l);
                        i1++;
                    }
                }, targetingTime, this).FollowedBy(() =>
                    {
                        int i2 = 0;
                        foreach (Transform child in transform)
                        {
                            child.localPosition = initialPositions[i2];
                            i2++;
                        }
                        SimplePool.Spawn(MeteorCraterPrefab, this.transform.position, this.transform.rotation).GetComponent<MeteorCrater>().side = side;
                        SimplePool.Despawn(this.gameObject);
                    }, this);
            }, Random.value * delayMax, this);
    }
}
