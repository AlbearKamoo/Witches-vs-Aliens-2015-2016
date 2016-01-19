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

    Countdown startCountdown;
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

        startCountdown = Countdown.TimedCountdown(startGame, 5, this);
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
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            switch (registrationStates[i])
            {
                case RegistrationState.NOTREGISTERED:
                    if (pressedAccept(i)) //register
                    {
                        GameObject spawnedPlayerRegistationPuck = (GameObject)Instantiate(playerRegistrationPrefab, Vector2.zero, Quaternion.identity); //the positions are temporary
                        Stats spawnedStats = spawnedPlayerRegistationPuck.AddComponent<Stats>();
                        spawnedStats.playerID = Stats.nextPlayerID();
                        spawnedStats.networkMode = NetworkMode.UNKNOWN; //TODO : change when we add networking to registration

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
                        playerUI[i].playerColor = spawnedPlayerRegistationPuck.GetComponentInChildren<Image>().color = spawnedPlayerRegistationPuck.GetComponent<ParticleSystem>().startColor = possiblePlayers[i].color;
                        spawnedPlayerRegistationPuck.GetComponentInChildren<Text>().text = possiblePlayers[i].abbreviation;
                        playerUI[i].playerName = possiblePlayers[i].name;

                        registrationStates[i] = RegistrationState.REGISTERING;
                    }
                    break;

                case RegistrationState.REGISTERING:
                    //check deregistration
                    if ( pressedBack(i)) //deregister
                    {
                        Destroy(playerSelections[i].gameObject);
                        playerSelections[i] = null;
                        playerUI[i].Despawn();
                        playerUI[i] = null;
                        registrationStates[i] = RegistrationState.NOTREGISTERED;
                    }
                    else if (playerSelections[i].selectedCharacter != null //if they've made a choice
                    && pressedAccept(i)) //ready
                    {
                        registrationStates[i] = RegistrationState.READY;
                        playerUI[i].ready = true;
                    }
                    break;

                case RegistrationState.READY:
                    if (pressedBack(i)) //not ready
                    {
                        registrationStates[i] = RegistrationState.REGISTERING;
                        playerUI[i].ready = false;
                    }
                    break;
            }
        }

        //now check if all are ready
        bool ready = registrationStates.All<RegistrationState>((RegistrationState s) => s != RegistrationState.REGISTERING) && registrationStates.Any<RegistrationState>((RegistrationState s) => s == RegistrationState.READY);
        if (!startCountdown.active)
        {
            if (ready)
            {
                introMusic = Instantiate(introMusicPrefab);
                startCountdown.Start();
                for (int i = 0; i < playerSelections.Length; i++)
                    if (registrationStates[i] == RegistrationState.READY)
                        playerSelections[i].GetComponent<InputToAction>().movementEnabled = false;
            }
        }
        else
        {
            if (!ready)
            {
                startCountdown.Stop();
                startCountdown = null;
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
            if (playerSelections[i] != null && playerSelections[i].selectedCharacter != null)
                count++;
        }
        //if count == 0 do something to reset
        data.playerComponentPrefabs = new PlayerComponents[count];

        count = 0;
        for (int i = 0; i < playerSelections.Length; i++)
        {
            if (playerSelections[i] != null && playerSelections[i].selectedCharacter != null)
                data.playerComponentPrefabs[count++] = new PlayerComponents(playerSelections[i].selectedCharacter, possiblePlayers[i].bindings);
        }
        Application.LoadLevel(mainGameSceneName);
        Destroy(this);
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