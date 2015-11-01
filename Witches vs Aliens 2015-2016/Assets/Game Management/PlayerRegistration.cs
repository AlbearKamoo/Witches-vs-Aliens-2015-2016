using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(SetupData))]
public class PlayerRegistration : MonoBehaviour {

#if UNITY_EDITOR
    [SerializeField]
    protected string mainGameSceneName;
#else
    const string mainGameSceneName = Tags.Scenes.root;
#endif
    [SerializeField]
    protected GameObject playerRegistrationPrefab;

    [SerializeField]
    protected PlayerRegisters[] possiblePlayers;

    SetupData data;
    CharacterSelector[] playerSelections;
	void Awake ()
    {
        data = GetComponent<SetupData>();

        playerSelections = new CharacterSelector[possiblePlayers.Length];
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            GameObject spawnedPlayerRegistationPuck = (GameObject)Instantiate(playerRegistrationPrefab, new Vector2(i - possiblePlayers.Length / 2, 0), Quaternion.identity); //the positions are temporary
            switch (possiblePlayers[i].bindings.inputMode)
            {
                case InputConfiguration.PlayerInputType.MOUSE:
                    spawnedPlayerRegistationPuck.AddComponent<MousePlayerInput>().bindings = possiblePlayers[i].bindings;
                    break;
                case InputConfiguration.PlayerInputType.JOYSTICK:
                    spawnedPlayerRegistationPuck.AddComponent<JoystickPlayerInput>().bindings = possiblePlayers[i].bindings;
                    break;
            }
            playerSelections[i] = spawnedPlayerRegistationPuck.AddComponent<CharacterSelector>();
            InputToAction action = spawnedPlayerRegistationPuck.GetComponent<InputToAction>();
            action.rotationEnabled = false;
            action.movementEnabled = true;
            spawnedPlayerRegistationPuck.GetComponentInChildren<Image>().color = possiblePlayers[i].color;
            spawnedPlayerRegistationPuck.GetComponentInChildren<Text>().text = possiblePlayers[i].abbreviation;
        }

        Callback.FireAndForget(startGame, 10f, this);
	}

    void startGame()
    {
        int count = 0;
        for(int i = 0; i < playerSelections.Length; i++)
        {
            if (playerSelections[i].selectedCharacter != null)
                count++;
        }
        //if count == 0 do something to reset
        data.playerComponentPrefabs = new PlayerComponents[count];

        count = 0;
        for (int i = 0; i < playerSelections.Length; i++)
        {
            if (playerSelections[i].selectedCharacter != null)
                data.playerComponentPrefabs[count++] = new PlayerComponents( playerSelections[i].selectedCharacter, possiblePlayers[i].bindings);
        }
        Application.LoadLevel(mainGameSceneName);
        Destroy(this);
    }
}

[System.Serializable]
public class PlayerRegisters
{
    public string name;
    public string abbreviation;
    public Color color;
    public InputConfiguration bindings;
    public PlayerRegisters() { }
}