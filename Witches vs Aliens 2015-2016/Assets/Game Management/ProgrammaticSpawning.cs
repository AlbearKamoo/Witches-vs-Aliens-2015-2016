using UnityEngine;
using System.Collections;

public class ProgrammaticSpawning : MonoBehaviour, IObserver<Message> {

    [SerializeField]
    protected GameObject PuckPrefab;

    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "Left")]
    protected Transform leftRespawnPointsParent;
    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "Right")]
    protected Transform rightRespawnPointsParent;

    [SerializeField]
    [AutoLink(parentTag = Tags.stage, parentName = "PuckRespawnPoint")]
    protected Transform puckRespawnPoint;

    [SerializeField]
    [AutoLink(parentTag = Tags.canvas, parentName = "Timer")]
    protected GameTimer timer;

    [SerializeField]
    protected float resetDuration;

    [SerializeField]
    protected float goalToResetTime;

    public SetupData data;
    Vector2[] leftPoints;
    Vector2[] rightPoints;
    Transform[] players;
    PuckFX puck;

    // Use this for initialization
    void Awake () {

        if (SetupData.self != null) //workaround for unity level-loading method order
            data = SetupData.self;

        Observers.Subscribe(this, new string[]{GoalScoredMessage.classMessageType});

        leftPoints = new Vector2[leftRespawnPointsParent.childCount];
        int index = 0;
        foreach(Transform child in leftRespawnPointsParent)
        {
            leftPoints[index] = child.position;
            index++;
        }

        index = 0;
        rightPoints = new Vector2[rightRespawnPointsParent.childCount];
        foreach (Transform child in rightRespawnPointsParent)
        {
            rightPoints[index] = child.position;
            index++;
        }

        puck = ((GameObject)(Instantiate(PuckPrefab, puckRespawnPoint.position, Quaternion.identity))).GetComponent<PuckFX>();

        players = new Transform[data.playerComponentPrefabs.Length];
        for (int i = 0; i < data.playerComponentPrefabs.Length; i++)
        {
            GameObject spawnedPlayer = (GameObject)Instantiate(data.playerComponentPrefabs[i].character.basePlayer, i % 2 == 0 ? leftPoints[i % 2] : rightPoints[i % 2], Quaternion.identity); //the positions are temporary
            spawnedPlayer.AddComponent<Stats>().side = data.playerComponentPrefabs[i].character.side;
            switch (data.playerComponentPrefabs[i].bindings.inputMode)
            {
                case InputConfiguration.PlayerInputType.MOUSE:
                    spawnedPlayer.AddComponent<MousePlayerInput>().bindings = data.playerComponentPrefabs[i].bindings;
                    break;
                case InputConfiguration.PlayerInputType.JOYSTICK:
                    spawnedPlayer.AddComponent<JoystickPlayerInput>().bindings = data.playerComponentPrefabs[i].bindings;
                    break;
                case InputConfiguration.PlayerInputType.CRAPAI:
                    spawnedPlayer.AddComponent<CrappyAIInput>().bindings = data.playerComponentPrefabs[i].bindings;
                    break;
            }
            GameObject.Instantiate(data.playerComponentPrefabs[i].character.movementAbility).transform.SetParent(spawnedPlayer.transform, false);
            GameObject.Instantiate(data.playerComponentPrefabs[i].character.genericAbility).transform.SetParent(spawnedPlayer.transform, false);
            //GameObject.Instantiate(data.playerComponentPrefabs[i].character.superAbility).transform.SetParent(spawnedPlayer.transform, false);
            players[i] = spawnedPlayer.transform;
        }
        Callback.FireForNextFrame(() => resetPositions(), this);
	}

    void resetPositions()
    {
        leftPoints.Shuffle<Vector2>();
        rightPoints.Shuffle<Vector2>();
        int leftPointsIndex = 0;
        int rightPointsIndex = 0;
        foreach (Transform t in players)
        {
            switch (t.GetComponent<Stats>().side)
            {
                case Side.LEFT:
                    t.GetComponent<ResetScripting>().Reset(leftPoints[leftPointsIndex], resetDuration);
                    leftPointsIndex++;
                    break;
                case Side.RIGHT:
                    t.GetComponent<ResetScripting>().Reset(rightPoints[rightPointsIndex], resetDuration);
                    rightPointsIndex++;
                    break;
            }
        }
        puck.Respawn(puckRespawnPoint.position);
        Callback.FireAndForget(() => timer.running = true, resetDuration, this);
    }

    public void Notify(Message m)
    {
        switch (m.messageType)
        {
            case GoalScoredMessage.classMessageType:
                timer.running = false;
                Callback.FireAndForget(() => resetPositions(), goalToResetTime, this);
                break;
        }
    }
}
