using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour {

    public GameObject playerPrefab;
    [SerializeField]
    public InputConfiguration[] bindings;
    public int numPlayers;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numPlayers; i++)
        {
            SimplePool.Spawn(playerPrefab).GetComponent<PlayerInput>().bindings = bindings[i];
        }
	}
}
