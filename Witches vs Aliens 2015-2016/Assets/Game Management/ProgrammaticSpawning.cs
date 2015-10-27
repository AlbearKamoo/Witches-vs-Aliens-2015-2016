using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour {

    //demonstration that we can actually do this

    public PlayerComponents[] playerComponentPrefabs;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < playerComponentPrefabs.Length; i++)
        {
            GameObject spawnedPlayer = GameObject.Instantiate(playerComponentPrefabs[i].basePlayer);
            if (playerComponentPrefabs[i].mouseMode)
            {
                spawnedPlayer.AddComponent<MousePlayerInput>().bindings = playerComponentPrefabs[i].bindings;
            }
            else
            {
                spawnedPlayer.AddComponent<JoystickPlayerInput>().bindings = playerComponentPrefabs[i].bindings;
            }
            GameObject.Instantiate(playerComponentPrefabs[i].movementAbility).transform.SetParent(spawnedPlayer.transform);
            //GameObject.Instantiate(playerComponentPrefabs[i].superAbility).transform.SetParent(spawnedPlayer.transform);
        }
	}
}
[System.Serializable]
public class PlayerComponents
{
    public GameObject basePlayer;
    public GameObject movementAbility;
    //public GameObject superAbility;

    public bool mouseMode;
    public InputConfiguration bindings;
    public PlayerComponents() { }
}