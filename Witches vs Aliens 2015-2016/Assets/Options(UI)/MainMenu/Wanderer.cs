using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Wanderer : MonoBehaviour {

    Rigidbody2D myRigidbody;

    public Rigidbody2D rigid { get { return myRigidbody; } }

    public AbstractWanderNode source;
    public AbstractWanderNode destination;
    public Vector2 targetPosition;

	// Use this for initialization
	void Awake () {
        myRigidbody = GetComponent<Rigidbody2D>();
	}
}

public abstract class AbstractWanderNode : MonoBehaviour
{
    [SerializeField]
    protected AbstractWanderNode[] neighbors;

    protected AbstractWanderNode randomNeighbor()
    {
        UnityEngine.Assertions.Assert.IsTrue(neighbors.Length >= 2);
        return neighbors[Random.Range(0, neighbors.Length)];
    }

    public abstract void direct(Wanderer wander);

    public abstract Vector2 targetPosition();
}