using UnityEngine;
using System.Collections;

public class DebugCollisions : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.transform);
    }
}
