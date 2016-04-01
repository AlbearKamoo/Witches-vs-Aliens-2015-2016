using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ProgrammaticSpawning))]
public class PlayerAddon : MonoBehaviour {

    [SerializeField]
    protected GameObject addonPrefab;

	// Use this for initialization
	void Start () {
        ProgrammaticSpawning action = GetComponent<ProgrammaticSpawning>();
        for (int i = 0; i < action.Players.Length; i++)
        {
            Instantiate(addonPrefab).transform.SetParent(action.Players[i], false);
        }
        Instantiate(addonPrefab).transform.SetParent(action.Puck, false);
        Destroy(this);
	}
}
