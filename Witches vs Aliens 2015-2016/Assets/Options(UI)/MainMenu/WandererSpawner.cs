using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AbstractWanderNode))]
public class WandererSpawner : MonoBehaviour {
    [SerializeField]
    protected GameObject wandererPrefab;

    [SerializeField]
    protected int numWanderers;

    AbstractWanderNode node;
	// Use this for initialization
	void Start () {
        node = GetComponent<AbstractWanderNode>();

        for(int i = 0; i < numWanderers; i++)
        {
            GameObject instantiatedWanderer = Instantiate(wandererPrefab);
            instantiatedWanderer.transform.position = node.targetPosition();
            node.direct(instantiatedWanderer.GetComponent<Wanderer>());
        }
	}
}
