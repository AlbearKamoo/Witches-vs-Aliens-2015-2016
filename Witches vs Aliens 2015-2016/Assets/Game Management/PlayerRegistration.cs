using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

[RequireComponent(typeof(SetupData))]
public class PlayerRegistration : MonoBehaviour, INetworkable {
    [SerializeField]
    protected GameObject puckPrefab;

    [SerializeField]
    protected GameObject meshInteraction;

    [SerializeField]
    protected GameObject nullSuperPrefab;

    [SerializeField]
    protected AudioClip countdownVoice;

    [SerializeField]
    protected Transform puckSpawnPoint;
   
    GameObject introMusic;

    [SerializeField]
    protected string mainGameSceneName;
    [SerializeField]
    protected string mainMenuSceneName;

    [SerializeField]
    protected GameObject playerRegistrationPrefab;

    [SerializeField]
    protected GameObject playerRegistrationUIPrefab;

    [SerializeField]
    protected PlayerRegisters[] possiblePlayers;

    [SerializeField]
    public CharacterHolder[] charactersData; //this array maps the characters to ints, for networking.

    [SerializeField]
    protected KeyCode[] startGameKeyBindings;

    [SerializeField]
    protected float keyHoldThreshold;

    [SerializeField]
    [AutoLink(parentTag = Tags.canvas, childPath = "RegisteredWitchPlayers")]
    protected Transform UIParentWitch;

    [SerializeField]
    [AutoLink(parentTag = Tags.canvas, childPath = "RegisteredAlienPlayers")]
    protected Transform UIParentAlien;

    SetupData data;
    Dictionary<int, Registration> registeredPlayers = new Dictionary<int, Registration>(); //int key is the playerID

    List<Transform> leftPlayers = new List<Transform>();
    public List<Transform> LeftPlayers { get { return leftPlayers; } }
    List<Transform> rightPlayers = new List<Transform>();
    public List<Transform> RightPlayers { get { return rightPlayers; } }

    int[] localIDToPlayerID;

    Vector2[] joystickEdgeTriggers;
    Vector2[] joystickEdgeTriggersXY;

    float[] pressedTimes;

    NetworkNode node;
    NetworkMode mode;

    IEnumerator startCountdown;

    GameObject pressStart;

    GameObject puck;

    static PersistentRegistration[] previousRegistrationData = new PersistentRegistration[0];

    void Awake()
    {
        Assert.IsTrue(Time.timeScale == 1);
        if (Time.timeScale != 1)
        {
            Pause.unPause();
        }

        data = GetComponent<SetupData>();

        localIDToPlayerID = new int[possiblePlayers.Length];
        for (int i = 0; i < localIDToPlayerID.Length; i++)
            localIDToPlayerID[i] = -1;

        joystickEdgeTriggers = new Vector2[possiblePlayers.Length];
        for (int i = 0; i < joystickEdgeTriggers.Length; i++)
            joystickEdgeTriggers[i] = Vector2.zero;

        joystickEdgeTriggersXY = new Vector2[possiblePlayers.Length];
        for (int i = 0; i < joystickEdgeTriggersXY.Length; i++)
            joystickEdgeTriggersXY[i] = Vector2.zero;

        pressedTimes = new float[possiblePlayers.Length];
        for (int i = 0; i < pressedTimes.Length; i++)
            pressedTimes[i] = 0;

        pressStart = GetComponentInChildren<Canvas>().gameObject;
        pressStart.SetActive(false);
    }

    void Start()
    {
        puck = Instantiate(puckPrefab);

        puck.GetComponent<Rigidbody2D>().velocity = puck.GetComponent<ISpeedLimiter>().maxSpeed * Random.insideUnitCircle;
        GameObject.Instantiate(meshInteraction).transform.SetParent(puck.transform, false);


        node = NetworkNode.node;
        if (node == null) //if null, no networking, server controls it if there is networking
        {
            mode = NetworkMode.UNKNOWN;
            loadPreviousRegistrationData();
        }
        else if (node is Server)
        {
            mode = NetworkMode.LOCALSERVER;
            node.Subscribe(this);
        }
        else if (node is Client)
        {
            mode = NetworkMode.REMOTECLIENT;
            node.Subscribe(this);
        }

        for (int i = 0; i < charactersData.Length; i++)
        {
            charactersData[i].characterID = i;
        }
    }

    void loadPreviousRegistrationData()
    {
        for (int i = 0; i < previousRegistrationData.Length; i++)
        {
            PersistentRegistration player = previousRegistrationData[i];
            if (player.localID >= 0)
                spawnPlayerRegistration(player.localID, player.playerID, player.networkMode);
            else
                spawnPlayerRegistration(player.playerID, player.networkMode);

            setPlayerReady(player.playerID, player.selectedCharacterID);
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
        checkPressedAccept();
        checkPressedBack();
    }

    void checkPressedAccept()
    {
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            if (registeredPlayers.ContainsKey(localIDToPlayerID[i]))
            {
                if (registeredPlayers[localIDToPlayerID[i]].registrationState == RegistrationState.READY)
                {
                    continue; //we shouldn't check
                }
            }
            //otherwise, check
            if (pressedAccept(i)) //register
            {
                OnPressedAccept(i);
            }
        }
    }

    void checkPressedBack()
    {
        if (registeredPlayers.Count == 0)
        {
            for (int i = 0; i < possiblePlayers.Length; i++)
            {
                if (pressedBack(i))
                    EndGame();
            }
        }
        else
        {
            for (int i = 0; i < possiblePlayers.Length; i++)
            {
                if (heldBack(i))
                {
                    pressedTimes[i] += Time.deltaTime;
                    Debug.Log(pressedTimes[i]);
                    if (pressedTimes[i] > keyHoldThreshold)
                        EndGame();
                }
                else
                {
                    pressedTimes[i] = 0;
                }
            }
        }

        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            if (registeredPlayers.ContainsKey(localIDToPlayerID[i]))
            {
                if (pressedBack(i)) //deregister
                {
                    OnPressedBack(i);
                }
            }
        }
    }

    void EndGame()
    {
        if (node != null)
            Destroy(node.gameObject);

        Destroy(this);
        Destroy(pressStart.gameObject);

        Application.LoadLevel(mainMenuSceneName);
    }

    RegistrationState localIDToState(int localID)
    {
        if (localIDToPlayerID[localID] == -1)
            return RegistrationState.NOTREGISTERED;
        return registeredPlayers[localIDToPlayerID[localID]].registrationState;
    }

    void OnPressedAccept(int localID)
    {
        pressedTimes[localID] = 0;

        switch (localIDToState(localID))
        {
            case RegistrationState.NOTREGISTERED:
                {
                    int playerID = Stats.nextPlayerID();
                    switch (mode)
                    {
                        case NetworkMode.REMOTECLIENT:
                            node.BinaryWriter.Write(PacketType.REGISTRATIONREQUEST);
                            node.BinaryWriter.Write((byte)(localID));
                            node.Send(node.AllCostChannel);
                            break;
                        case NetworkMode.LOCALSERVER:
                            node.BinaryWriter.Write(PacketType.PLAYERREGISTER);
                            node.BinaryWriter.Write((byte)(playerID));
                            node.BinaryWriter.Write((byte)(RegistrationState.REGISTERING));
                            node.Send(node.AllCostChannel);
                            spawnPlayerRegistration(localID, playerID, NetworkMode.LOCALSERVER);
                            break;
                        default:
                            spawnPlayerRegistration(localID, playerID, NetworkMode.UNKNOWN);
                            break;
                    }
                    break;
                }
            case RegistrationState.REGISTERING:
                {
                    int playerID = localIDToPlayerID[localID];
                    if (validCharacterID(registeredPlayers[playerID]))
                    {
                        switch (mode)
                        {
                            case NetworkMode.LOCALSERVER:
                                setPlayerReady(playerID);
                                goto case NetworkMode.REMOTECLIENT;
                            case NetworkMode.REMOTECLIENT:
                                node.BinaryWriter.Write(PacketType.PLAYERREGISTER);
                                node.BinaryWriter.Write((byte)(playerID));
                                node.BinaryWriter.Write((byte)(RegistrationState.READY));
                                node.BinaryWriter.Write((byte)(registeredPlayers[playerID].SelectedCharacterID));
                                node.Send(node.AllCostChannel);
                                break;
                            default:
                                setPlayerReady(playerID);
                                break;
                        }
                    }
                    break;
                }
            //case RegistrationState.READY: //don't need to do anything
        }
    }

    void spawnPlayerRegistration(int playerID, NetworkMode networkMode)
    {
        GameObject spawnedPlayerRegistrationPuck = SpawnPlayerRegistrationPuck(playerID, networkMode);
        SpawnPlayerRegistrationComponents(playerID, spawnedPlayerRegistrationPuck, networkMode);
        checkReady();
        checkNotReady();
    }

    void spawnPlayerRegistration(int localID, int playerID, NetworkMode networkMode)
    {
        GameObject spawnedPlayerRegistrationPuck = SpawnPlayerRegistrationPuck(localID, playerID, networkMode);
        SpawnPlayerRegistrationComponentsAndLocalInfo(localID, playerID, spawnedPlayerRegistrationPuck, networkMode);
        checkReady();
        checkNotReady();
    }

    /*
    void spawnPlayerRegistration(int localID, int playerID, NetworkMode networkMode)
    {
        GameObject spawnedPlayerRegistationPuck = (GameObject)Instantiate(playerRegistrationPrefab, Vector2.zero, Quaternion.identity); //the positions are temporary
        Stats spawnedStats = spawnedPlayerRegistationPuck.AddComponent<Stats>();
        spawnedStats.playerID = playerID;
        spawnedStats.networkMode = networkMode;
        switch (networkMode)
        {
            case NetworkMode.REMOTECLIENT:
            case NetworkMode.REMOTESERVER:
                break;
            default:
                switch (possiblePlayers[localID].bindings.inputMode)
                {
                    case InputConfiguration.PlayerInputType.MOUSE:
                        spawnedPlayerRegistationPuck.AddComponent<MousePlayerInput>().bindings = possiblePlayers[localID].bindings;
                        break;
                    case InputConfiguration.PlayerInputType.JOYSTICK:
                        spawnedPlayerRegistationPuck.AddComponent<JoystickCustomDeadZoneInput>().bindings = possiblePlayers[localID].bindings;
                        break;
                }
                break;
        }

         * spawn them
        playerSelections[localID] = spawnedPlayerRegistationPuck.AddComponent<CharacterSelector>();
        playerUI[localID] = SimplePool.Spawn(playerRegistrationUIPrefab).GetComponent<RegisteredPlayerUIView>();
        playerUI[localID].transform.SetParent(UIParent, Vector3.one, false);
        InputToAction action = spawnedPlayerRegistationPuck.GetComponent<InputToAction>();
        action.rotationEnabled = false;
        action.movementEnabled = true;
        playerUI[localID].inputMode = possiblePlayers[localID].bindings.inputMode;
        playerUI[localID].ready = false;
        playerUI[localID].playerColor = spawnedPlayerRegistationPuck.GetComponentInChildren<Image>().color = spawnedPlayerRegistationPuck.GetComponent<ParticleSystem>().startColor = possiblePlayers[localID].color;
        spawnedPlayerRegistationPuck.GetComponentInChildren<Text>().text = possiblePlayers[localID].abbreviation;
        playerUI[localID].playerName = possiblePlayers[localID].name;

        registrationStates[localID] = RegistrationState.REGISTERING;
    }
     * */

    GameObject SpawnPlayerRegistrationPuck(int playerID, NetworkMode networkMode)
    {
        GameObject spawnedPlayerRegistrationPuck = (GameObject)Instantiate(playerRegistrationPrefab, puckSpawnPoint.position, Quaternion.identity); //the positions are temporary
        Stats spawnedStats = spawnedPlayerRegistrationPuck.AddComponent<Stats>();
        spawnedStats.playerID = playerID;
        spawnedStats.networkMode = networkMode;
        return spawnedPlayerRegistrationPuck;
    }

    GameObject SpawnPlayerRegistrationPuck(int localID, int playerID, NetworkMode networkMode)
    {
        GameObject spawnedPlayerRegistrationPuck = SpawnPlayerRegistrationPuck(playerID, networkMode);
        switch (networkMode)
        {
            case NetworkMode.REMOTECLIENT:
            case NetworkMode.REMOTESERVER:
                break;
            default:
                switch (possiblePlayers[localID].bindings.inputMode)
                {
                    case InputConfiguration.PlayerInputType.MOUSE:
                        spawnedPlayerRegistrationPuck.AddComponent<MousePlayerInput>().bindings = possiblePlayers[localID].bindings;
                        break;
                    case InputConfiguration.PlayerInputType.JOYSTICK:
                        spawnedPlayerRegistrationPuck.AddComponent<JoystickCustomDeadZoneInput>().bindings = possiblePlayers[localID].bindings;
                        break;
                }
                break;
        }
        return spawnedPlayerRegistrationPuck;
    }

    void SpawnPlayerRegistrationComponents(int playerID, GameObject spawnedPlayerRegistrationPuck, NetworkMode networkMode, int localID = -1)
    {
        CharacterSelector selector = spawnedPlayerRegistrationPuck.AddComponent<CharacterSelector>();

        InputToAction action = spawnedPlayerRegistrationPuck.GetComponent<InputToAction>();
        action.rotationEnabled = false;
        action.movementEnabled = true;



        Color playerColor;
        if (registeredPlayers.ContainsKey(playerID))
            playerColor = registeredPlayers[playerID].color;
        else if (localID != -1)
            playerColor = possiblePlayers[localID].color;
        else
            playerColor = HSVColor.HSVToRGB(Random.value, 1, 1);


        registeredPlayers[playerID] = new Registration(selector, null, RegistrationState.REGISTERING, localID, networkMode, this);

        selector.gameObject.GetComponentInChildren<Image>().color = selector.gameObject.GetComponent<ParticleSystem>().startColor = registeredPlayers[playerID].color = playerColor;

        selector.registration = registeredPlayers[playerID];
        
    }

    void SpawnPlayerRegistrationComponentsAndLocalInfo(int localID, int playerID, GameObject spawnedPlayerRegistrationPuck, NetworkMode networkMode)
    {
        SpawnPlayerRegistrationComponents(playerID, spawnedPlayerRegistrationPuck, networkMode, localID : localID);

        localIDToPlayerID[localID] = playerID;
    }

    void setPlayerReady(int playerID)
    {
        Registration data = registeredPlayers[playerID];
        data.ready = true;
        CharacterHolder characterHolder = data.context.charactersData[data.SelectedCharacterID];
        characterHolder.Select();

        RegisteredPlayerUIView ui = SimplePool.Spawn(playerRegistrationUIPrefab).GetComponent<RegisteredPlayerUIView>();

        Transform UIParent;
        switch (characterHolder.character.side)
        {
            default:
            case Side.LEFT: //witch
                UIParent = UIParentWitch;
                break;
            case Side.RIGHT:
                UIParent = UIParentAlien;
                break;
        }
        if (data.localID != -1)
            ui.playerColor = possiblePlayers[data.localID].color;
        else
            ui.playerColor = data.color;
        ui.transform.SetParent(UIParent, Vector3.one, false);
        ui.registration = data;
        data.ui = ui;

        Vector2 echoPosition = data.selector.transform.position;

        Destroy(data.selector.gameObject);
        data.selector = null;

        spawnPlaygroundAvatar(playerID);

        ui.UpdateCharacterSprite(data.SelectedCharacterID);

        data.echoPosition = echoPosition;

        checkReady();
    }

    void setPlayerReady(int playerID, int characterID)
    {
        Assert.IsTrue(registeredPlayers[playerID].SelectedCharacterID != characterID);
        registeredPlayers[playerID].SelectedCharacterID = characterID;
        setPlayerReady(playerID);
    }

    void spawnPlaygroundAvatar(int playerID)
    {
        Registration data = registeredPlayers[playerID];
        CharacterComponents character = charactersData[data.SelectedCharacterID].character;

        GameObject spawnedPlayer = (GameObject)Instantiate(character.basePlayer);
        Stats spawnedStats = spawnedPlayer.AddComponent<Stats>();
        spawnedStats.side = character.side;
        spawnedStats.playerID = playerID;
        spawnedStats.networkMode = data.networkMode;

        AbstractPlayerInput input;

        switch (data.networkMode)
        {
            case NetworkMode.REMOTECLIENT:
            case NetworkMode.REMOTESERVER:
                break;
            default:
                if (data.localID != -1)
                {
                    switch (possiblePlayers[data.localID].bindings.inputMode)
                    {
                        case InputConfiguration.PlayerInputType.MOUSE:
                            input = spawnedPlayer.AddComponent<MousePlayerInput>();
                            input.bindings = possiblePlayers[data.localID].bindings;
                            break;
                        case InputConfiguration.PlayerInputType.JOYSTICK:
                            input = spawnedPlayer.AddComponent<JoystickCustomDeadZoneInput>();
                            input.bindings = possiblePlayers[data.localID].bindings;
                            break;
                    }
                }
                break;
        }
        GameObject.Instantiate(character.movementAbility).transform.SetParent(spawnedPlayer.transform, false);
        GameObject.Instantiate(character.genericAbility).transform.SetParent(spawnedPlayer.transform, false);
        GameObject.Instantiate(nullSuperPrefab).transform.SetParent(spawnedPlayer.transform, false);

        GameObject instantiatedMeshInteraction = GameObject.Instantiate(meshInteraction);
        instantiatedMeshInteraction.transform.SetParent(spawnedPlayer.transform, false);
        instantiatedMeshInteraction.GetComponent<ParticleSystem>().startColor = data.localID != -1 ? possiblePlayers[data.localID].color : data.color;

        GameObject visuals = GameObject.Instantiate(character.visuals);
        visuals.transform.SetParent(spawnedPlayer.transform, false);
        IHueShiftableVisuals huedVisuals = visuals.GetComponent<IHueShiftableVisuals>();
        if (huedVisuals != null)
        {
            data.playgroundAvatarVisuals = huedVisuals;
            huedVisuals.shiftAsync = data.ui.CharacterVisualsVector;
        }
        data.playgroundAvatar = spawnedPlayer;

        InputToAction action = spawnedPlayer.GetComponent<InputToAction>();
        action.movementEnabled = true;
        action.abilitiesEnabled = true;

        switch(character.side)
        {
            case Side.LEFT:
                leftPlayers.Add(spawnedPlayer.transform);
                break;
            case Side.RIGHT:
                rightPlayers.Add(spawnedPlayer.transform);
                break;
        }

        foreach (AbstractAbility ability in spawnedPlayer.GetComponentsInChildren<AbstractAbility>())
        {
            if (ability is IOpponentsAbility)
            {
                switch (character.side)
                {
                    case Side.LEFT:
                        (ability as IOpponentsAbility).opponents = rightPlayers;
                        break;
                    case Side.RIGHT:
                        (ability as IOpponentsAbility).opponents = leftPlayers;
                        break;
                }
            }
            if (ability is IAlliesAbility)
            {
                switch (character.side)
                {
                    case Side.LEFT:
                        (ability as IAlliesAbility).allies = leftPlayers;
                        break;
                    case Side.RIGHT:
                        (ability as IAlliesAbility).allies = rightPlayers;
                        break;
                }
            }
            if (ability is IPuckAbility)
            {
                (ability as IPuckAbility).puck = puck.transform;
            }
        }
        //Callback.FireForUpdate(() => spawnedPlayer.GetComponent<ResetScripting>().Reset(echoPosition, 0f), this);
    }

    void OnPressedBack(int localID)
    {
        pressedTimes[localID] = 0;

        int playerID = localIDToPlayerID[localID];
        switch (localIDToState(localID))
        {
            //case RegistrationState.NOTREGISTERED: //don't need to do anything
            case RegistrationState.REGISTERING:
                switch (mode)
                {
                    case NetworkMode.LOCALSERVER:
                        despawnPlayerRegistrationPuck(localID, playerID);
                        goto case NetworkMode.REMOTECLIENT;
                    case NetworkMode.REMOTECLIENT:
                        node.BinaryWriter.Write(PacketType.PLAYERDEREGISTER);
                        node.BinaryWriter.Write((byte)(playerID));
                        node.BinaryWriter.Write((byte)(RegistrationState.NOTREGISTERED)); //the new state
                        node.Send(node.AllCostChannel);
                        break;
                    default:
                        despawnPlayerRegistrationPuck(localID, playerID);
                        break;
                }
                break;
            case RegistrationState.READY:
                switch (mode)
                {
                    case NetworkMode.LOCALSERVER:
                        setPlayerNotReady(playerID);
                        goto case NetworkMode.REMOTECLIENT;
                    case NetworkMode.REMOTECLIENT:
                        node.BinaryWriter.Write(PacketType.PLAYERDEREGISTER);
                        node.BinaryWriter.Write((byte)(playerID));
                        node.BinaryWriter.Write((byte)(RegistrationState.REGISTERING)); //the new state
                        node.Send(node.AllCostChannel);
                        break;
                    default:
                        setPlayerNotReady(playerID);
                        break;
                }
                break;
        }
    }

    void despawnPlayerRegistrationPuck(int playerID)
    {
        Registration toDespawn = registeredPlayers[playerID];
        Destroy(toDespawn.selector.gameObject);
        registeredPlayers.Remove(playerID);
        checkReady();
        checkNotReady();
    }

    void despawnPlayerRegistrationPuck(int localID, int playerID)
    {
        despawnPlayerRegistrationPuck(playerID);
        localIDToPlayerID[localID] = -1;
    }

    void despawnPlayerRegistrationPuck(int playerID, bool notLocal)
    {
        despawnPlayerRegistrationPuck(playerID);
        if (notLocal)
        {
            for (int i = 0; i < localIDToPlayerID.Length; i++)
            {
                if (localIDToPlayerID[i] == playerID)
                {
                    localIDToPlayerID[i] = -1;
                    return;
                }
            }
        }
    }

    void setPlayerNotReady(int playerID)
    {
        Registration data = registeredPlayers[playerID];
        data.ready = false;
        data.ui.Despawn();
        data.ui = null;

        Vector2 playgroundEchoPosition = data.playgroundAvatar.transform.position;

        switch (charactersData[data.SelectedCharacterID].character.side)
        {
            case Side.LEFT:
                leftPlayers.Remove(data.playgroundAvatar.transform);
                break;
            case Side.RIGHT:
                rightPlayers.Remove(data.playgroundAvatar.transform);
                break;
        }

        data.playgroundAvatar.GetComponent<ResetScripting>().Reset(Vector2.zero, 0f);
        Destroy(data.playgroundAvatar);
        data.playgroundAvatar = null;
        data.playgroundAvatarVisuals = null;

        if (registeredPlayers[playerID].localID != -1)
            spawnPlayerRegistration(data.localID, playerID, data.networkMode);
        else
            spawnPlayerRegistration(playerID, data.networkMode);

        //spawnPlayerRegistration creates a new registration

        registeredPlayers[playerID].selector.transform.position = data.echoPosition;

        registeredPlayers[playerID].echoPosition = playgroundEchoPosition;
        checkNotReady();
    }

    bool isReady()
    {
        bool atLeastOneReady = false;
        int sideDiff = 0;
        foreach(Registration r in registeredPlayers.Values)
        {
            if(r.registrationState == RegistrationState.READY)
                atLeastOneReady = true;
            else
                return false; //everyone has to be ready

            if (GameSelection.balancedTeams)
            {
                switch (r.context.charactersData[r.SelectedCharacterID].character.side)
                {
                    case Side.LEFT:
                        sideDiff--;
                        break;
                    default:
                    case Side.RIGHT:
                        sideDiff++;
                        break;
                }
            }
        }
        return atLeastOneReady && sideDiff == 0;
    }

    void checkReady()
    {
        if (startCountdown == null && isReady())
        {
            pressStart.SetActive(true);
            startCountdown = startGameCountdown();
            StartCoroutine(startCountdown);
        }
    }

    IEnumerator startGameCountdown()
    {
        for (; ; )
        {
            yield return null;

            for (int i = 0; i < startGameKeyBindings.Length; i++)
            {
                if (Input.GetKeyDown(startGameKeyBindings[i]))
                {
                    attemptStartGame();
                    yield break;
                }
            }

            for (int i = 0; i < possiblePlayers.Length; i++)
            {
                int playerID = localIDToPlayerID[i];
                if (registeredPlayers.ContainsKey(playerID) && registeredPlayers[playerID].registrationState == RegistrationState.READY)
                {
                    bool pressed = pressedAccept(i);

                    if (pressed)
                    {
                        attemptStartGame();
                        yield break;
                    }
                    /*
                    else if (pressedBack(i))
                    {

                    }
                    */
                }
            }
        }
    }

    void checkNotReady()
    {
        if(startCountdown != null && !isReady())
        {
            StopCoroutine(startCountdown);
            startCountdown = null;
            pressStart.SetActive(false);
        }
    }

    bool pressedAccept(int i)
    {
        if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE)
        {
            if(registeredPlayers.ContainsKey(localIDToPlayerID[i]))
            {
                Registration data = registeredPlayers[localIDToPlayerID[i]];
                if (data.registrationState == RegistrationState.READY)
                {
                    return Input.GetKeyDown(KeyCode.Space);
                }
            }
            //otherwise
            return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        }
        else if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK)
        {
            //ability axis
            float currentAxisValue = Input.GetAxis(possiblePlayers[i].bindings.movementAbilityAxis);

            bool returnValue = false;
            if (currentAxisValue != 0 && joystickEdgeTriggers[i].x == 0)
            {
                returnValue = true;
            }

            joystickEdgeTriggers[i].x = currentAxisValue;

            if (registeredPlayers.ContainsKey(localIDToPlayerID[i]))
            {
                Registration data = registeredPlayers[localIDToPlayerID[i]];
                if (data.registrationState == RegistrationState.READY)
                {
                    returnValue = false; //disable ability axis, because ability axis is used in the playground
                }
            }

            //now XY axis
            currentAxisValue = Input.GetAxis(possiblePlayers[i].bindings.acceptAbilityAxis);

            if (currentAxisValue != 0 && joystickEdgeTriggersXY[i].x == 0)
            {
                returnValue = true;
            }

            joystickEdgeTriggersXY[i].x = currentAxisValue;

            return returnValue;
        }
        return false;
    }

    //TODO : switch to calling the pressed/pressing Accept and Back in PlayerInput scripts
    bool pressedBack(int i)
    {
        if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE)
        {
            if (registeredPlayers.ContainsKey(localIDToPlayerID[i]))
            {
                Registration data = registeredPlayers[localIDToPlayerID[i]];
                if (data.registrationState == RegistrationState.READY)
                {
                    return Input.GetKeyDown(KeyCode.Escape);
                }
            }
            //otherwise
            return Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1);
        }
        else if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.JOYSTICK)
        {
            float currentAxisValue = Input.GetAxis(possiblePlayers[i].bindings.genericAbilityAxis);

            bool returnValue = false;
            if (currentAxisValue != 0 && joystickEdgeTriggers[i].y == 0)
                returnValue = true;

            joystickEdgeTriggers[i].y = currentAxisValue;

            if (registeredPlayers.ContainsKey(localIDToPlayerID[i]))
            {
                Registration data = registeredPlayers[localIDToPlayerID[i]];
                if (data.registrationState == RegistrationState.READY)
                {
                    returnValue = false; //disable ability axis, because ability axis is used in the playground
                }
            }

            //now XY axis
            currentAxisValue = Input.GetAxis(possiblePlayers[i].bindings.backAbilityAxis);

            if (currentAxisValue != 0 && joystickEdgeTriggersXY[i].y == 0)
            {
                returnValue = true;
            }

            joystickEdgeTriggersXY[i].y = currentAxisValue;

            return returnValue;
        }
        return false;
    }

    bool heldBack(int i)
    {
        if (possiblePlayers[i].bindings.inputMode == InputConfiguration.PlayerInputType.MOUSE)
        {
            return Input.GetKey(KeyCode.Escape) || Input.GetMouseButton(1);
        }
        else
        {
            return Input.GetAxis(possiblePlayers[i].bindings.genericAbilityAxis) != 0 || Input.GetAxis(possiblePlayers[i].bindings.backAbilityAxis) != 0;
        }
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

    void attemptStartGame()
    {
        if (node == null) //local networking
        {
            startGame();
        }
        else
        {
            switch (node.networkMode)
            {
                case NetworkMode.LOCALCLIENT:
                    //do nothing; only the server has the power
                    break;
                case NetworkMode.LOCALSERVER:
                    node.BinaryWriter.Write(PacketType.SCENEJUMP);
                    node.Send(node.AllCostChannel);
                    startGame();
                    break;
            }
        }
    }

    void startGame()
    {
        List<PlayerComponents> results = new List<PlayerComponents>();
        foreach (KeyValuePair<int, Registration> entry in registeredPlayers)
        {
            InputConfiguration config = entry.Value.localID != -1 ? possiblePlayers[entry.Value.localID].bindings : new InputConfiguration();
            config.networkMode = entry.Value.networkMode;
            results.Add(new PlayerComponents(charactersData[entry.Value.SelectedCharacterID].character, config, entry.Key, entry.Value.ui.CharacterVisualsVector));
        }

        data.playerComponentPrefabs = results.ToArray();

        data.warmupActive = GameSelection.warmupActive;

        previousRegistrationData = PersistentRegistration.extractPersistentData(registeredPlayers);

        Debug.Log(mainGameSceneName);
        if (node != null)
        {
            node.Unsubscribe(this);
            node.Clear();
        }
        Destroy(this);
        Destroy(pressStart.gameObject);

        Application.LoadLevel(mainGameSceneName);
    }

    public bool validCharacterID(Registration registration)
    {
        return registration.SelectedCharacterID >= 0 && registration.SelectedCharacterID < charactersData.Length;
    }

    public enum RegistrationState
    {
        NOTREGISTERED,
        REGISTERING,
        READY
    }

    public class Registration
    {
        public CharacterSelector selector;
        public RegisteredPlayerUIView ui;
        public RegistrationState registrationState;
        public PlayerRegistration context;
        private int selectedCharacterID = -1;
        public int localID;
        public Color color;
        public NetworkMode networkMode;
        public IHueShiftableVisuals playgroundAvatarVisuals;
        public GameObject playgroundAvatar;
        public Vector2 echoPosition;
        public int SelectedCharacterID
        {
            get { return selectedCharacterID; }
            set
            {
                selectedCharacterID = value;
            }
        }
        public Registration(CharacterSelector selector, RegisteredPlayerUIView ui, RegistrationState registrationState, NetworkMode networkMode, PlayerRegistration context)
        {
            this.selector = selector;
            this.ui = ui;
            this.registrationState = registrationState;
            this.context = context;
            this.localID = -1;
            this.networkMode = networkMode;
            this.echoPosition = Vector2.zero;
        }

        public Registration(CharacterSelector selector, RegisteredPlayerUIView ui, RegistrationState registrationState, int localID, NetworkMode networkMode, PlayerRegistration context)
            : this(selector, ui, registrationState, networkMode, context)
        {
            this.localID = localID;
        }
        public bool ready
        {
            set
            {
                if (value)
                {
                    registrationState = RegistrationState.READY;
                }
                else
                {
                    registrationState = RegistrationState.REGISTERING;
                }
            }
        }
    }

    public PacketType[] packetTypes { get { return new PacketType[] { PacketType.REGISTRATIONREQUEST, PacketType.PLAYERREGISTER, PacketType.PLAYERDEREGISTER, PacketType.SCENEJUMP }; } }

    public void Notify(IncomingNetworkStreamMessage m)
    {
        Debug.Log(mode);
        Debug.Log(m.packetType);
        switch (m.packetType)
        {
            case PacketType.REGISTRATIONREQUEST:
                if (mode == NetworkMode.LOCALSERVER)
                {
                    int playerID = Stats.nextPlayerID();

                    node.BinaryWriter.Write(PacketType.REGISTRATIONREQUEST);
                    node.BinaryWriter.Write(false); //is a confirmation of local control, no
                    //node.BinaryWriter.Write(m.reader.ReadByte()); //local playerID
                    node.BinaryWriter.Write((byte)playerID);
                    node.Send(node.ConnectionIDs.Where((int id) => id != m.connectionID), node.AllCostChannel);

                    spawnPlayerRegistration(playerID, NetworkMode.REMOTESERVER);

                    node.BinaryWriter.Write(PacketType.REGISTRATIONREQUEST);
                    node.BinaryWriter.Write(true); //is a confirmation, yes, is locally controlled
                    node.BinaryWriter.Write((byte)playerID);
                    node.BinaryWriter.Write(m.reader.ReadByte()); //local playerID
                    node.Send(new int[] { m.connectionID }, node.AllCostChannel);
                }
                else // if running on the client
                {
                    bool localControl = m.reader.ReadBoolean();
                    int playerID = m.reader.ReadByte();
                    if (localControl)
                    {
                        int localID = m.reader.ReadByte();
                        spawnPlayerRegistration(localID, playerID, NetworkMode.LOCALCLIENT);
                    }
                    else
                    {
                        spawnPlayerRegistration(playerID, NetworkMode.REMOTECLIENT);
                    }
                }
                break;
            case PacketType.PLAYERREGISTER:
                {
                    int playerID = m.reader.ReadByte();
                    RegistrationState newState = (RegistrationState)(m.reader.ReadByte());
                    switch(newState)
                    {
                        case RegistrationState.REGISTERING:
                            spawnPlayerRegistration(playerID, NetworkMode.REMOTECLIENT);
                            if(mode == NetworkMode.LOCALSERVER)
                                Debug.LogError("Invalid Message Type");
                            break;
                        case RegistrationState.READY:
                            int characterID = m.reader.ReadByte();
                            if (mode == NetworkMode.LOCALSERVER)
                            {
                                node.BinaryWriter.Write(PacketType.PLAYERREGISTER);
                                node.BinaryWriter.Write((byte)(playerID));
                                node.BinaryWriter.Write((byte)(RegistrationState.READY));
                                node.BinaryWriter.Write((byte)(characterID));
                                node.Send(node.AllCostChannel);
                            }
                            setPlayerReady(playerID);

                            break;
                        default:
                            Debug.LogError("Invalid Message Type");
                            break;
                    }
                    break;
                }
            case PacketType.PLAYERDEREGISTER:
                {
                    int playerID = m.reader.ReadByte();
                    RegistrationState newState = (RegistrationState)(m.reader.ReadByte());
                    switch (newState)
                    {
                        case RegistrationState.REGISTERING:
                            if (mode == NetworkMode.LOCALSERVER)
                            {
                                node.BinaryWriter.Write(PacketType.PLAYERDEREGISTER);
                                node.BinaryWriter.Write((byte)(playerID));
                                node.BinaryWriter.Write((byte)(RegistrationState.REGISTERING));
                                node.Send(node.AllCostChannel);
                            }
                            setPlayerNotReady(playerID);
                            break;
                        case RegistrationState.NOTREGISTERED:
                            if (mode == NetworkMode.LOCALSERVER)
                            {
                                node.BinaryWriter.Write(PacketType.PLAYERDEREGISTER);
                                node.BinaryWriter.Write((byte)(playerID));
                                node.BinaryWriter.Write((byte)(RegistrationState.NOTREGISTERED));
                                node.Send(node.AllCostChannel);
                            }
                            despawnPlayerRegistrationPuck(playerID, true);
                            break;
                        default:
                            Debug.LogError("Invalid Message Type");
                            break;
                    }
                    break;
                }
            case PacketType.SCENEJUMP:
                {
                    Assert.IsTrue(startCountdown != null);
                    startGame();
                    break;
                }
            default:
                Debug.LogError("Invalid Message Type");
                break;
        }
    }

    public class PersistentRegistration
    {
        public int playerID;
        public int selectedCharacterID;
        public int localID;
        public NetworkMode networkMode;
        public Vector2 characterVisualsVector;
        public PersistentRegistration(int playerID, int selectedCharacterID, int localID, NetworkMode networkMode, Vector2 characterVisualsVector)
        {
            this.playerID = playerID;
            this.selectedCharacterID = selectedCharacterID;
            this.localID = localID;
            this.networkMode = networkMode;
            this.characterVisualsVector = characterVisualsVector;
        }

        public static PersistentRegistration[] extractPersistentData(Dictionary<int, Registration> registeredPlayers)
        {
            PersistentRegistration[] result = new PersistentRegistration[registeredPlayers.Count];
            int i = 0;
            foreach (KeyValuePair<int, Registration> player in registeredPlayers)
            {
                result[i] = new PersistentRegistration(player.Key, player.Value.SelectedCharacterID, player.Value.localID, player.Value.networkMode, player.Value.ui.CharacterVisualsVector);
                i++;
            }
            return result;
        }
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