using UnityEngine;
using System.Collections;

public class StaticAvoidBoid : MonoBehaviour, IStaticAvoidBoid
{

    [SerializeField]
    protected Vector2 _direction;

    void Awake()
    {
        _direction.Normalize();
    }

    public Vector2 direction { get { return _direction; } }
}
