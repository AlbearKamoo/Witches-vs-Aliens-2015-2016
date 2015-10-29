using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour {

    //demonstration that we can actually do this

    public PlayerComponents[] playerComponentPrefabs;

    [AutoLink(parentTag = Tags.stage, parentName = "Player1RespawnPoint")]
    public Transform P1respawnPoint;

    [AutoLink(parentTag = Tags.stage, parentName = "Player2RespawnPoint")]
    public Transform P2respawnPoint;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < playerComponentPrefabs.Length; i++)
        {
            GameObject spawnedPlayer;
            if (i == 0)
            {
             spawnedPlayer = (GameObject)Instantiate(playerComponentPrefabs[i].basePlayer, P1respawnPoint.position, Quaternion.identity);
            }
            else
            {
                spawnedPlayer = (GameObject)Instantiate(playerComponentPrefabs[i].basePlayer, P2respawnPoint.position, Quaternion.identity);
            }
            if (playerComponentPrefabs[i].mouseMode)
            {
                spawnedPlayer.AddComponent<MousePlayerInput>().bindings = playerComponentPrefabs[i].bindings;
            }
            else
            {
                spawnedPlayer.AddComponent<JoystickPlayerInput>().bindings = playerComponentPrefabs[i].bindings;
            }
            GameObject.Instantiate(playerComponentPrefabs[i].movementAbility).transform.SetParent(spawnedPlayer.transform, false);
            GameObject.Instantiate(playerComponentPrefabs[i].genericAbility).transform.SetParent(spawnedPlayer.transform, false);
            //GameObject.Instantiate(playerComponentPrefabs[i].superAbility).transform.SetParent(spawnedPlayer.transform);
        }
	}
}
[System.Serializable]
public class PlayerComponents
{
    public GameObject basePlayer;
    public GameObject movementAbility;
    public GameObject genericAbility;
    //public GameObject superAbility;

    public bool mouseMode;
    public InputConfiguration bindings;
    public PlayerComponents() { }
}