using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Turbine : MonoBehaviour {

    [SerializeField]
    protected float rotationSpeed;

    public bool active
    {
        set
        {
            this.gameObject.SetActive(value);
            if (value)
            {
                rigid.angularVelocity = rotationSpeed;
            }
        }
    }

    Rigidbody2D rigid;
    Collider2D[] myColliders;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        myColliders = GetComponentsInChildren<Collider2D>();
        active = true;
    }

    public void ignoreCollisions(List<Collider2D> colliders)
    {
        for (int i = 0; i < myColliders.Length; i++)
        {
            for (int j = 0; j < colliders.Count; j++)
            {
                Physics2D.IgnoreCollision(myColliders[i], colliders[j]);
            }
        }
    }
}
