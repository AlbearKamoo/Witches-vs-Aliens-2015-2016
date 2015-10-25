using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour {

    //demonstration that we can actually do this

    public GameObject[] playerPrefabs;
    [SerializeField]
    public InputConfiguration[] bindings;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < bindings.Length && i < playerPrefabs.Length; i++ )
        {
            SimplePool.Spawn(playerPrefabs[i]).GetComponent<PlayerInput>().bindings = bindings[i];
        }
	}
}
