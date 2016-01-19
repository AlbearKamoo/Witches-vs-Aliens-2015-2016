using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(SetupData))]
public class PlayerRegistration : MonoBehaviour {
    [SerializeField]
    protected GameObject introMusicPrefab;
    GameObject introMusic;

    [SerializeField]
    protected AudioClip[] mainScenePlaylist;
#if UNITY_EDITOR
    [SerializeField]
    protected string mainGameSceneName;
#else
    const string mainGameSceneName = Tags.Scenes.root;
#endif
    [SerializeField]
    protected GameObject playerRegistrationPrefab;

    [SerializeField]
    protected GameObject playerRegistrationUIPrefab;

    [SerializeField]
    protected PlayerRegisters[] possiblePlayers;

    [SerializeField]
    protected CharacterHolder[] charactersData; //this array maps the characters to ints, for networking.

    [SerializeField]
    [AutoLink(parentTag = Tags.canvas, childPath = "RegisteredPlayers")]
    protected Transform UIParent;

    SetupData data;
    CharacterSelector[] playerSelections;
    RegisteredPlayerUIView[] playerUI;
    RegistrationState[] registrationStates;

    IEnumerator startCountdown;
	void Awake ()
    {
        data = GetComponent<SetupData>();

        playerSelections = new CharacterSelector[possiblePlayers.Length];
        playerUI = new RegisteredPlayerUIView[possiblePlayers.Length];

        registrationStates = new RegistrationState[possiblePlayers.Length];
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            registrationStates[i] = RegistrationState.NOTREGISTERED;
        }
	}

    void Start()
    {
        for (int i = 0; i < charactersData.Length; i++)
        {
            charactersData[i].characterID = i;
        }
    }

    void Update()
    {
        checkInput();

        //now check if all are ready
        checkReady();
    }

    void checkInput()
    {
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            if (pressedAccept(i)) //register
            {
                OnPressedAccept(i);
            }
            else if (pressedBack(i)) //deregister
            {
                OnPressedBack(i);
            }
        }
    }

    void OnPressedAccept(int playerIndex)
    {
        switch (registrationStates[playerIndex])
        {
            case RegistrationState.NOTREGISTERED:
                spawnPlayerRegistrationPuck(playerIndex);
                break;
            case RegistrationState.REGISTERING:
                if (validCharacterID(playerSelections[playerIndex].SelectedCharacterID))
                    setPlayerReady(playerIndex);
                break;
            //case RegistrationState.READY: //don't need to do anything
        }
    }

    void spawnPlayerRegistrationPuck(int playerIndex)
    {
        GameObject spawnedPlayerRegistationPuck = (GameObject)Instantiate(playerRegistrationPrefab, Vector2.zero, Quaternion.identity); //the positions are temporary
        Stats spawnedStats = spawnedPlayerRegistationPuck.AddComponent<Stats>();
        spawnedStats.playerID = Stats.nextPlayerID();
        spawnedStats.networkMode = NetworkMode.UNKNOWN; //TODO : change when we add networking to registration

        switch (possiblePlayers[playerIndex].bindings.inputMode)
        {
            case InputConfiguration.PlayerInputType.MOUSE:
                spawnedPlayerRegistationPuck.AddComponent<MousePlayerInput>().bindings = possiblePlayers[playerIndex].bindings;
                break;
            case InputConfiguration.PlayerInputType.JOYSTICK:
                spawnedPlayerRegistationPuck.AddComponent<JoystickCustomDeadZoneInput>().bindings = possiblePlayers[playerIndex].bindings;
                break;
        }

        //spawn them
        playerSelections[playerIndex] = spawnedPlayerRegistationPuck.AddComponent<CharacterSelector>();
        playerUI[playerIndex] = SimplePool.Spawn(playerRegistrationUIPrefab).GetComponent<RegisteredPlayerUIView>();
        playerUI[playerIndex].transform.SetParent(UIParent, Vector3.one, false);
        InputToAction action = spawnedPlayerRegistationPuck.GetComponent<InputToAction>();
        action.rotationEnabled = false;
        action.movementEnabled = true;
        playerUI[playerIndex].inputMode = possiblePlayers[playerIndex].bindings.inputMode;
        playerUI[playerIndex].ready = false;
        playerUI[playerIndex].playerColor = spawnedPlayerRegistationPuck.GetComponentInChildren<Image>().color = spawnedPlayerRegistationPuck.GetComponent<ParticleSystem>().startColor = possiblePlayers[playerIndex].color;
        spawnedPlayerRegistationPuck.GetComponentInChildren<Text>().text = possiblePlayers[playerIndex].abbreviation;
        playerUI[playerIndex].playerName = possiblePlayers[playerIndex].name;

        registrationStates[playerIndex] = RegistrationState.REGISTERING;
    }

    void setPlayerReady(int playerIndex)
    {
        registrationStates[playerIndex] = RegistrationState.READY;
        playerUI[playerIndex].ready = true;
    }

    void OnPressedBack(int playerIndex)
    {
        switch (registrationStates[playerIndex])
        {
            //case RegistrationState.NOTREGISTERED: //don't need to do anything
            case RegistrationState.REGISTERING:
                despawnPlayerRegistrationPuck(playerIndex);
                break;
            case RegistrationState.READY:
                setPlayerRegistering(playerIndex);
                break;
        }
    }

    void despawnPlayerRegistrationPuck(int playerIndex)
    {
        Destroy(playerSelections[playerIndex].gameObject);
        playerSelections[playerIndex] = null;
        playerUI[playerIndex].Despawn();
        playerUI[playerIndex] = null;
        registrationStates[playerIndex] = RegistrationState.NOTREGISTERED;
    }

    void setPlayerRegistering(int playerIndex)
    {
        registrationStates[playerIndex] = RegistrationState.REGISTERING;
        playerUI[playerIndex].ready = false;
    }

    void checkReady()
    {
        bool ready = registrationStates.All<RegistrationState>((RegistrationState s) => s != RegistrationState.REGISTERING) && registrationStates.Any<RegistrationState>((RegistrationState s) => s == RegistrationState.READY);
        if (startCountdown == null)
        {
            if (ready)
            {
                introMusic = Instantiate(introMusicPrefab);
                startCountdown = Callback.Routines.FireAndForgetRoutine(() => startGame(), 5, this);
                StartCoroutine(startCountdown);
                for (int i = 0; i < playerSelections.Length; i++)
                    if (registrationStates[i] == RegistrationState.READY)
                        playerSelections[i].GetComponent<InputToAction>().movementEnabled = false;
            }
        }
        else
        {
            if (!ready)
            {
                if (startCountdown != null)
                {
                    StopCoroutine(startCountdown);
                    startCountdown = null;
                }
                Destroy(introMusic);
                for (int i = 0; i < playerSelections.Length; i++)
                    if (registrationStates[i] != RegistrationState.NOTREGISTERED)
                        playerSelections[i].GetComponent<InputToAction>().movementEnabled = true;
            }
        }
    }

    bool pressedAccept(int i)
    {
        return possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE && Input.GetMouseButtonDown(0)
                    || possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK && Input.GetAxis(possiblePlayers[i].bindings.movementAbilityAxis) != 0;
    }

    bool pressedBack(int i)
    {
        return possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE && Input.GetMouseButtonDown(1)
                    || possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK && Input.GetAxis(possiblePlayers[i].bindings.genericAbilityAxis) != 0;
    }
    // non-state-machine implementation
    /*
    void Update()
    {
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            if (playerSelections[i] == null)
            {
                if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE && Input.GetMouseButtonDown(0)
                    || possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK && Input.GetAxis(possiblePlayers[i].bindings.AcceptAxis) != 0) //register
                {
                    GameObject spawnedPlayerRegistationPuck = (GameObject)Instantiate(playerRegistrationPrefab, Vector2.zero, Quaternion.identity); //the positions are temporary
                    switch (possiblePlayers[i].bindings.inputMode)
                    {
                        case InputConfiguration.PlayerInputType.MOUSE:
                            spawnedPlayerRegistationPuck.AddComponent<MousePlayerInput>().bindings = possiblePlayers[i].bindings;
                            break;
                        case InputConfiguration.PlayerInputType.JOYSTICK:
                            spawnedPlayerRegistationPuck.AddComponent<JoystickCustomDeadZoneInput>().bindings = possiblePlayers[i].bindings;
                            break;
                    }

                    //spawn them
                    playerSelections[i] = spawnedPlayerRegistationPuck.AddComponent<CharacterSelector>();
                    playerUI[i] = SimplePool.Spawn(playerRegistrationUIPrefab).GetComponent<RegisteredPlayerUIView>();
                    playerUI[i].transform.SetParent(UIParent, Vector3.one, false);
                    InputToAction action = spawnedPlayerRegistationPuck.GetComponent<InputToAction>();
                    action.rotationEnabled = false;
                    action.movementEnabled = true;
                    playerUI[i].inputMode = possiblePlayers[i].bindings.inputMode;
                    playerUI[i].ready = false;
                    spawnedPlayerRegistationPuck.GetComponentInChildren<Image>().color = possiblePlayers[i].color;
                    playerUI[i].playerColor = possiblePlayers[i].color;
                    spawnedPlayerRegistationPuck.GetComponentInChildren<Text>().text = possiblePlayers[i].abbreviation;
                    playerUI[i].playerName = possiblePlayers[i].name;
                }
            }
            else
            {
                //check if ready
                if (playerSelections[i].selectedCharacter != null)
                {
                    if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE && Input.GetMouseButtonDown(0))
                    {
                            //toggle ready
                        playerUI[i].ready = playersReady[i] = !playersReady[i];

                    }
                    else if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK)
                    {
                        if (Input.GetAxis(possiblePlayers[i].bindings.AcceptAxis) != 0)
                        {
                            if (!previousAxisInputNonzero[i])
                            {
                                playerUI[i].ready = playersReady[i] = !playersReady[i]; //toggle
                            }
                            previousAxisInputNonzero[i] = true;
                        }
                        else
                        {
                            previousAxisInputNonzero[i] = false;
                        }
                    }
                }

                if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE && Input.GetMouseButtonDown(1)
                    || possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK && Input.GetAxis(possiblePlayers[i].bindings.genericAbilityAxis) != 0) //deregister
                {
                    Destroy(playerSelections[i].gameObject);
                    playerSelections[i] = null;
                    playerUI[i].Despawn();
                    playerUI[i] = null;
                    playersReady[i] = false;
                }

                
            }
        }
        bool allReady = true;
        bool oneReady = false;
        for (int i = 0; i < playersReady.Length; i++)
        {
            if (playerSelections[i] != null)
            {
                oneReady = true;
                if (!playersReady[i])
                    allReady = false;
            }
        }
        if (startCountdown == null)
        {
            if (oneReady && allReady)
            {
                introMusic = Instantiate(introMusicPrefab);
                startCountdown = Callback.FireAndForget(startGame, 5, this);
                Debug.Log("Counting Down");
            }
        }
        else
        {
            if (!oneReady || !allReady)
            {
                StopCoroutine(startCountdown);
                startCountdown = null;
                Destroy(introMusic);
            }
        }
     * }
         */

    void startGame()
    {
        int count = 0;
        for(int i = 0; i < playerSelections.Length; i++)
        {
            if (playerSelections[i] != null && validCharacterID(playerSelections[i].SelectedCharacterID))
                count++;
        }
        //if count == 0 do something to reset
        data.playerComponentPrefabs = new PlayerComponents[count];

        count = 0;
        for (int i = 0; i < playerSelections.Length; i++)
        {
            if (playerSelections[i] != null && validCharacterID(playerSelections[i].SelectedCharacterID))
                data.playerComponentPrefabs[count++] = new PlayerComponents(charactersData[playerSelections[i].SelectedCharacterID].character, possiblePlayers[i].bindings);
        }
        Application.LoadLevel(mainGameSceneName);
        Destroy(this);
    }

    public bool validCharacterID(int characterID)
    {
        return characterID >= 0 && characterID < charactersData.Length;
    }

    enum RegistrationState
    {
        NOTREGISTERED,
        REGISTERING,
        READY
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